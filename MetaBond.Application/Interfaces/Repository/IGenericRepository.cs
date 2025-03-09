using System.Linq.Expressions;

namespace MetaBond.Application.Interfaces.Repository
{
    /// <summary>
    /// <para> TEntity: It must be a class.</para>
    /// </summary>
    public interface IGenericRepository<TEntity>
        where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAll(CancellationToken cancellationToken);

        Task<TEntity> GetByIdAsync(Guid id);

        Task CreateAsync(TEntity entity, CancellationToken cancellationToken);

        Task UpdateAsync(TEntity entity, CancellationToken cancellationToken);

        Task DeleteAsync(TEntity entity, CancellationToken cancellationToken);

        Task<bool> ValidateAsync(Expression<Func<TEntity, bool>> predicate);
        
        Task SaveAsync(CancellationToken cancellationToken);
    }
}
