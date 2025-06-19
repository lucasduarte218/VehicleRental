using VehicleRental.Application.Models;

namespace VehicleRental.Application.Interfaces.Repository;

public interface ILogRepository
{
    public Task SaveAsync(Log log);
}