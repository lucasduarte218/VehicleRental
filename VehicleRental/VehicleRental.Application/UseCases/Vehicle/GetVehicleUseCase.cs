using VehicleRental.Application.DTOs.Vehicle;
using VehicleRental.Application.Interfaces.UseCases.Vehicle;
using VehicleRental.Domain.Entities.Base;
using VehicleRental.Domain.Enums;
using VehicleRental.Domain.Interfaces.Repositories;

namespace VehicleRental.Application.UseCases.Vehicle
{
    public class GetVehicleUseCase(IVehicleRepository vehicleRepository) : IGetVehicleUseCase
    {
        public async Task<Result<VehicleDTO>> HandleAsync(string identifier, CancellationToken cancellationToken = default)
        {
            VehicleRental.Domain.Entities.Vehicle? vehicle = await vehicleRepository.GetByIdAsync(identifier, cancellationToken);

            if (vehicle is null)
            {
                return Result<VehicleDTO>.Failure("Veiculo não encontrado", ResultErrorType.NotFound);
            }

            VehicleDTO dto = new()
            {
                Identifier = vehicle.Id,
                Plate = vehicle.Plate,
                Year = vehicle.Year,
                Model = vehicle.Model
            };
            return Result<VehicleDTO>.Success(dto);
        }
    }
}
