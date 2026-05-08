using AdminFlow.API.Models;
using BCrypt.Net;

namespace AdminFlow.API.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (db.Users.Any()) return;

        var admin = new User
        {
            Name = "Usman Sheikh",
            Email = "admin@adminflow.io",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
            Role = Roles.Admin,
        };

        var teachers = new[]
        {
            new User { Name = "Sarah Johnson",  Email = "sarah@adminflow.io",  PasswordHash = BCrypt.Net.BCrypt.HashPassword("teacher123"), Role = Roles.Teacher },
            new User { Name = "Mike Chen",       Email = "mike@adminflow.io",   PasswordHash = BCrypt.Net.BCrypt.HashPassword("teacher123"), Role = Roles.Teacher },
            new User { Name = "Emma Davis",      Email = "emma@adminflow.io",   PasswordHash = BCrypt.Net.BCrypt.HashPassword("teacher123"), Role = Roles.Teacher },
        };

        var students = new[]
        {
            new User { Name = "James Wilson",    Email = "james@student.io",    PasswordHash = BCrypt.Net.BCrypt.HashPassword("student123"), Role = Roles.Student },
            new User { Name = "Lisa Wang",       Email = "lisa@student.io",     PasswordHash = BCrypt.Net.BCrypt.HashPassword("student123"), Role = Roles.Student },
            new User { Name = "Tom Anderson",    Email = "tom@student.io",      PasswordHash = BCrypt.Net.BCrypt.HashPassword("student123"), Role = Roles.Student },
            new User { Name = "Anna Martinez",   Email = "anna@student.io",     PasswordHash = BCrypt.Net.BCrypt.HashPassword("student123"), Role = Roles.Student },
            new User { Name = "David Lee",       Email = "david@student.io",    PasswordHash = BCrypt.Net.BCrypt.HashPassword("student123"), Role = Roles.Student },
            new User { Name = "Rachel Kim",      Email = "rachel@student.io",   PasswordHash = BCrypt.Net.BCrypt.HashPassword("student123"), Role = Roles.Student },
            new User { Name = "Chris Brown",     Email = "chris@student.io",    PasswordHash = BCrypt.Net.BCrypt.HashPassword("student123"), Role = Roles.Student },
            new User { Name = "Priya Patel",     Email = "priya@student.io",    PasswordHash = BCrypt.Net.BCrypt.HashPassword("student123"), Role = Roles.Student },
        };

        db.Users.Add(admin);
        db.Users.AddRange(teachers);
        db.Users.AddRange(students);
        await db.SaveChangesAsync();

        var courses = new[]
        {
            new Course { Title = "Web Development Fundamentals", Code = "WD101", Credits = 3, Description = "HTML, CSS, JavaScript fundamentals for beginners.", TeacherId = teachers[0].Id },
            new Course { Title = "React & Modern Frontend",       Code = "RF201", Credits = 4, Description = "Component architecture, hooks, state management.",  TeacherId = teachers[0].Id },
            new Course { Title = "ASP.NET Core Backend",          Code = "AN301", Credits = 4, Description = "REST APIs, EF Core, SQL Server, auth.",            TeacherId = teachers[1].Id },
            new Course { Title = "Database Design",               Code = "DB201", Credits = 3, Description = "Normalization, indexing, SQL querying.",            TeacherId = teachers[1].Id },
            new Course { Title = "Cybersecurity Fundamentals",    Code = "CS101", Credits = 3, Description = "Network security, pentesting basics, OWASP.",       TeacherId = teachers[2].Id },
            new Course { Title = "Software Engineering",          Code = "SE401", Credits = 4, Description = "SDLC, Agile, system design, code review.",          TeacherId = teachers[2].Id, Status = "archived" },
        };

        db.Courses.AddRange(courses);
        await db.SaveChangesAsync();

        var rng = new Random(42);
        var enrollments = new List<Enrollment>();
        var statuses = new[] { "enrolled", "enrolled", "enrolled", "completed", "dropped" };

        foreach (var student in students)
        {
            var courseCount = rng.Next(2, 5);
            var picked = courses.OrderBy(_ => rng.Next()).Take(courseCount).ToList();
            foreach (var course in picked)
            {
                var status = statuses[rng.Next(statuses.Length)];
                enrollments.Add(new Enrollment
                {
                    StudentId = student.Id,
                    CourseId = course.Id,
                    Status = status,
                    Grade = status == "completed" ? Math.Round((decimal)(rng.NextDouble() * 40 + 60), 2) : null,
                    EnrolledAt = DateTime.UtcNow.AddDays(-rng.Next(10, 120)),
                });
            }
        }

        db.Enrollments.AddRange(enrollments);
        await db.SaveChangesAsync();
    }
}
