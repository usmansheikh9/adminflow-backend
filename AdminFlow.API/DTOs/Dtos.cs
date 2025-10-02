namespace AdminFlow.API.DTOs;

// Auth
public record LoginRequest(string Email, string Password);
public record LoginResponse(string Token, UserDto User);

// User
public record UserDto(int Id, string Name, string Email, string Role, bool IsActive, DateTime CreatedAt, DateTime? LastLogin);
public record CreateUserRequest(string Name, string Email, string Password, string Role);
public record UpdateUserRequest(string? Name, string? Role, bool? IsActive);

// Course
public record CourseDto(int Id, string Title, string Description, string Code, int Credits, string Status, int? TeacherId, string? TeacherName, int EnrollmentCount, DateTime CreatedAt);
public record CreateCourseRequest(string Title, string? Description, string Code, int Credits, int? TeacherId);
public record UpdateCourseRequest(string? Title, string? Description, int? Credits, string? Status, int? TeacherId);

// Enrollment
public record EnrollmentDto(int Id, int StudentId, string StudentName, int CourseId, string CourseTitle, decimal? Grade, string Status, DateTime EnrolledAt);
public record CreateEnrollmentRequest(int StudentId, int CourseId);
public record UpdateEnrollmentRequest(decimal? Grade, string? Status);

// Stats
public record DashboardStats(
    int TotalStudents,
    int TotalTeachers,
    int TotalCourses,
    int ActiveEnrollments,
    double AverageGrade,
    int NewStudentsThisMonth,
    List<CourseEnrollmentStat> TopCourses,
    List<RecentActivityItem> RecentActivity
);

public record CourseEnrollmentStat(string CourseTitle, int EnrollmentCount);
public record RecentActivityItem(string Description, DateTime Timestamp, string Type);

// Shared
public record PagedResult<T>(List<T> Data, int Total, int Page, int PageSize);
public record ApiResponse<T>(bool Success, T? Data, string? Message = null);
