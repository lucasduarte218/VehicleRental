using VehicleRental.Application.Events;

namespace VehicleRental.Application.Interfaces.Messaging;

public interface IVehicleEventPublisher
{
    Task PublishVehicleRegisteredAsync(VehicleRegisteredEvent @event, CancellationToken cancellationToken);
}
