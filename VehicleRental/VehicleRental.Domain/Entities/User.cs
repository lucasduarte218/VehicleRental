using VehicleRental.Domain.Entities.Base;
using VehicleRental.Domain.Enums;

namespace VehicleRental.Domain.Entities
{
    public class User : EntityBase
    {
        public UserType Type { get; set; }
        public string Name { get; set; }
        public string TaxNumber { get; set; }
        public DateTime BirthDate { get; set; }
        public string DriverLicenseNumber { get; set; }
        public DriverLicenseType? DriverLicenseType { get; set; }
        public string DriverLicenseImageIdentifier { get; set; }
    }
}
