using System.Linq.Expressions;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace MetaBond.Infrastructure.Persistence.Repository
{
    /// <summary>
    /// <para> TEntity: It must be a class.</para>
    /// </summary>
    public class GenericRepository<TEntity> : IGenericRepository<TEntity>
        where TEntity : class
    {
        protected readonly MetaBondContext _metaBondContext;

        public GenericRepository(MetaBondContext metaBondContext)
        {
            _metaBondContext = metaBondContext;
        }

        public virtual async Task CreateAsync(TEntity entity, CancellationToken cancellationToken)
        {
            await _metaBondContext.Set<TEntity>().AddAsync(entity, cancellationToken);
            await SaveAsync(cancellationToken);
        }

        public virtual async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken)
        {
            _metaBondContext.Remove(entity);
            await SaveAsync(cancellationToken);
        }

        public virtual async Task<IEnumerable<TEntity>> GetAll(CancellationToken cancellationToken) =>
            await _metaBondContext.Set<TEntity>().ToListAsync(cancellationToken);


        public virtual async Task<TEntity> GetByIdAsync(Guid id)
        {
            TEntity? entity = await _metaBondContext.Set<TEntity>().FindAsync(id);
            return entity;
        }

        public virtual async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken)
        {
            _metaBondContext.Set<TEntity>().Attach(entity);
            _metaBondContext.Entry(entity).State = EntityState.Modified;
            await SaveAsync(cancellationToken);
        }

        public virtual async Task SaveAsync(CancellationToken cancellationToken) =>
            await _metaBondContext.SaveChangesAsync(cancellationToken);

        public async Task<bool> ValidateAsync(Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken)
        {
            return await _metaBondContext.Set<TEntity>()
                .AsNoTracking()
                .AnyAsync(predicate, cancellationToken);
        }
    }
}