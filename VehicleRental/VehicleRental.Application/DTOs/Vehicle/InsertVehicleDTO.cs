namespace VehicleRental.Application.DTOs.Vehicle;

public class InsertVehicleDTO
{
    public required string Identifier { get; set; }
    public required string Plate { get; init; }
    public required int Year { get; init; }
    public required string Model { get; init; }
}
