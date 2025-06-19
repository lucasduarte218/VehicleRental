using Microsoft.EntityFrameworkCore;
using VehicleRental.Domain.Entities;
using VehicleRental.Domain.Filters.Vehicles.Find;
using VehicleRental.Domain.Interfaces.Repositories;
using VehicleRental.Infrastructure.Context;
using VehicleRental.Infrastructure.Repositories.Base;

namespace VehicleRental.Infrastructure.Repositories;

public class VehicleRepository(AppDbContext context) : RepositoryBase<Vehicle>(context), IVehicleRepository
{
    public async Task<Vehicle?> GetByPlateIdentificationAsync(string plate, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Plate == plate, cancellationToken);
    }

    public async Task<List<Vehicle>> FindAsync(VehicleFindFilter filters, CancellationToken cancellationToken = default)
    {
        IQueryable<Vehicle> query = DbSet.Where(c => c.DeletedAt == null)
                                            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(filters.Identifier))
        {
            query = query.Where(m => m.Id.Contains(filters.Identifier));
        }

        if (!string.IsNullOrWhiteSpace(filters.Model))
        {
            query = query.Where(m => m.Model.Contains(filters.Model));
        }

        if (!string.IsNullOrWhiteSpace(filters.Plate))
        {
            query = query.Where(m => m.Plate.Contains(filters.Plate));
        }

        if (filters.Year.HasValue)
        {
            query = query.Where(m => m.Year == filters.Year.Value);
        }

        return await query.Skip(filters.Offset)
                          .Take(filters.Limit)
                          .ToListAsync(cancellationToken);
    }
}
