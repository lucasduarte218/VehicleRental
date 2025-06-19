
using VehicleRental.Application.DTOs.Courier;
using VehicleRental.Application.DTOs.Courier.Insert;
using VehicleRental.Domain.Entities.Base;

namespace VehicleRental.Application.Interfaces.UseCases.Courier;

public interface IInsertCourierUseCase
{
    Task<Result<CourierDTO>> HandleAsync(InsertCourierDTO dto, CancellationToken cancellationToken);
}
