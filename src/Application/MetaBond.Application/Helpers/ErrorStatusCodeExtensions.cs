using Microsoft.AspNetCore.Http;

namespace MetaBond.Application.Helpers;

public static class ErrorExtensions
{
    public static int ToInt(this string code)
    {
        return int.TryParse(code, out var statusCode) ? statusCode : StatusCodes.Status500InternalServerError;
    }
}