namespace VehicleRental.Application.DTOs.Vehicle;

public class RentVehicleDTO
{
    public string? Identifier { get; set; }
    public required string CourierIdentifier { get; set; }
    public required string MotorcycleIdentifier { get; set; }
    public required int Plan { get; set; }
}
