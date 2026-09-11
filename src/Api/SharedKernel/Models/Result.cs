using System.Diagnostics.CodeAnalysis;

namespace Todo.Api.SharedKernel.Models;

public class Result
{
    protected Result(
        bool isSuccess,
        params Error[]? errors)
    {
        List<Error> errorList = errors?.ToList() ?? [];

        if (isSuccess &&
            errorList.Any(error => error != Error.None))
        {
            throw new ArgumentException(
                "A successful result cannot contain errors.",
                nameof(errors));
        }

        if (!isSuccess &&
            errorList.Count == 0)
        {
            throw new ArgumentException(
                "A failed result must contain at least one error.",
                nameof(errors));
        }

        IsSuccess = isSuccess;
        Errors = errorList;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public List<Error> Errors { get; }

    public Error FirstError =>
        Errors.FirstOrDefault() ?? Error.None;

    public static Result Success() =>
        new(true, Error.None);

    public static Result Failure(Error error) =>
        new(false, error);

    public static Result Failure(
        params Error[] errors) =>
        new(false, errors);

    public static implicit operator Result(Error error) =>
        Failure(error);
}


public class Result<TValue> : Result
{
    private readonly TValue? _value;

    protected Result(
        TValue? value,
        bool isSuccess,
        params Error[]? errors)
        : base(isSuccess, errors)
    {
        _value = value;
    }

    [NotNull]
    public TValue Value =>
        IsSuccess
            ? _value!
            : throw new InvalidOperationException(
                "The value of a failure result can't be accessed.");

    public static Result<TValue> Success(TValue value) =>
        new(
            value,
            true,
            Error.None);

    public new static Result<TValue> Failure(
        Error error) =>
        new(
            default,
            false,
            error);

    public new static Result<TValue> Failure(
        params Error[] errors) =>
        new(
            default,
            false,
            errors);

    public static Result<TValue> ValidationFailure(
        Error error) =>
        new(
            default,
            false,
            error);

    public static implicit operator Result<TValue>(
        TValue? value) =>
        value is not null
            ? Success(value)
            : Failure(Error.NullValue);

    public static implicit operator Result<TValue>(
        Error error) =>
        Failure(error);

    public static implicit operator Result<TValue>(
        Error[] errors) =>
        Failure(errors);

    public static implicit operator Result<TValue>(
        List<Error> errors) =>
        Failure(errors.ToArray());
}
