using VehicleRental.Application.DTOs.Vehicle;
using VehicleRental.Domain.Entities.Base;

namespace VehicleRental.Application.Interfaces.UseCases.Vehicle;

public interface IGetVehicleUseCase
{
    Task<Result<VehicleDTO>> HandleAsync(string identifier, CancellationToken cancellationToken = default);
}
