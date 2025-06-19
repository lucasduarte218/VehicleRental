
using VehicleRental.Application.DTOs.Courier;
using VehicleRental.Application.DTOs.Courier.Update;
using VehicleRental.Domain.Entities.Base;

namespace VehicleRental.Application.Interfaces.UseCases.Courier;

public interface IUpdateCourierUseCase
{
    Task<Result<CourierDTO>> HandleAsync(string identifier, UpdateCourierDTO dto, CancellationToken cancellationToken = default);
}
