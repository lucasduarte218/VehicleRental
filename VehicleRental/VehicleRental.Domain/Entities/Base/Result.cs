using VehicleRental.Domain.Enums;

namespace VehicleRental.Domain.Entities.Base;

public class Result
{
    public bool IsSuccess { get; }
    public string? ErrorMessage { get; }
    public ResultErrorType? ErrorType { get; }

    protected Result(bool isSuccess, string? errorMessage, ResultErrorType? errorType)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
        ErrorType = errorType;
    }

    public static Result Success() => new(true, null, null);
    public static Result Failure(string errorMessage, ResultErrorType errorType) => new(false, errorMessage, errorType);
}

public class Result<T> : Result
{
    public T? Data { get; }

    private Result(T? data, bool isSuccess, string? errorMessage, ResultErrorType? errorType)
        : base(isSuccess, errorMessage, errorType)
    {
        Data = data;
    }

    public static Result<T> Success(T data) => new(data, true, null, null);
    public static new Result<T> Failure(string errorMessage, ResultErrorType errorType) => new(default, false, errorMessage, errorType);
}
