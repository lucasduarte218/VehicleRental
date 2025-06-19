using VehicleRental.Application.DTOs.Rent;
using VehicleRental.Application.Interfaces.UseCases.Rent;
using VehicleRental.Domain.Entities.Base;
using VehicleRental.Domain.Enums;
using VehicleRental.Domain.Interfaces.Repositories;
using VehicleRental.Domain.Interfaces.Services;

namespace VehicleRental.Application.UseCases.Rent;

public class CompleteRentUseCase(IRentRepository rentRepository, IRentPricingCalculator pricingCalculator) : ICompleteRentUseCase
{
    public async Task<Result<CompleteRentResultDTO>> HandleAsync(CompleteRentRequestDTO dto, CancellationToken cancellationToken = default)
    {
        VehicleRental.Domain.Entities.Rent? rent = await rentRepository.GetByIdAsync(dto.RentIdentifier, cancellationToken);

        if (rent is null)
        {
            return Result<CompleteRentResultDTO>.Failure("Aluguel não encontrado", ResultErrorType.NotFound);
        }

        if (rent.Status != RentStatus.Active)
        {
            return Result<CompleteRentResultDTO>.Failure("Este aluguel já foi encerrado", ResultErrorType.BusinessError);
        }

        if (dto.ReturnDate < rent.StartDate)
        {
            return Result<CompleteRentResultDTO>.Failure("Data de devolução inválida", ResultErrorType.BusinessError);
        }

        rent.EndDate = dto.ReturnDate;
        rent.Status = RentStatus.Completed;

        decimal totalCost = pricingCalculator.CalculateRentalCost(rent, dto.ReturnDate);

        await rentRepository.UpdateAsync(rent, cancellationToken);

        CompleteRentResultDTO resultDto = new()
        {
            Identifier = rent.Id,
            MotorcycleIdentifier = rent.VehicleIdentifier,
            CourierIdentifier = rent.CourierIdentifier,
            StartDate = rent.StartDate,
            EndDate = rent.EndDate.Value,
            TotalCost = totalCost,
            Status = rent.Status
        };

        return Result<CompleteRentResultDTO>.Success(resultDto);
    }
}
