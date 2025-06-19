using VehicleRental.Application.DTOs.Rent;
using VehicleRental.Application.Interfaces.UseCases.Rent;
using VehicleRental.Domain.Entities;
using VehicleRental.Domain.Entities.Base;
using VehicleRental.Domain.Enums;
using VehicleRental.Domain.Interfaces.Repositories;
using VehicleRental.Domain.Interfaces.Services;
using VehicleRental.Domain.ValueObjects;

namespace VehicleRental.Application.UseCases.Rent
{
    public class InsertRentUseCase(IRentRepository rentRepository,
                                       IVehicleRepository vehicleRepository,
                                       IUserRepository userRepository,
                                       IRentContractCatalog rentContractCatalog) : IInsertRentUseCase
    {
        public async Task<Result<RentDTO>> HandleAsync(RentVehicleDTO dto, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(dto.Identifier))
            {
                dto.Identifier = Guid.NewGuid().ToString();
            }
            else
            {
                if (await rentRepository.GetByIdAsync(dto.Identifier, cancellationToken) is not null)
                {
                    return Result<RentDTO>.Failure("Identificador de locação já existe", ResultErrorType.ValidationError);
                }
            }

            if (string.IsNullOrWhiteSpace(dto.CourierIdentifier))
            {
                return Result<RentDTO>.Failure("Identificador do locatário inválido", ResultErrorType.ValidationError);
            }

            if (string.IsNullOrWhiteSpace(dto.VehicleIdentifier))
            {
                return Result<RentDTO>.Failure("Identificador da moto inválido", ResultErrorType.ValidationError);
            }

            RentContract? contract = rentContractCatalog.FindContractByNumber(dto.Contract);

            if (contract is null)
            {
                return Result<RentDTO>.Failure("Plano de aluguel inválido", ResultErrorType.ValidationError);
            }

            VehicleRental.Domain.Entities.Vehicle? vehicle = await vehicleRepository.GetByIdAsync(dto.VehicleIdentifier, cancellationToken);

            if (vehicle is null)
            {
                return Result<RentDTO>.Failure("Moto não encontrada", ResultErrorType.NotFound);
            }

            User? courier = await userRepository.GetByIdAsync(dto.CourierIdentifier, cancellationToken);

            if (courier is null)
            {
                return Result<RentDTO>.Failure("Entregador com identificador informado não encontrado", ResultErrorType.NotFound);
            }

            if (courier.DriverLicenseType is not DriverLicenseType.A and not DriverLicenseType.AB)
            {
                return Result<RentDTO>.Failure("O entregador não tem o tipo de licença A para realizar o aluguel", ResultErrorType.BusinessError);
            }

            VehicleRental.Domain.Entities.Rent? existingRent = await rentRepository.GetActiveByVehicleIdentifierAsync(dto.VehicleIdentifier, cancellationToken);

            if (existingRent is not null)
            {
                return Result<RentDTO>.Failure("Esta moto já está alugada", ResultErrorType.BusinessError);
            }

            DateTime startDate = DateTime.Now.AddDays(1).Date;
            DateTime estimatedEndDate = startDate.AddDays(contract.DurationInDays).Date;

            VehicleRental.Domain.Entities.Rent rent = new()
            {
                Id = dto.Identifier,
                VehicleIdentifier = dto.VehicleIdentifier,
                CourierIdentifier = dto.CourierIdentifier,
                StartDate = startDate,
                EndDate = null,
                EstimatedEndDate = estimatedEndDate,
                Status = RentStatus.Active,
                DailyRate = contract.DailyRate,
                EarlyReturnDailyPenalty = contract.EarlyReturnDailyPenalty,
                LateReturnDailyFee = contract.LateReturnDailyFee
            };

            await rentRepository.AddAsync(rent, cancellationToken);

            RentDTO resultDto = new()
            {
                Identifier = rent.Id,
                VehicleIdentifier = rent.VehicleIdentifier,
                CourierIdentifier = rent.CourierIdentifier,
                StartDate = rent.StartDate,
                EndDate = rent.EndDate,
                EstimatedEndDate = rent.EstimatedEndDate,
                Plan = contract.Identifier,
                DailyRate = contract.DailyRate,
                Status = rent.Status
            };

            return Result<RentDTO>.Success(resultDto);
        }
    }
}
