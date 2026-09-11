namespace Todo.Api.Features.IAM.Services;

public sealed record AccessTokenResult(
    string Token,
    DateTime ExpiresAtUtc);