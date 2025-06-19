using VehicleRental.Domain.Enums;

namespace VehicleRental.Application.DTOs.Courier;

public class CourierDTO
{
    public required string Identifier { get; set; }
    public required string Name { get; set; }
    public string? TaxNumber { get; set; }
    public DateOnly BirthDate { get; set; }
    public string? DriverLicenseNumber { get; set; }
    public DriverLicenseType? DriverLicenseType { get; set; }
    public string? DriverLicenseImageBase64 { get; set; }
}
