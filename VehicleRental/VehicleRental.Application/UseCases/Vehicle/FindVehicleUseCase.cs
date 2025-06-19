using VehicleRental.Application.DTOs.Vehicle;
using VehicleRental.Application.Interfaces.UseCases.Vehicle;
using VehicleRental.Domain.Entities.Base;
using VehicleRental.Domain.Enums;
using VehicleRental.Domain.Filters.Vehicles.Find;
using VehicleRental.Domain.Interfaces.Repositories;

namespace VehicleRental.Application.UseCases.Vehicle
{
    public class FindVehicleUseCase(IVehicleRepository vehicleRepository) : IFindVehicleUseCase
    {
        public async Task<Result<List<VehicleDTO>>> HandleAsync(VehicleFindFilter dto, CancellationToken cancellationToken = default)
        {
            if (dto.Offset < 0 || dto.Limit <= 0)
            {
                return Result<List<VehicleDTO>>.Failure("Offset and limit must be greater than or equal to zero.", ResultErrorType.ValidationError);
            }

            if (dto.Limit > 100)
            {
                return Result<List<VehicleDTO>>.Failure("Limit cannot exceed 100.", ResultErrorType.ValidationError);
            }

            List<VehicleRental.Domain.Entities.Vehicle> vehicleList = await vehicleRepository.FindAsync(dto, cancellationToken);

            List<VehicleDTO> dtoList = [.. vehicleList.Select(ToDto)];

            return Result<List<VehicleDTO>>.Success(dtoList);
        }

        private static VehicleDTO ToDto(VehicleRental.Domain.Entities.Vehicle vehicle)
        {
            return new VehicleDTO
            {
                Identifier = vehicle.Id,
                Year = vehicle.Year,
                Plate = vehicle.Plate,
                Model = vehicle.Model,
            };
        }
    }
}
