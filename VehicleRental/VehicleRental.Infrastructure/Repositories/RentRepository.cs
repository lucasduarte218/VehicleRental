using Microsoft.EntityFrameworkCore;
using VehicleRental.Domain.Entities;
using VehicleRental.Domain.Enums;
using VehicleRental.Domain.Interfaces.Repositories;
using VehicleRental.Infrastructure.Context;
using VehicleRental.Infrastructure.Repositories.Base;

namespace VehicleRental.Infrastructure.Repositories;

public class RentRepository(AppDbContext context) : RepositoryBase<Rent>(context), IRentRepository
{
    public Task<Rent?> GetActiveByVehicleIdentifierAsync(string vehicleIdentifier, CancellationToken cancellationToken)
    {
        return DbSet.AsNoTracking()
                    .Where(e => e.DeletedAt == null)
                    .Where(e => e.VehicleIdentifier == vehicleIdentifier)
                    .Where(e => e.Status == RentStatus.Active)
                    .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<List<Rent>> GetActivesAsync(CancellationToken cancellationToken)
    {
        return DbSet.AsNoTracking()
                    .Where(e => e.DeletedAt == null)
                    .Where(e => e.Status == RentStatus.Active)
                    .ToListAsync(cancellationToken);
    }

    public Task<List<Rent>> GetByCourierAsync(string courierIdentifier, CancellationToken cancellationToken)
    {
        return DbSet.AsNoTracking()
                    .Where(e => e.DeletedAt == null)
                    .Where(e => e.CourierIdentifier == courierIdentifier)
                    .ToListAsync(cancellationToken);
    }

    public Task<bool> IsVehicleRentedAsync(string Vehicleidentifier, CancellationToken cancellationToken)
    {
        return DbSet.AsNoTracking()
                    .Where(e => e.DeletedAt == null)
                    .Where(e => e.VehicleIdentifier == Vehicleidentifier)
                    .Where(e => e.Status == RentStatus.Active)
                    .AnyAsync(cancellationToken);
    }
}
