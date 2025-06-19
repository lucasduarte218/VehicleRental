using VehicleRental.Application.DTOs.Rent;
using VehicleRental.Application.Interfaces.UseCases.Rent;
using VehicleRental.Domain.Entities.Base;
using VehicleRental.Domain.Enums;
using VehicleRental.Domain.Interfaces.Repositories;

namespace VehicleRental.Application.UseCases.Rent
{
    public class GetRentUseCase(IRentRepository rentRepository) : IGetRentUseCase
    {
        public async Task<Result<RentDTO>> HandleAsync(string identifier, CancellationToken cancellationToken = default)
        {
            VehicleRental.Domain.Entities.Rent? rent = await rentRepository.GetByIdAsync(identifier, cancellationToken);

            if (rent is null)
            {
                return Result<RentDTO>.Failure("Aluguel não encontrado", ResultErrorType.NotFound);
            }

            RentDTO resultDto = new()
            {
                Identifier = rent.Id,
                VehicleIdentifier = rent.VehicleIdentifier,
                CourierIdentifier = rent.CourierIdentifier,
                StartDate = rent.StartDate,
                EndDate = rent.EndDate,
                EstimatedEndDate = rent.EstimatedEndDate,
                Status = rent.Status,
                DailyRate = rent.DailyRate,
            };

            return Result<RentDTO>.Success(resultDto);
        }
    }
}
