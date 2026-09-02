using System.Linq.Expressions;
using DJI.Core.Entities;

namespace DJI.Infrastructure.Repositories;

public interface IRepository<TEntity>
    where TEntity : Entity
{
    Task<TResult?> GetByIdAsync<TResult>(
        int id,
        Expression<Func<TEntity, TResult>> selector,
        CancellationToken ct = default);

    Task<TResult?> FindAsync<TResult>(
        Expression<Func<TEntity, bool>> predicate,
        Expression<Func<TEntity, TResult>> selector,
        CancellationToken ct = default);

    Task<List<TResult>> GetAllAsync<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? sorter = null,
        int? offset = null,
        int? amount = null,
        CancellationToken ct = default);

    Task<int> CountAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken ct = default);

    Task<bool> AnyAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken ct = default);

    IQueryable<TEntity> Query();
}
