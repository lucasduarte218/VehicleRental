namespace VehicleRental.Application.DTOs.Rent;

public class RentVehicleDTO
{
    public string? Identifier { get; set; }
    public required string CourierIdentifier { get; set; }
    public required string VehicleIdentifier { get; set; }
    public required int Contract { get; set; }
}
