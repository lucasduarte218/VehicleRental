using VehicleRental.Domain.Entities.Base;

namespace VehicleRental.Domain.Entities;

public class Vehicle : EntityBase
{
    public string Model { get; set; }
    public int Year { get; set; }
    public string Plate { get; set; }
}
