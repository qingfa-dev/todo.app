using System.Net;

namespace Todo.Api.SharedKernel.Models;

public sealed record Error
{
    public static readonly Error None =
        new(
            string.Empty,
            string.Empty,
            HttpStatusCode.OK);

    public static readonly Error NullValue =
        new(
            "General.Null",
            "Null value was provided.",
            HttpStatusCode.BadRequest);

    public Error(
        string code,
        string description,
        HttpStatusCode statusCode)
    {
        ArgumentNullException.ThrowIfNull(code);
        ArgumentNullException.ThrowIfNull(description);

        if (statusCode != HttpStatusCode.OK &&
            string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException(
                "Error code cannot be empty.",
                nameof(code));
        }

        if (statusCode != HttpStatusCode.OK &&
            string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException(
                "Error description cannot be empty.",
                nameof(description));
        }

        Code = code;
        Description = description;
        StatusCode = statusCode;
    }

    public string Code { get; }

    public string Description { get; }

    public HttpStatusCode StatusCode { get; }

    public int StatusCodeValue => (int)StatusCode;

    public static Error BadRequest(
        string code,
        string description) =>
        new(code, description, HttpStatusCode.BadRequest);

    public static Error Unauthorized(
        string code,
        string description) =>
        new(code, description, HttpStatusCode.Unauthorized);

    public static Error Forbidden(
        string code,
        string description) =>
        new(code, description, HttpStatusCode.Forbidden);

    public static Error NotFound(
        string code,
        string description) =>
        new(code, description, HttpStatusCode.NotFound);

    public static Error Conflict(
        string code,
        string description) =>
        new(code, description, HttpStatusCode.Conflict);

    public static Error UnprocessableEntity(
        string code,
        string description) =>
        new(code, description, HttpStatusCode.UnprocessableEntity);

    public static Error TooManyRequests(
        string code,
        string description) =>
        new(code, description, HttpStatusCode.TooManyRequests);

    public static Error Problem(
        string code,
        string description) =>
        new(code, description, HttpStatusCode.InternalServerError);

    public static Error ServiceUnavailable(
        string code,
        string description) =>
        new(code, description, HttpStatusCode.ServiceUnavailable);
}
