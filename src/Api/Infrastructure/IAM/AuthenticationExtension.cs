using System.Text;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

using Todo.Api.Features.IAM.Services;

namespace Todo.Api.Infrastructure.IAM;

public static class TokenExtension
{
    public static WebApplicationBuilder AddApplicationAuthentication(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddOptions<JwtOptions>()
            .BindConfiguration(JwtOptions.SectionName)
            .Validate(
                options =>
                    !string.IsNullOrWhiteSpace(options.Issuer),
                "JWT Issuer is required.")
            .Validate(
                options =>
                    !string.IsNullOrWhiteSpace(options.Audience),
                "JWT Audience is required.")
            .Validate(
                options =>
                    !string.IsNullOrWhiteSpace(options.SecretKey),
                "JWT SecretKey is required.")
            .Validate(
                options =>
                    options.SecretKey.Length >= 32,
                "JWT SecretKey must contain at least 32 characters.")
            .Validate(
                options =>
                    options.AccessTokenMinutes > 0,
                "JWT AccessTokenMinutes must be greater than zero.")
            .Validate(
                options =>
                    options.RefreshTokenDays > 0,
                "JWT RefreshTokenDays must be greater than zero.")
            .ValidateOnStart();

        JwtOptions jwtOptions =
            builder.Configuration
                .GetSection(JwtOptions.SectionName)
                .Get<JwtOptions>()
            ?? throw new InvalidOperationException(
                "JWT configuration is missing.");

        var signingKey =
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    jwtOptions.SecretKey));

        builder.Services
            .AddAuthentication(
                JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,

                        IssuerSigningKey = signingKey,

                        ValidateIssuer = true,

                        ValidIssuer = jwtOptions.Issuer,

                        ValidateAudience = true,

                        ValidAudience = jwtOptions.Audience,

                        ValidateLifetime = true,

                        ClockSkew =
                            TimeSpan.FromSeconds(30),

                        NameClaimType =
                            System.Security.Claims.ClaimTypes.Name,

                        RoleClaimType =
                            System.Security.Claims.ClaimTypes.Role
                    };
            });


        builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

        return builder;
    }
}