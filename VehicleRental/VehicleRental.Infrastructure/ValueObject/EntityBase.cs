using VehicleRental.Domain.Interfaces;

namespace VehicleRental.Infrastructure.ValueObject
{
    public abstract class EntityBase : IEntity
    {
        public virtual string Id { get; set; }
        public virtual DateTimeOffset CreatedAt { get; set; }
        public virtual DateTimeOffset? UpdatedAt { get; set; }
        public virtual DateTimeOffset? DeletedAt { get; set; }
    }
}
