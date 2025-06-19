using VehicleRental.Domain.Enums;
using VehicleRental.Domain.Filters.Base;

namespace VehicleRental.Domain.Filters.User.Find;

public class UserFindFilter : PagedBase
{
    public UserType? Type { get; set; }
    public string? Name { get; set; }
    public string? TaxNumber { get; set; }
    public string? DriverLicenseNumber { get; set; }
}
