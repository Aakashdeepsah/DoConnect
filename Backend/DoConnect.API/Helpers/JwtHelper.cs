// Helpers/JwtHelper.cs
using DoConnect.API.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DoConnect.API.Helpers
{
    public class JwtHelper
    {
        private readonly IConfiguration _config;
        public JwtHelper(IConfiguration config) { _config = config; }

        public string GenerateToken(User user)
        {
            var secret = _config["JwtSettings:Secret"]
                ?? throw new InvalidOperationException("JWT Secret not configured.");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = int.Parse(_config["JwtSettings:ExpiresInHours"] ?? "24");

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _config["JwtSettings:Issuer"],
                audience: _config["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(expires),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
