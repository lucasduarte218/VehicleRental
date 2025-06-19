using VehicleRental.Domain.Enums;

namespace VehicleRental.Application.DTOs.Courier.Update
{
    public class UpdateCourierDTO
    {
        public string? Name { get; set; }
        public string? TaxNumber { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? DriverLicenseNumber { get; set; }
        public DriverLicenseType? DriverLicenseType { get; set; }
        public string? DriverLicenseImageBase64 { get; set; }
    }
}
