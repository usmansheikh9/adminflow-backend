using AdminFlow.API.DTOs;
using AdminFlow.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(AuthService auth) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        var result = await auth.LoginAsync(req.Email, req.Password);
        if (result is null)
            return Unauthorized(new ApiResponse<object>(false, null, "Invalid credentials"));

        return Ok(new ApiResponse<LoginResponse>(true, result));
    }

    [HttpGet("me"), Authorize]
    public IActionResult Me() => Ok(new ApiResponse<object>(true, new
    {
        id    = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value,
        email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value,
        role  = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value,
    }));
}

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController(UserService users) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? role, [FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await users.GetUsersAsync(role, search, page, pageSize);
        return Ok(new ApiResponse<PagedResult<UserDto>>(true, result));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await users.GetByIdAsync(id);
        return user is null ? NotFound() : Ok(new ApiResponse<UserDto>(true, user));
    }

    [HttpPost, Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest req)
    {
        var user = await users.CreateAsync(req);
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, new ApiResponse<UserDto>(true, user));
    }

    [HttpPatch("{id}"), Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserRequest req)
    {
        var user = await users.UpdateAsync(id, req);
        return user is null ? NotFound() : Ok(new ApiResponse<UserDto>(true, user));
    }

    [HttpDelete("{id}"), Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await users.DeleteAsync(id);
        return success ? Ok(new ApiResponse<object>(true, null, "User deleted")) : NotFound();
    }
}

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CoursesController(CourseService courses) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? status, [FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await courses.GetCoursesAsync(status, search, page, pageSize);
        return Ok(new ApiResponse<PagedResult<CourseDto>>(true, result));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var course = await courses.GetByIdAsync(id);
        return course is null ? NotFound() : Ok(new ApiResponse<CourseDto>(true, course));
    }

    [HttpPost, Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateCourseRequest req)
    {
        var course = await courses.CreateAsync(req);
        return CreatedAtAction(nameof(GetById), new { id = course.Id }, new ApiResponse<CourseDto>(true, course));
    }

    [HttpPatch("{id}"), Authorize(Roles = "Admin,Teacher")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCourseRequest req)
    {
        var course = await courses.UpdateAsync(id, req);
        return course is null ? NotFound() : Ok(new ApiResponse<CourseDto>(true, course));
    }

    [HttpDelete("{id}"), Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await courses.DeleteAsync(id);
        return success ? Ok(new ApiResponse<object>(true, null, "Course deleted")) : NotFound();
    }
}

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EnrollmentsController(EnrollmentService enrollments) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? courseId, [FromQuery] int? studentId, [FromQuery] string? status, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await enrollments.GetAsync(courseId, studentId, status, page, pageSize);
        return Ok(new ApiResponse<PagedResult<EnrollmentDto>>(true, result));
    }

    [HttpPost, Authorize(Roles = "Admin,Teacher")]
    public async Task<IActionResult> Create([FromBody] CreateEnrollmentRequest req)
    {
        var enrollment = await enrollments.CreateAsync(req);
        return Ok(new ApiResponse<EnrollmentDto>(true, enrollment));
    }

    [HttpPatch("{id}"), Authorize(Roles = "Admin,Teacher")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateEnrollmentRequest req)
    {
        var result = await enrollments.UpdateAsync(id, req);
        return result is null ? NotFound() : Ok(new ApiResponse<EnrollmentDto>(true, result));
    }

    [HttpDelete("{id}"), Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await enrollments.DeleteAsync(id);
        return success ? Ok(new ApiResponse<object>(true, null, "Enrollment removed")) : NotFound();
    }
}

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StatsController(StatsService stats) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var result = await stats.GetStatsAsync();
        return Ok(new ApiResponse<DashboardStats>(true, result));
    }
}
