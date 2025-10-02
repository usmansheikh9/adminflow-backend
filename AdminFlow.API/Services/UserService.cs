using AdminFlow.API.Data;
using AdminFlow.API.DTOs;
using AdminFlow.API.Models;
using Microsoft.EntityFrameworkCore;

namespace AdminFlow.API.Services;

public class UserService(AppDbContext db)
{
    public async Task<PagedResult<UserDto>> GetUsersAsync(string? role, string? search, int page, int pageSize)
    {
        var q = db.Users.AsQueryable();

        if (!string.IsNullOrWhiteSpace(role))
            q = q.Where(u => u.Role == role);

        if (!string.IsNullOrWhiteSpace(search))
            q = q.Where(u => u.Name.Contains(search) || u.Email.Contains(search));

        var total = await q.CountAsync();
        var data = await q
            .OrderByDescending(u => u.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(u => u.ToDto())
            .ToListAsync();

        return new PagedResult<UserDto>(data, total, page, pageSize);
    }

    public async Task<UserDto?> GetByIdAsync(int id) =>
        (await db.Users.FindAsync(id))?.ToDto();

    public async Task<UserDto> CreateAsync(CreateUserRequest req)
    {
        var user = new User
        {
            Name = req.Name,
            Email = req.Email.ToLower(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password),
            Role = req.Role,
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();
        return user.ToDto();
    }

    public async Task<UserDto?> UpdateAsync(int id, UpdateUserRequest req)
    {
        var user = await db.Users.FindAsync(id);
        if (user is null) return null;

        if (req.Name is not null) user.Name = req.Name;
        if (req.Role is not null) user.Role = req.Role;
        if (req.IsActive.HasValue) user.IsActive = req.IsActive.Value;

        await db.SaveChangesAsync();
        return user.ToDto();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var user = await db.Users.FindAsync(id);
        if (user is null) return false;
        db.Users.Remove(user);
        await db.SaveChangesAsync();
        return true;
    }
}
