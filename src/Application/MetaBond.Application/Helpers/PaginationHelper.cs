using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Helpers;

public static class PaginationHelper
{
    public static ResultT<PagedResult<T>> ValidatePagination<T>(int pageNumber, int pageSize, ILogger logger)
    {
        if (pageNumber <= 0 || pageSize <= 0)
        {
            logger.LogWarning("Invalid pagination parameters: PageNumber={PageNumber}, PageSize={PageSize}.",
                pageNumber, pageSize);

            return ResultT<PagedResult<T>>.Failure(
                Error.Failure("400",
                    "Invalid pagination parameters. PageNumber and PageSize must be greater than zero."));
        }

        return ResultT<PagedResult<T>>.Success(null); // Indicates that it is valid
    }
}