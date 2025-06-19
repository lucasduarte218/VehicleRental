using VehicleRental.Application.DTOs.Vehicle;
using VehicleRental.Application.Interfaces.UseCases.Vehicle;
using VehicleRental.Domain.Entities.Base;
using VehicleRental.Domain.Enums;
using VehicleRental.Domain.Interfaces.Repositories;

namespace VehicleRental.Application.UseCases.Vehicle
{
    public class UpdateVehicleUseCase(IVehicleRepository vehicleRepository) : IUpdateVehicleUseCase
    {
        public async Task<Result<VehicleDTO>> HandleAsync(string identifier, UpdateVehicleDTO dto, CancellationToken cancellationToken = default)
        {
            VehicleRental.Domain.Entities.Vehicle? existingVehicle = await vehicleRepository.GetByIdAsync(identifier, cancellationToken);

            if (existingVehicle is null)
            {
                return Result<VehicleDTO>.Failure("Veiculo não encontrado", ResultErrorType.NotFound);
            }

            if (!string.IsNullOrWhiteSpace(dto.Plate)) // Atualizar placa
            {
                if (await IsPlateInUseAsync(dto.Plate, identifier, cancellationToken))
                {
                    return Result<VehicleDTO>.Failure("Já existe um Veiculo com esta placa registrada no sistema", ResultErrorType.BusinessError);
                }

                existingVehicle.Plate = dto.Plate;
            }

            if (dto.Year > 1900) // Atualizar ano
            {
                existingVehicle.Year = dto.Year.Value;
            }

            if (!string.IsNullOrWhiteSpace(dto.Model)) // Atualizar modelo
            {
                existingVehicle.Model = dto.Model;
            }

            await vehicleRepository.UpdateAsync(existingVehicle, cancellationToken);

            VehicleDTO resultDto = new()
            {
                Identifier = existingVehicle.Id,
                Plate = existingVehicle.Plate,
                Year = existingVehicle.Year,
                Model = existingVehicle.Model
            };

            return Result<VehicleDTO>.Success(resultDto);
        }

        private async Task<bool> IsPlateInUseAsync(string plate, string identifier, CancellationToken cancellationToken)
        {
            VehicleRental.Domain.Entities.Vehicle? motorcycleWithSamePlate = await vehicleRepository.GetByPlateIdentificationAsync(plate, cancellationToken);
            return motorcycleWithSamePlate is not null && motorcycleWithSamePlate.Id != identifier;
        }
    }
}
