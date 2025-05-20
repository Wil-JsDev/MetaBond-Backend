using System.Text.Json.Serialization;

namespace MetaBond.Application.Utils;

/// <summary>
/// Represents the outcome of an operation that can either succeed or fail.
/// </summary>
public class Result
{
    /// <summary>
    /// Gets a value indicating whether the operation was successful.
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// Gets the error information if the operation failed.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Error? Error { get; set; }

    /// <summary>
    /// Initializes a successful result.
    /// </summary>
    protected Result() 
    {
        IsSuccess = true;
        Error = default;
    }

    /// <summary>
    /// Initializes a failed result with the specified error.
    /// </summary>
    /// <param name="error">The error describing the failure.</param>
    protected Result(Error error)
    {
        IsSuccess = false;
        Error = error;
    }

    /// <summary>
    /// Implicitly converts an <see cref="Error"/> to a failed <see cref="Result"/>.
    /// </summary>
    /// <param name="error">The error to convert.</param>
    public static implicit operator Result(Error error) =>
        new(error);

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    /// <returns>A new <see cref="Result"/> indicating success.</returns>
    public static Result Success() =>
        new();

    /// <summary>
    /// Creates a failed result with the specified error.
    /// </summary>
    /// <param name="error">The error describing the failure.</param>
    /// <returns>A new <see cref="Result"/> indicating failure.</returns>
    public static Result Failure(Error error) =>
        new(error);
}


/// <summary>
/// Represents the outcome of an operation that can either succeed with a value of type <typeparamref name="TValue"/> 
/// or fail with an associated error.
/// </summary>
/// <typeparam name="TValue">The type of the value contained in the result when the operation is successful.</typeparam>
public class ResultT<TValue> : Result
{
    /// <summary>
    /// Contains the value of the result in case of success.
    /// </summary>
    private readonly TValue? _value;

    /// <summary>
    /// Initializes a successful result with the specified value.
    /// </summary>
    /// <param name="value">The value associated with the successful result.</param>
    private ResultT(TValue value) : base()
    {
        _value = value;
    }

    /// <summary>
    /// Initializes a failed result with the specified error.
    /// </summary>
    /// <param name="error">The error describing the failure.</param>
    private ResultT(Error error) : base(error)
    {
        _value = default;
    }

    /// <summary>
    /// Gets the value of the result if the operation was successful; otherwise, throws an exception.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the result indicates failure.</exception>
    public TValue Value =>
        IsSuccess ? _value! : throw new InvalidOperationException("Value cannot be accessed when IsSuccess is false");

    /// <summary>
    /// Implicitly converts an <see cref="Error"/> to a failed <see cref="ResultT{TValue}"/>.
    /// </summary>
    /// <param name="error">The error to convert.</param>
    public static implicit operator ResultT<TValue>(Error error) =>
        new(error);

    /// <summary>
    /// Implicitly converts a value of type <typeparamref name="TValue"/> to a successful <see cref="ResultT{TValue}"/>.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    public static implicit operator ResultT<TValue>(TValue value) =>
        new(value);

    /// <summary>
    /// Creates a successful result with the provided value.
    /// </summary>
    /// <param name="value">The value associated with the successful result.</param>
    /// <returns>An instance of <see cref="ResultT{TValue}"/> representing success.</returns>
    public static ResultT<TValue> Success(TValue value) =>
        new(value);

    /// <summary>
    /// Creates a failed result with the provided error.
    /// </summary>
    /// <param name="error">The error associated with the failed result.</param>
    /// <returns>An instance of <see cref="ResultT{TValue}"/> representing failure.</returns>
    public static ResultT<TValue> Failure(Error error) =>
        new(error);
}
