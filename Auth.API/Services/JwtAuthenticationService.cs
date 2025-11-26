using Auth.API.Abstractions;
using Auth.API.Contracts;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.IdentityModel.Tokens;
using Shared.Database.Abstractions;
using Shared.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Auth.API.Services
{
    public class JwtAuthenticationService : IJwtAuthenticationService
    {
        private readonly IConfiguration _configuration;
        private readonly IUsersRepository _usersRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public JwtAuthenticationService(IConfiguration configuraton, IUsersRepository usersRepository, IRefreshTokenRepository refreshTokenRepository)
        {
            _configuration = configuraton;
            _usersRepository = usersRepository;
            _refreshTokenRepository = refreshTokenRepository;
        }


        public async Task<LoginResponse> GenerateJwtToken(Guid userId)
        {
            var user = await _usersRepository.GetUserByIdAsync(userId);
            var issuer = _configuration["JwtConfig:Issuer"];
            var audience = _configuration["JwtConfig:Audience"];
            var key = Encoding.UTF8.GetBytes(_configuration["JwtConfig:Key"]!);
            var tokenValidityMins = _configuration.GetValue<int>("JwtConfig:TokenValidInMins");

            var tokenExpiryTimestamp = DateTime.UtcNow.AddMinutes(tokenValidityMins);

            var token = new JwtSecurityToken(
                issuer,
                audience,
                [
                    new Claim(JwtRegisteredClaimNames.NameId, userId.ToString()),
                    new Claim(JwtRegisteredClaimNames.Nickname, user != null ? user.Username : "Unnamed")
                ],
                expires: tokenExpiryTimestamp,
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha512Signature)
            );

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

            return new LoginResponse(
                accessToken: accessToken,
                expiresIn: (int)tokenExpiryTimestamp.Subtract(DateTime.UtcNow).TotalSeconds,
                refreshToken: await GenerateRefreshToken(userId)

                );
        }

        public async Task<string> GenerateRefreshToken(Guid userId)
        {
            var refreshTokenValidityMins = _configuration.GetValue<int>("JwtConfig:TokenValidInMins");

            var result = RefreshToken.Create(
                id: Guid.NewGuid(),
                token: Guid.NewGuid().ToString(),
                expireTime: DateTime.UtcNow.AddMinutes(refreshTokenValidityMins),
                userId: userId
                );
            if (!result.IsSuccess) return null;

            var refreshToken = result.Value;
            await _refreshTokenRepository.Create(refreshToken);
            return refreshToken.Token;
        }

        public async Task<LoginResponse?> ValidateRefreshToken(string token)
        {
            var refreshToken = await _refreshTokenRepository.GetTokenByToken(token);

            if (refreshToken is null || refreshToken.ExpireTime < DateTime.UtcNow)
            {
                return null;
            }

            await _refreshTokenRepository.DeleteById(refreshToken.Id);

            var user = await _usersRepository.GetUserByIdAsync(refreshToken.Id);
            if (user is null) return null;
            return await GenerateJwtToken(user.Id);
        }
    }
}
