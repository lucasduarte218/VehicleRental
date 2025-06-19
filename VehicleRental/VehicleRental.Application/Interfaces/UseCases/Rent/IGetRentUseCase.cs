using VehicleRental.Application.DTOs.Rent;
using VehicleRental.Domain.Entities.Base;

namespace VehicleRental.Application.Interfaces.UseCases.Rent;
public interface IGetRentUseCase
{
    Task<Result<RentDTO>> HandleAsync(string identifier, CancellationToken cancellationToken = default);
}
