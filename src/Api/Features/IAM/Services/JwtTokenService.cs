using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using Todo.Api.Domain.IAM.Entities;

namespace Todo.Api.Features.IAM.Services;

public sealed class JwtTokenService : IJwtTokenService
{
    private readonly JwtOptions _options;

    public JwtTokenService(
        IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    public AccessTokenResult CreateAccessToken(
        ApplicationUser user,
        IEnumerable<string> roles)
    {
        var now = DateTime.UtcNow;

        var expiresAtUtc =
            now.AddMinutes(_options.AccessTokenMinutes);

        var claims = new List<Claim>
        {
            new(
                JwtRegisteredClaimNames.Sub,
                user.Id.ToString()),

            new(
                JwtRegisteredClaimNames.Email,
                user.Email ?? string.Empty),

            new(
                ClaimTypes.NameIdentifier,
                user.Id.ToString()),

            new(
                ClaimTypes.Name,
                user.UserName ??
                user.Email ??
                string.Empty),

            new(
                JwtRegisteredClaimNames.Jti,
                Guid.NewGuid().ToString())
        };

        claims.AddRange(
            roles.Select(role =>
                new Claim(
                    ClaimTypes.Role,
                    role)));

        var key =
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    _options.SecretKey));

        var credentials =
            new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

        var token =
            new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                notBefore: now,
                expires: expiresAtUtc,
                signingCredentials: credentials);

        var accessToken =
            new JwtSecurityTokenHandler()
                .WriteToken(token);

        return new AccessTokenResult(
            accessToken,
            expiresAtUtc);
    }

    public string CreateRefreshToken()
    {
        var bytes =
            RandomNumberGenerator.GetBytes(64);

        return Convert.ToBase64String(bytes);
    }

    public string HashRefreshToken(
        string refreshToken)
    {
        var hash =
            SHA256.HashData(
                Encoding.UTF8.GetBytes(
                    refreshToken));

        return Convert.ToHexString(hash);
    }
}