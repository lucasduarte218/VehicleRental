using System.Security.Principal;
using VehicleRental.Domain.Interfaces;

namespace VehicleRental.Domain.Entities.Base
{
    public abstract class EntityBase : IEntity
    {
        public virtual string Id { get; set; }
        public virtual DateTimeOffset CreatedAt { get; set; }
        public virtual DateTimeOffset? UpdatedAt { get; set; }
        public virtual DateTimeOffset? DeletedAt { get; set; }
    }
}
