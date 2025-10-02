using Microsoft.EntityFrameworkCore;
using AdminFlow.API.Models;

namespace AdminFlow.API.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<User>(e =>
        {
            e.HasIndex(u => u.Email).IsUnique();
            e.Property(u => u.Role).HasMaxLength(20);
        });

        b.Entity<Course>(e =>
        {
            e.HasIndex(c => c.Code).IsUnique();
            e.HasOne(c => c.Teacher)
             .WithMany()
             .HasForeignKey(c => c.TeacherId)
             .OnDelete(DeleteBehavior.SetNull);
        });

        b.Entity<Enrollment>(e =>
        {
            e.HasIndex(en => new { en.StudentId, en.CourseId }).IsUnique();
            e.HasOne(en => en.Student).WithMany(u => u.Enrollments).HasForeignKey(en => en.StudentId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(en => en.Course).WithMany(c => c.Enrollments).HasForeignKey(en => en.CourseId).OnDelete(DeleteBehavior.Cascade);
            e.Property(en => en.Grade).HasPrecision(5, 2);
        });
    }
}
