namespace VehicleRental.Domain.Filters.Base;

public abstract class PagedBase
{
    public string? Identifier { get; set; }
    public int Offset { get; set; } = 0;
    public int Limit { get; set; } = 10;
}
