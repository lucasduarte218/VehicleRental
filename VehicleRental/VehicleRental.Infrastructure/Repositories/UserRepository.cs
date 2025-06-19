using Microsoft.EntityFrameworkCore;
using VehicleRental.Domain.Entities;
using VehicleRental.Domain.Enums;
using VehicleRental.Domain.Filters.User.Find;
using VehicleRental.Domain.Interfaces.Repositories;
using VehicleRental.Infrastructure.Context;
using VehicleRental.Infrastructure.Repositories.Base;

namespace VehicleRental.Infrastructure.Repositories;

public class UserRepository(AppDbContext context) : RepositoryBase<User>(context), IUserRepository
{
    public async Task<List<User>> GetByRoleAsync(UserType type, CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(u => u.DeletedAt == null)
                          .Where(u => u.Type == type)
                          .AsNoTracking()
                          .ToListAsync(cancellationToken);

    }

    public Task<List<User>> FindAsync(UserFindFilter filters, CancellationToken cancellationToken = default)
    {
        IQueryable<User> query = DbSet.Where(u => u.DeletedAt == null)
                                      .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(filters.Name))
        {
            query = query.Where(u => u.Name.Contains(filters.Name));
        }

        if (!string.IsNullOrWhiteSpace(filters.TaxNumber))
        {
            query = query.Where(u => u.TaxNumber.Contains(filters.TaxNumber));
        }

        if (!string.IsNullOrWhiteSpace(filters.DriverLicenseNumber))
        {
            query = query.Where(u => u.DriverLicenseNumber.Contains(filters.DriverLicenseNumber));
        }

        if (filters.Type.HasValue)
        {
            query = query.Where(u => u.Type == filters.Type.Value);
        }

        return query.Skip(filters.Offset)
                    .Take(filters.Limit)
                    .ToListAsync(cancellationToken);
    }

    public Task<User?> GetByLicenseNumberAsync(string licenseNumber, CancellationToken cancellationToken = default)
    {
        return DbSet.AsNoTracking()
                    .Where(u => u.DeletedAt == null)
                    .FirstOrDefaultAsync(u => u.DriverLicenseNumber == licenseNumber, cancellationToken);
    }

    public Task<User?> GetByTaxNumberAsync(string taxNumber, CancellationToken cancellationToken)
    {
        return DbSet.AsNoTracking()
                .Where(u => u.DeletedAt == null)
                .FirstOrDefaultAsync(u => u.TaxNumber == taxNumber, cancellationToken);
    }
}
