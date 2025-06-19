using VehicleRental.Application.DTOs.Vehicle;
using VehicleRental.Domain.Entities.Base;

namespace VehicleRental.Application.Interfaces.UseCases.Vehicle;

public interface IInsertVehicleUseCase
{
    Task<Result<VehicleDTO>> HandleAsync(InsertVehicleDTO dto, CancellationToken cancellationToken = default);
}
