// Interfaces/IAuthService.cs
using DoConnect.API.DTOs;
namespace DoConnect.API.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> RegisterAsync(RegisterDto dto);
        Task<AuthResponseDto?> LoginAsync(LoginDto dto);
    }
}
