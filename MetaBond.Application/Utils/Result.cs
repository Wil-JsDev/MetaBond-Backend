using System.Text.Json.Serialization;

namespace MetaBond.Application.Utils
{
    public class Result
    {
        public bool IsSuccess { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Error? Error { get; set; }

        protected Result() 
        {
            IsSuccess = true;
            Error = default;
        }

        protected Result(Error error)
        {
            IsSuccess = false;
            Error = error;
        }

        public static implicit operator Result(Error error) =>
           new(error);

        public static Result Success() =>
            new();

        public static Result Failure(Error error) =>
            new(error);

    }

    /// <summary>
    /// Represents the outcome of an operation that can either succeed with a value of type 
    /// or fail with an associated error. Provides utility methods for implicit conversion and result handling.
    /// The type of the value contained in the result when the operation is successful.
    /// </summary>
    public class ResultT<TValue> : Result
    {
        /// <summary>
        /// Contiene el valor del resultado en caso de éxito.
        /// </summary>
        private readonly TValue? _value;

        private ResultT(TValue value) : base()
        {
            _value = value;
        }

        private ResultT(Error error) : base(error)
        {
            _value = default;
        }

        public TValue Value =>
            IsSuccess ? _value! : throw new InvalidOperationException("Value can not be accessed when IsSuccess is false");

        public static implicit operator ResultT<TValue>(Error error) =>
            new(error);

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

}
