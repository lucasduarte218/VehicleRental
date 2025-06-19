using VehicleRental.Domain.Filters.Base;

namespace VehicleRental.Domain.Filters.Vehicles.Find;

public class VehicleFindFilter : PagedBase
{
    public string? Plate { get; set; }
    public string? Model { get; set; }
    public int? Year { get; }
}
