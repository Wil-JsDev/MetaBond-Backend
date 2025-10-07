using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Helpers;

public static class EntityHelper
{
    public static async Task<ResultT<T>> GetEntityByIdAsync<T>(
        Func<Guid, Task<T>> getEntityByIdAsync,
        Guid id,
        string entityName,
        ILogger logger) where T : class
    {
        var entity = await getEntityByIdAsync(id);
        if (entity is not null)
            return ResultT<T>.Success(entity);

        logger.LogWarning("{EntityName} with ID {Id} was not found.", entityName, id);

        return ResultT<T>.Failure(Error.NotFound("404", $"{entityName} with ID {id} was not found."));
    }
}