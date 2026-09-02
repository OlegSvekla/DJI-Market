using System.Linq.Expressions;
using DJI.Core.Entities;
using DJI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DJI.Infrastructure.Repositories;

public class Repository<TEntity>(DjiDbContext context) : IRepository<TEntity>
    where TEntity : Entity
{
    public Task<TResult?> GetByIdAsync<TResult>(
        int id,
        Expression<Func<TEntity, TResult>> selector,
        CancellationToken ct = default)
        => Query().Where(entity => entity.Id == id).Select(selector).FirstOrDefaultAsync(ct);

    public Task<TResult?> FindAsync<TResult>(
        Expression<Func<TEntity, bool>> predicate,
        Expression<Func<TEntity, TResult>> selector,
        CancellationToken ct = default)
        => Query().Where(predicate).Select(selector).FirstOrDefaultAsync(ct);

    public async Task<List<TResult>> GetAllAsync<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? sorter = null,
        int? offset = null,
        int? amount = null,
        CancellationToken ct = default)
    {
        var query = Query();

        if (predicate is not null) query = query.Where(predicate);
        if (sorter is not null) query = sorter(query);
        if (offset is not null) query = query.Skip(offset.Value);
        if (amount is not null) query = query.Take(amount.Value);

        return await query.Select(selector).ToListAsync(ct);
    }

    public Task<int> CountAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken ct = default)
        => predicate is null
            ? Query().CountAsync(ct)
            : Query().CountAsync(predicate, ct);

    public Task<bool> AnyAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken ct = default)
        => predicate is null
            ? Query().AnyAsync(ct)
            : Query().AnyAsync(predicate, ct);

    public IQueryable<TEntity> Query() => context.Set<TEntity>().AsNoTracking();
}
