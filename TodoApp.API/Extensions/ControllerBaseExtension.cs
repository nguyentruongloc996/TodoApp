using Microsoft.AspNetCore.Mvc;
using TodoApp.Application.Common.Result;

namespace TodoApp.API.Extensions
{
    public static class ControllerBaseExtension
    {
        /// <summary>
        /// This method converts a Result<TValue> into an appropriate ActionResult.
        /// </summary>
        /// <typeparam name="TValue">Return type</typeparam>
        /// <param name="controller">Current controller</param>
        /// <param name="result">Result object</param>
        /// <returns></returns>
        public static ActionResult FromResult<TValue>(this ControllerBase controller, Result<TValue> result)
        {
            if (result.IsSuccess)
            {
                return controller.Ok(result.Value);
            }

            return result.Error switch
            {
                { IsNotFound: true } => controller.NotFound(result.Error.Message),
                { IsUnauthorized: true } => controller.Unauthorized(result.Error.Message),
                { IsConflict: true } => controller.Conflict(result.Error.Message),
                { IsValidation: true } => controller.ValidationProblem(result.Error.Message),
                _ => controller.StatusCode(500, result.Error)
            };
        }
    }
}
