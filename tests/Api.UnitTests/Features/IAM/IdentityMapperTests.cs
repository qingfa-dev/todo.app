using Shouldly;

using Todo.Api.Domain.IAM.Constants;
using Todo.Api.Domain.IAM.Entities;
using Todo.Api.Features.IAM;
using Todo.Api.Features.IAM.Authentications;
using Todo.Api.Features.IAM.Services;

namespace Todo.Api.UnitTests.Features.IAM;

public class IdentityMapperTests
{
    #region ToAuthResponse

    [Fact]
    public void ToAuthResponse_ShouldMapAllProperties()
    {
        var accessToken = new AccessTokenResult(
            "test-token",
            DateTime.UtcNow.AddMinutes(15));
        var refreshToken = "refresh-token";

        var result = IdentityMapper.ToAuthResponse<AuthResponse>(
            accessToken,
            refreshToken);

        result.TokenType.ShouldBe(IAMConstant.Token.Bearer);
        result.AccessToken.ShouldBe("test-token");
        result.RefreshToken.ShouldBe("refresh-token");
        result.ExpiresIn.ShouldBeGreaterThan(0);
    }

    [Fact]
    public void ToAuthResponse_WhenNullAccessToken_ShouldThrowArgumentNullException()
    {
        Should.Throw<ArgumentNullException>(() =>
            IdentityMapper.ToAuthResponse<AuthResponse>(
                null!,
                "refresh-token"));
    }

    [Fact]
    public void ToAuthResponse_ShouldCalculateExpiresIn()
    {
        DateTime expiresAt = DateTime.UtcNow.AddSeconds(120);
        var accessToken = new AccessTokenResult("token", expiresAt);
        var refreshToken = "refresh";

        var result = IdentityMapper.ToAuthResponse<AuthResponse>(
            accessToken,
            refreshToken);

        result.ExpiresIn.ShouldBeGreaterThanOrEqualTo(119);
        result.ExpiresIn.ShouldBeLessThanOrEqualTo(121);
    }

    #endregion

    #region ToRegisterResponse

    [Fact]
    public void ToRegisterResponse_ShouldMapAllProperties()
    {
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com"
        };

        var result = IdentityMapper.ToRegisterResponse<RegisterResponse>(
            user);

        result.Id.ShouldBe(user.Id);
        result.Email.ShouldBe("test@example.com");
    }

    [Fact]
    public void ToRegisterResponse_WhenNullUser_ShouldThrowArgumentNullException()
    {
        Should.Throw<ArgumentNullException>(() =>
            IdentityMapper.ToRegisterResponse<RegisterResponse>(null!));
    }

    [Fact]
    public void ToRegisterResponse_WhenEmailIsNull_ShouldReturnEmptyString()
    {
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Email = null
        };

        var result = IdentityMapper.ToRegisterResponse<RegisterResponse>(
            user);

        result.Email.ShouldBe(string.Empty);
    }

    #endregion
}
