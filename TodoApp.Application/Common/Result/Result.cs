namespace TodoApp.Application.Common.Result
{
    public class Result
    {
        protected Result(bool isSuccess, Error error)
        {
            if (isSuccess && error != Error.None ||
                !isSuccess && error == Error.None)
            {
                throw new ArgumentException("Invalid error", nameof(error));
            }

            IsSuccess = isSuccess;
            Error = error;
        }

        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public Error Error { get; }

        public static Result Success() => new(true, Error.None);
        public static Result Failure(Error error) => new(false, error);

        public static implicit operator Result(Error error) => Failure(error);

        public static Result<TValue> Success<TValue>(TValue value) => new(value, true, Error.None);
        public static Result<TValue> Failure<TValue>(Error error) => new(default, false, error);

        public static Result<TValue> Create<TValue>(TValue? value) where TValue : class
            => value is not null ? Success(value) : Failure<TValue>(Error.NullValue);
    }

    /// <summary>
    /// Represents the result of an operation with a return value of type TValue
    /// </summary>
    /// <typeparam name="TValue">The type of the return value</typeparam>
    public class Result<TValue> : Result
    {
        private readonly TValue? _value;

        protected internal Result(TValue? value, bool isSuccess, Error error)
            : base(isSuccess, error)
        {
            _value = value;
        }

        public TValue Value => IsSuccess
            ? _value!
            : throw new InvalidOperationException("The value of a failure result can't be accessed.");

        public static implicit operator Result<TValue>(TValue value)
            => new(value, true, Error.None);

        public static implicit operator Result<TValue>(Error error) => Failure<TValue>(error);

        public static Result<TValue> ValidationFailure(Error error) => new(default, false, error);
    }
}
