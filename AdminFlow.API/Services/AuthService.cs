using AdminFlow.API.Data;
using AdminFlow.API.DTOs;
using AdminFlow.API.Middleware;
using AdminFlow.API.Models;
using Microsoft.EntityFrameworkCore;

namespace AdminFlow.API.Services;

public class AuthService(AppDbContext db, IConfiguration config)
{
    public async Task<LoginResponse?> LoginAsync(string email, string password)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email.ToLower() && u.IsActive);
        if (user is null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return null;

        user.LastLogin = DateTime.UtcNow;
        await db.SaveChangesAsync();

        var token = JwtHelper.GenerateToken(user.Id, user.Email, user.Role, config);
        return new LoginResponse(token, user.ToDto());
    }
}

public static class UserExtensions
{
    public static UserDto ToDto(this User u) =>
        new(u.Id, u.Name, u.Email, u.Role, u.IsActive, u.CreatedAt, u.LastLogin);
}
