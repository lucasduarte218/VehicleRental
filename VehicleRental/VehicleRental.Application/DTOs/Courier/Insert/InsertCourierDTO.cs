using VehicleRental.Domain.Enums;

namespace VehicleRental.Application.DTOs.Courier.Insert;

public class InsertCourierDTO
{
    public required string Identifier { get; set; }
    public required string Name { get; set; }
    public required string TaxNumber { get; set; }
    public required DateTime BirthDate { get; set; }
    public required string DriverLicenseNumber { get; set; }
    public required DriverLicenseType DriverLicenseType { get; set; }
    public required string DriverLicenseImageBase64 { get; set; }
}
