using VehicleRental.Domain.Entities.Base;

namespace VehicleRental.Application.Interfaces.UseCases.Vehicle;

public interface IDeleteVehicleUseCase
{
    Task<Result> HandleAsync(string identifier, CancellationToken cancellationToken = default);
}
