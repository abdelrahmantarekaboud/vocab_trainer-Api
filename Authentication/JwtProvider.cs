using System.Security.Claims;
using VocabTrainer.Api.Entities;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using VocabTrainer.Api.Abstractions.Consts;

namespace VocabTrainer.Api.Authentication
{
    public class JwtProvider : IJwtProvider
    {
        private readonly JwtOptions _options;

        public JwtProvider(IOptions<JwtOptions> opt) => _options = opt.Value;

        public string GenerateAccessToken(User user)
        {
            var claims = new List<Claim>
            {
                new(AppClaims.UserId, user.Id.ToString()),
                new(AppClaims.Role, user.Role.ToString()),
                new(AppClaims.Email, user.Email),
                new(AppClaims.EmailConfirmed, user.EmailConfirmed.ToString().ToLower())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_options.ExpiryMinutes),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public RefreshToken GenerateRefreshToken(User user)
        {
            return new RefreshToken
            {
                UserId = user.Id,
                Token = Guid.NewGuid().ToString("N"),
                ExpiresAt = DateTime.UtcNow.AddDays(_options.RefreshExpiryDays)
            };
        }
    }
}