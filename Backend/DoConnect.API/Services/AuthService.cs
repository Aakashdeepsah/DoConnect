// Services/AuthService.cs
// FIX 1: Registration ALWAYS creates a User, never an Admin.
//         Admin accounts are only created via database seed.
using DoConnect.API.Data;
using DoConnect.API.DTOs;
using DoConnect.API.Helpers;
using DoConnect.API.Interfaces;
using DoConnect.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DoConnect.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly DoConnectDbContext _context;
        private readonly JwtHelper _jwtHelper;

        public AuthService(DoConnectDbContext context, JwtHelper jwtHelper)
        {
            _context = context;
            _jwtHelper = jwtHelper;
        }

        public async Task<AuthResponseDto?> RegisterAsync(RegisterDto dto)
        {
            // Check for duplicate email
            var emailExists = await _context.Users
                .AnyAsync(u => u.Email.ToLower() == dto.Email.ToLower());
            if (emailExists) return null;

            // Check for duplicate username
            var usernameExists = await _context.Users
                .AnyAsync(u => u.Username.ToLower() == dto.Username.ToLower());
            if (usernameExists) return null;

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var newUser = new User
            {
                Username     = dto.Username.Trim(),
                Email        = dto.Email.Trim().ToLower(),
                PasswordHash = passwordHash,
                // FIX: ALWAYS "User" — the dto.Role value is IGNORED entirely.
                // Admins can only be created directly in the database.
                Role      = "User",
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            var token = _jwtHelper.GenerateToken(newUser);
            return new AuthResponseDto
            {
                Token    = token,
                Username = newUser.Username,
                Role     = newUser.Role,
                UserId   = newUser.UserId
            };
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == dto.Email.ToLower());

            if (user == null) return null;

            var valid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
            if (!valid) return null;

            var token = _jwtHelper.GenerateToken(user);
            return new AuthResponseDto
            {
                Token    = token,
                Username = user.Username,
                Role     = user.Role,
                UserId   = user.UserId
            };
        }
    }
}
