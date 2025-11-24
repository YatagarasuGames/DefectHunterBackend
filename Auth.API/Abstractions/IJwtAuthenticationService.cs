using Auth.API.Contracts;
using Shared.Models;

namespace Auth.API.Abstractions
{
    public interface IJwtAuthenticationService
    {
        Task<LoginResponse> GenerateJwtToken(Guid userId);
        Task<string> GenerateRefreshToken(Guid userId);
        Task<LoginResponse?> ValidateRefreshToken(string token);
    }
}