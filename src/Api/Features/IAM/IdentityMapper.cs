using Todo.Api.Domain.IAM.Constants;
using Todo.Api.Domain.IAM.Entities;
using Todo.Api.Features.IAM.Authentications;
using Todo.Api.Features.IAM.Services;

namespace Todo.Api.Features.IAM;

public static class IdentityMapper
{
    public static T ToAuthResponse<T>(
        AccessTokenResult accessToken,
        string refreshToken) where T : AuthResponse, new()
    {
        ArgumentNullException.ThrowIfNull(accessToken);

        return new T
        {
            TokenType = IAMConstant.Token.Bearer,
            AccessToken = accessToken.Token,
            ExpiresIn = Math.Max(
                0,
                (int)(
                    accessToken.ExpiresAtUtc -
                    DateTime.UtcNow)
                    .TotalSeconds),
            RefreshToken = refreshToken
        };
    }

    public static T ToRegisterResponse<T>(
        ApplicationUser user) where T : RegisterResponse, new()
    {
        ArgumentNullException.ThrowIfNull(user);

        return new T
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty
        };
    }
}