using Microsoft.EntityFrameworkCore;
using VehicleRental.Domain.Interfaces;
using VehicleRental.Domain.Interfaces.Repositories;
using VehicleRental.Infrastructure.Context;

namespace VehicleRental.Infrastructure.Repositories.Base;

public class RepositoryBase<T> : IRepository<T> where T : class, IEntity
{
    protected AppDbContext Context { get; }
    protected DbSet<T> DbSet { get; }

    public RepositoryBase(AppDbContext context)
    {
        Context = context;
        DbSet = Context.Set<T>();
    }

    public virtual async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await DbSet.AddAsync(entity, cancellationToken);
        await Context.SaveChangesAsync(cancellationToken);
    }

    public virtual async Task DeleteAsync(string identifier, CancellationToken cancellationToken = default)
    {
        T? entity = await DbSet.FindAsync([identifier], cancellationToken: cancellationToken);
        if (entity != null)
        {
            DbSet.Remove(entity);
            await Context.SaveChangesAsync(cancellationToken);
        }
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(e => e.DeletedAt == null)
                          .AsNoTracking()
                          .ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task<T?> GetByIdAsync(string identifier, CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(e => e.DeletedAt == null)
                          .AsNoTracking()
                          .FirstOrDefaultAsync(e => e.Id == identifier, cancellationToken: cancellationToken);
    }

    public virtual async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        DbSet.Update(entity);
        await Context.SaveChangesAsync(cancellationToken);
    }
}
