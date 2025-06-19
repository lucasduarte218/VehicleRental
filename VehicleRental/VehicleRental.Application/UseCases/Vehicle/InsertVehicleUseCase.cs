using VehicleRental.Application.DTOs.Vehicle;
using VehicleRental.Application.Events;
using VehicleRental.Application.Interfaces.Messaging;
using VehicleRental.Application.Interfaces.UseCases.Vehicle;
using VehicleRental.Domain.Entities.Base;
using VehicleRental.Domain.Enums;
using VehicleRental.Domain.Interfaces.Repositories;

namespace VehicleRental.Application.UseCases.Vehicle
{
    public class InsertVehicleUseCase(IVehicleRepository vehicleRepository, IVehicleEventPublisher eventPublisher) : IInsertVehicleUseCase
    {
        public async Task<Result<VehicleDTO>> HandleAsync(InsertVehicleDTO dto, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(dto.Plate))
            {
                return Result<VehicleDTO>.Failure("Placa inválida", ResultErrorType.ValidationError);
            }

            if (dto.Year <= 1900)
            {
                return Result<VehicleDTO>.Failure("O ano deve ser maior que 1900", ResultErrorType.ValidationError);
            }

            VehicleRental.Domain.Entities.Vehicle? vehicle = await vehicleRepository.GetByPlateIdentificationAsync(dto.Plate, cancellationToken);

            if (vehicle is not null)
            {
                return Result<VehicleDTO>.Failure("Já existe uma moto com esta placa registrado no sistema", ResultErrorType.BusinessError);
            }

            vehicle = await vehicleRepository.GetByIdAsync(dto.Identifier, cancellationToken);

            if (vehicle is not null)
            {
                return Result<VehicleDTO>.Failure("Já existe uma moto com este identificador no sistema", ResultErrorType.BusinessError);
            }

            vehicle = new()
            {
                Id = dto.Identifier,
                Plate = dto.Plate,
                Year = dto.Year,
                Model = dto.Model,
            };

            await vehicleRepository.AddAsync(vehicle, cancellationToken);

            await eventPublisher.PublishVehicleRegisteredAsync(new VehicleRegisteredEvent(vehicle.Id,
                                                                                                vehicle.Plate,
                                                                                                vehicle.Year,
                                                                                                vehicle.Model), cancellationToken);

            VehicleDTO resultDto = new()
            {
                Identifier = vehicle.Id,
                Plate = vehicle.Plate,
                Year = vehicle.Year,
                Model = vehicle.Model
            };

            return Result<VehicleDTO>.Success(resultDto);
        }
    }
}
