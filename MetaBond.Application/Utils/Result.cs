using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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

    public class ResultT<TValue> : Result
    {
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

        public static ResultT<TValue> Success(TValue value) => 
            new(value);

        public static ResultT<TValue> Failure(Error error) =>
            new(error);
    }

}
