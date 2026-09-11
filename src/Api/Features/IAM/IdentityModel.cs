namespace Todo.Api.Features.IAM.Authentications;

// Request:
public record EmailPasswordParameter
{
    public string Email { get; init; } = string.Empty;

    public string Password { get; init; } = string.Empty;
}
public record AuthResponse
{
    public string TokenType { get; set; } = default!;
    public string AccessToken { get; set; } = default!;
    public int ExpiresIn { get; set; }
    public string RefreshToken { get; set; } = default!;
}

public record RegisterResponse
{
    public Guid Id { get; init; }

    public string Email { get; init; } = string.Empty;
}