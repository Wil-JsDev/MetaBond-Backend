using MetaBond.Application.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MetaBond.Presentation.Api.Filters;

/// <summary>
/// Action filter that automatically handles ResultT<T> responses
/// Converts ResultT<T> to appropriate HTTP responses
/// </summary>
public class ResultTActionFilter(ILogger<ResultTActionFilter> logger) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var executedContext = await next();

        // Process only ObjectResult responses with values
        if (executedContext.Result is ObjectResult objectResult && objectResult.Value != null)
        {
            var valueType = objectResult.Value.GetType();

            // Check if the response is a ResultT<T> type
            if (valueType.IsGenericType && valueType.GetGenericTypeDefinition() == typeof(ResultT<>))
            {
                dynamic resultT = objectResult.Value;
                
                if (!resultT.IsSuccess)
                {
                    // Extract HTTP status code from the error
                    int statusCode = GetStatusCodeFromError(resultT.Error);

                    LoggerExtensions.LogWarning(logger, "Operation failed with code {Code} and message: {Message}",
                        resultT.Error?.Code,
                        resultT.Error?.Description
                    );

                    // Create standardized error response
                    var errorResponse = new
                    {
                        code = resultT.Error?.Code,
                        description = resultT.Error?.Description
                    };

                    // Convert ResultT<T> to proper HTTP error response
                    executedContext.Result = new ObjectResult(errorResponse)
                    {
                        StatusCode = statusCode
                    };
                }
                else
                {
                    // Convert successful ResultT<T> to 200 OK with value
                    executedContext.Result = new OkObjectResult(resultT.Value);
                }
            }
        }
    }

    /// <summary>
    /// Extracts HTTP status code from Error object
    /// Handles both string and numeric error codes safely
    /// </summary>
    private static int GetStatusCodeFromError(Error error)
    {
        // Default to 500 if no error code provided
        if (error?.Code == null)
            return StatusCodes.Status500InternalServerError;

        // Attempt to parse string error code to integer
        if (int.TryParse(error.Code, out int statusCode))
        {
            // Validate that the code is within valid HTTP status code range
            return statusCode >= 100 && statusCode <= 599 ? statusCode : StatusCodes.Status500InternalServerError;
        }
        
        return StatusCodes.Status500InternalServerError;
    }
}