using AdminFlow.API.Data;
using AdminFlow.API.DTOs;
using AdminFlow.API.Models;
using Microsoft.EntityFrameworkCore;

namespace AdminFlow.API.Services;

public class EnrollmentService(AppDbContext db)
{
    public async Task<PagedResult<EnrollmentDto>> GetAsync(int? courseId, int? studentId, string? status, int page, int pageSize)
    {
        var q = db.Enrollments
            .Include(e => e.Student)
            .Include(e => e.Course)
            .AsQueryable();

        if (courseId.HasValue)   q = q.Where(e => e.CourseId == courseId.Value);
        if (studentId.HasValue)  q = q.Where(e => e.StudentId == studentId.Value);
        if (status is not null)  q = q.Where(e => e.Status == status);

        var total = await q.CountAsync();
        var data = await q
            .OrderByDescending(e => e.EnrolledAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(e => new EnrollmentDto(
                e.Id, e.StudentId, e.Student.Name,
                e.CourseId, e.Course.Title,
                e.Grade, e.Status, e.EnrolledAt
            ))
            .ToListAsync();

        return new PagedResult<EnrollmentDto>(data, total, page, pageSize);
    }

    public async Task<EnrollmentDto> CreateAsync(CreateEnrollmentRequest req)
    {
        var enrollment = new Enrollment { StudentId = req.StudentId, CourseId = req.CourseId };
        db.Enrollments.Add(enrollment);
        await db.SaveChangesAsync();

        return await db.Enrollments
            .Include(e => e.Student).Include(e => e.Course)
            .Where(e => e.Id == enrollment.Id)
            .Select(e => new EnrollmentDto(
                e.Id, e.StudentId, e.Student.Name,
                e.CourseId, e.Course.Title,
                e.Grade, e.Status, e.EnrolledAt
            ))
            .FirstAsync();
    }

    public async Task<EnrollmentDto?> UpdateAsync(int id, UpdateEnrollmentRequest req)
    {
        var e = await db.Enrollments.FindAsync(id);
        if (e is null) return null;

        if (req.Grade.HasValue) e.Grade = req.Grade.Value;
        if (req.Status is not null) e.Status = req.Status;

        await db.SaveChangesAsync();
        return await db.Enrollments
            .Include(x => x.Student).Include(x => x.Course)
            .Where(x => x.Id == id)
            .Select(x => new EnrollmentDto(
                x.Id, x.StudentId, x.Student.Name,
                x.CourseId, x.Course.Title,
                x.Grade, x.Status, x.EnrolledAt
            ))
            .FirstOrDefaultAsync();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var e = await db.Enrollments.FindAsync(id);
        if (e is null) return false;
        db.Enrollments.Remove(e);
        await db.SaveChangesAsync();
        return true;
    }
}

public class StatsService(AppDbContext db)
{
    public async Task<DashboardStats> GetStatsAsync()
    {
        var now = DateTime.UtcNow;
        var monthStart = new DateTime(now.Year, now.Month, 1);

        var totalStudents     = await db.Users.CountAsync(u => u.Role == Roles.Student && u.IsActive);
        var totalTeachers     = await db.Users.CountAsync(u => u.Role == Roles.Teacher && u.IsActive);
        var totalCourses      = await db.Courses.CountAsync(c => c.Status == "active");
        var activeEnrollments = await db.Enrollments.CountAsync(e => e.Status == "enrolled");
        var newStudents       = await db.Users.CountAsync(u => u.Role == Roles.Student && u.CreatedAt >= monthStart);

        var avgGrade = await db.Enrollments
            .Where(e => e.Grade.HasValue)
            .AverageAsync(e => (double?)e.Grade) ?? 0;

        var topCourses = await db.Enrollments
     .Where(e => e.Status == "enrolled")
     .Include(e => e.Course)
     .ToListAsync();

        var topCoursesList = topCourses
            .GroupBy(e => e.Course.Title)
            .Select(g => new CourseEnrollmentStat(g.Key, g.Count()))
            .OrderByDescending(x => x.EnrollmentCount)
            .Take(5)
            .ToList();

        var recentActivity = await db.Enrollments
            .Include(e => e.Student)
            .Include(e => e.Course)
            .OrderByDescending(e => e.EnrolledAt)
            .Take(8)
            .Select(e => new RecentActivityItem(
                $"{e.Student.Name} enrolled in {e.Course.Title}",
                e.EnrolledAt,
                "enrollment"
            ))
            .ToListAsync();

        return new DashboardStats(
            totalStudents, totalTeachers, totalCourses,
            activeEnrollments, Math.Round(avgGrade, 1),
            newStudents, topCoursesList, recentActivity
        );
    }
}
