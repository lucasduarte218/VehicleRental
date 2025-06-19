using VehicleRental.Domain.Enums;

namespace VehicleRental.Application.DTOs.Rent;

public class CompleteRentResultDTO
{
    public required string Identifier { get; set; }
    public required string MotorcycleIdentifier { get; set; }
    public required string CourierIdentifier { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalCost { get; set; }
    public RentStatus Status { get; set; }
}
