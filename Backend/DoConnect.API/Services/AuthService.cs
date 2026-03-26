// Services/AuthService.cs — Sprint 2
// Change: AuthResponseDto now includes ExpiresAt so Angular
//         can show a session expiry warning before it happens
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
        private readonly IConfiguration _config;

        public AuthService(DoConnectDbContext context, JwtHelper jwtHelper, IConfiguration config)
        {
            _context   = context;
            _jwtHelper = jwtHelper;
            _config    = config;
        }

        public async Task<AuthResponseDto?> RegisterAsync(RegisterDto dto)
        {
            var emailExists = await _context.Users
                .AnyAsync(u => u.Email.ToLower() == dto.Email.ToLower());
            if (emailExists) return null;

            var usernameExists = await _context.Users
                .AnyAsync(u => u.Username.ToLower() == dto.Username.ToLower());
            if (usernameExists) return null;

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            var newUser = new User
            {
                Username     = dto.Username.Trim(),
                Email        = dto.Email.Trim().ToLower(),
                PasswordHash = passwordHash,
                Role         = "User",  // Always User — never from dto
                CreatedAt    = DateTime.UtcNow
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            var token     = _jwtHelper.GenerateToken(newUser);
            var expiresAt = DateTime.UtcNow.AddHours(
                int.Parse(_config["JwtSettings:ExpiresInHours"] ?? "24"));

            return new AuthResponseDto
            {
                Token     = token,
                Username  = newUser.Username,
                Role      = newUser.Role,
                UserId    = newUser.UserId,
                ExpiresAt = expiresAt  // Sprint 2: tell frontend when session ends
            };
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == dto.Email.ToLower());
            if (user == null) return null;

            var valid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
            if (!valid) return null;

            var token     = _jwtHelper.GenerateToken(user);
            var expiresAt = DateTime.UtcNow.AddHours(
                int.Parse(_config["JwtSettings:ExpiresInHours"] ?? "24"));

            return new AuthResponseDto
            {
                Token     = token,
                Username  = user.Username,
                Role      = user.Role,
                UserId    = user.UserId,
                ExpiresAt = expiresAt
            };
        }
    }
}
