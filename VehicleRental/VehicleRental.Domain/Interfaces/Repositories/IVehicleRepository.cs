using VehicleRental.Domain.Entities;
using VehicleRental.Domain.Filters.Vehicles.Find;

namespace VehicleRental.Domain.Interfaces.Repositories;

public interface IVehicleRepository : IRepository<Vehicle>
{
    Task<Vehicle?> GetByPlateIdentificationAsync(string plate, CancellationToken cancellationToken = default);
    Task<List<Vehicle>> FindAsync(VehicleFindFilter filter, CancellationToken cancellationToken = default);
}