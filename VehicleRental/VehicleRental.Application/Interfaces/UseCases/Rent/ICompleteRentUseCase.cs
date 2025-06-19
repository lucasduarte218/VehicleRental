using VehicleRental.Application.DTOs.Rent;
using VehicleRental.Domain.Entities.Base;

namespace VehicleRental.Application.Interfaces.UseCases.Rent;
public interface ICompleteRentUseCase
{
    Task<Result<CompleteRentResultDTO>> HandleAsync(CompleteRentRequestDTO dto, CancellationToken cancellationToken = default);
}
