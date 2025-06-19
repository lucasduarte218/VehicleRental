using VehicleRental.Domain.Entities;

namespace VehicleRental.Domain.Interfaces.Repositories
{
    public interface IRentRepository : IRepository<Rent>
    {
        Task<Rent?> GetActiveByVehicleIdentifierAsync(string vehicleIdentifier, CancellationToken cancellationToken);
        Task<List<Rent>> GetByCourierAsync(string courierIdentifier, CancellationToken cancellationToken);
        Task<List<Rent>> GetActivesAsync(CancellationToken cancellationToken);
        Task<bool> IsVehicleRentedAsync(string vehicleIdentifier, CancellationToken cancellationToken);
    }
}
