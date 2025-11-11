using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApp.Application.Common.Result
{
    /// <summary>
    /// Represents an error that occurred during an operation
    /// </summary>
    public sealed record Error(string Code, string Message, ErrorType Type)
    {
        public static readonly Error None = new(string.Empty, string.Empty, ErrorType.None);
        public static readonly Error NullValue = new("Error.NullValue", "Null value was provided", ErrorType.Failure);

        public static Error NotFound(string code, string message) => new(code, message, ErrorType.NotFound);
        public static Error Validation(string code, string message) => new(code, message, ErrorType.Validation);
        public static Error Conflict(string code, string message) => new(code, message, ErrorType.Conflict);
        public static Error Failure(string code, string message) => new(code, message, ErrorType.Failure);
        public static Error Unauthorized(string code, string message) => new(code, message, ErrorType.Unauthorized);

        public bool IsNotFound => Type == ErrorType.NotFound;
        public bool IsValidation => Type == ErrorType.Validation;
        public bool IsConflict => Type == ErrorType.Conflict;
        public bool IsFailure => Type == ErrorType.Failure;
        public bool IsUnauthorized => Type == ErrorType.Unauthorized;
    }

    /// <summary>
    /// Represents the type of error that occurred
    /// </summary>
    public enum ErrorType
    {
        None = 0,
        Failure = 1,
        Validation = 2,
        NotFound = 3,
        Conflict = 4,
        Unauthorized = 5
    }
}
