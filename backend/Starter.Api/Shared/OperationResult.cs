namespace Starter.Api.Shared;

/// <summary>What kind of failure happened, so the controller can pick the right HTTP status.</summary>
public enum ErrorKind
{
    None,
    Validation,
    NotFound,
}

/// <summary>
/// The result of a service operation. Expected failures (bad input, missing item) come back as a
/// result instead of an exception, so the controller decides the HTTP status in one place.
/// </summary>
public sealed record OperationResult<T>(bool Success, T? Data, IReadOnlyList<string> Errors, ErrorKind Kind)
{
    public static OperationResult<T> Ok(T data) => new(true, data, [], ErrorKind.None);

    public static OperationResult<T> Invalid(params string[] errors) => new(false, default, errors, ErrorKind.Validation);

    public static OperationResult<T> NotFound(string error) => new(false, default, [error], ErrorKind.NotFound);
}
