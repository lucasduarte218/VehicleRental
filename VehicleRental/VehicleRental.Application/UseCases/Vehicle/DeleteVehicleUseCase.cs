using VehicleRental.Application.Interfaces.UseCases.Vehicle;
using VehicleRental.Domain.Entities.Base;
using VehicleRental.Domain.Enums;
using VehicleRental.Domain.Interfaces.Repositories;

namespace VehicleRental.Application.UseCases.Vehicle
{
    public class DeleteVehicleUseCase(IVehicleRepository motorcycleRepository, IRentRepository rentRepository) : IDeleteVehicleUseCase
    {
        public async Task<Result> HandleAsync(string identifier, CancellationToken cancellationToken = default)
        {
            VehicleRental.Domain.Entities.Vehicle? motorcycle = await motorcycleRepository.GetByIdAsync(identifier, cancellationToken);

            if (motorcycle is null)
            {
                return Result.Failure("Moto não encontrada", ResultErrorType.NotFound);
            }

            bool isMotorcycleRented = await rentRepository.IsVehicleRentedAsync(motorcycle.Id, cancellationToken);

            if (isMotorcycleRented)
            {
                return Result.Failure("Moto está atualmente alugada", ResultErrorType.BusinessError);
            }

            await motorcycleRepository.DeleteAsync(motorcycle.Id, cancellationToken);

            return Result.Success();
        }
    }
}
