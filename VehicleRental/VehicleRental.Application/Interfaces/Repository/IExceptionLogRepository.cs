using VehicleRental.Application.Models;

namespace VehicleRental.Application.Interfaces.Repository;
public interface IExceptionLogRepository
{
    Task SaveAsync(ExceptionLog exceptionLog);
}
