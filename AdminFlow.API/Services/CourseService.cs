using AdminFlow.API.Data;
using AdminFlow.API.DTOs;
using AdminFlow.API.Models;
using Microsoft.EntityFrameworkCore;

namespace AdminFlow.API.Services;

public class CourseService(AppDbContext db)
{
    public async Task<PagedResult<CourseDto>> GetCoursesAsync(string? status, string? search, int page, int pageSize)
    {
        var q = db.Courses.Include(c => c.Teacher).Include(c => c.Enrollments).AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
            q = q.Where(c => c.Status == status);

        if (!string.IsNullOrWhiteSpace(search))
            q = q.Where(c => c.Title.Contains(search) || c.Code.Contains(search));

        var total = await q.CountAsync();
        var data = await q
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => c.ToDto())
            .ToListAsync();

        return new PagedResult<CourseDto>(data, total, page, pageSize);
    }

    public async Task<CourseDto?> GetByIdAsync(int id)
    {
        var c = await db.Courses.Include(c => c.Teacher).Include(c => c.Enrollments).FirstOrDefaultAsync(c => c.Id == id);
        return c?.ToDto();
    }

    public async Task<CourseDto> CreateAsync(CreateCourseRequest req)
    {
        var course = new Course
        {
            Title = req.Title,
            Description = req.Description,
            Code = req.Code,
            Credits = req.Credits,
            TeacherId = req.TeacherId,
        };

        db.Courses.Add(course);
        await db.SaveChangesAsync();

        return await GetByIdAsync(course.Id) ?? course.ToDto();
    }

    public async Task<CourseDto?> UpdateAsync(int id, UpdateCourseRequest req)
    {
        var course = await db.Courses.FindAsync(id);
        if (course is null) return null;

        if (req.Title is not null) course.Title = req.Title;
        if (req.Description is not null) course.Description = req.Description;
        if (req.Credits.HasValue) course.Credits = req.Credits.Value;
        if (req.Status is not null) course.Status = req.Status;
        if (req.TeacherId.HasValue) course.TeacherId = req.TeacherId.Value;

        await db.SaveChangesAsync();
        return await GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var course = await db.Courses.FindAsync(id);
        if (course is null) return false;
        db.Courses.Remove(course);
        await db.SaveChangesAsync();
        return true;
    }
}

public static class CourseExtensions
{
    public static CourseDto ToDto(this Course c) => new(
        c.Id, c.Title, c.Description ?? string.Empty, c.Code,
        c.Credits, c.Status, c.TeacherId, c.Teacher?.Name,
        c.Enrollments.Count, c.CreatedAt
    );
}
