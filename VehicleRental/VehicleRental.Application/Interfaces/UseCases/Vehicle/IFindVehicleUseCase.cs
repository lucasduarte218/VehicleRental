using VehicleRental.Application.DTOs.Vehicle;
using VehicleRental.Domain.Entities.Base;
using VehicleRental.Domain.Filters.Vehicles.Find;

namespace VehicleRental.Application.Interfaces.UseCases.Vehicle;

public interface IFindVehicleUseCase
{
    Task<Result<List<VehicleDTO>>> HandleAsync(VehicleFindFilter dto, CancellationToken cancellationToken = default);
}
