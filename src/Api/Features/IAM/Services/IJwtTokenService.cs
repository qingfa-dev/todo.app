using Todo.Api.Domain.IAM.Entities;

namespace Todo.Api.Features.IAM.Services;

public interface IJwtTokenService
{
    AccessTokenResult CreateAccessToken(
        ApplicationUser user,
        IEnumerable<string> roles);

    string CreateRefreshToken();

    string HashRefreshToken(
        string refreshToken);
}