namespace VehicleRental.Domain.Interfaces
{
    public interface IEntity
    {
        string Id { get; set; }
        DateTimeOffset CreatedAt { get; set; }
        DateTimeOffset? UpdatedAt { get; set; }
        DateTimeOffset? DeletedAt { get; set; }
    }
}
