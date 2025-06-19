
using VehicleRental.Application.DTOs.Vehicle;
using VehicleRental.Domain.Entities.Base;

namespace VehicleRental.Application.Interfaces.UseCases.Vehicle;
    public interface IUpdateVehicleUseCase
    {
        Task<Result<VehicleDTO>> HandleAsync(string identifier, UpdateVehicleDTO dto, CancellationToken cancellationToken = default);
    }

