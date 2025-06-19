using VehicleRental.Application.DTOs.Rent;
using VehicleRental.Domain.Entities.Base;

namespace VehicleRental.Application.Interfaces.UseCases.Rent;
public interface IInsertRentUseCase
{
    Task<Result<RentDTO>> HandleAsync(RentVehicleDTO dto, CancellationToken cancellationToken = default);
}
