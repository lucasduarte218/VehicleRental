using VehicleRental.Domain.Entities.Base;
using VehicleRental.Domain.Enums;

namespace VehicleRental.Domain.Entities;

public class Rent : EntityBase
{
    public string CourierIdentifier { get; set; }
    public string VehicleIdentifier { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime EstimatedEndDate { get; set; }
    public decimal DailyRate { get; set; }
    public decimal EarlyReturnDailyPenalty { get; set; }
    public decimal LateReturnDailyFee { get; set; }
    public RentStatus Status { get; set; }
}
