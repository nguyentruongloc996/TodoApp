using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApp.Application.Common.Result
{
    public static class DomainErrors
    {
        public static class User
        {
            public static readonly Error NotFound = Error.NotFound(
                "User.NotFound",
                "The user with the specified identifier was not found");
        }

        public static class Auth
        {
            public static readonly Error InvalidToken = Error.Unauthorized(
                "Auth.InvalidToken",
                "The provided token is invalid");

            public static readonly Error TokenExpired = Error.Unauthorized(
                "Auth.TokenExpired",
                "The provided token has expired");

            public static readonly Error RefreshTokenExpired = Error.Unauthorized(
                "Auth.RefreshTokenExpired",
                "The refresh token has expired");

            public static readonly Error InvalidRefreshToken = Error.Unauthorized(
                "Auth.InvalidRefreshToken",
                "The provided refresh token is invalid");

            public static readonly Error InvalidCredentials = Error.Unauthorized(
                "Auth.InvalidCredentials",
                "The provided credentials are invalid");

            public static readonly Error EmailAlreadyExists = Error.Conflict(
                "Auth.EmailAlreadyExists",
                "The user with the specified email already exists");

            public static readonly Error WeakPassword = Error.Validation(
                "Auth.WeakPassword",
                "The password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, and one number");

            public static readonly Error InvalidEmail = Error.Validation(
                "Auth.InvalidEmail",
                "The email format is invalid");
        }
    }
}
