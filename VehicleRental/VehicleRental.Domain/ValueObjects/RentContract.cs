namespace VehicleRental.Domain.ValueObjects
{
    public class RentContract
    {
        public required int Identifier { get; init; }
        public required decimal DailyRate { get; init; }
        public required int DurationInDays { get; init; }
        public required decimal EarlyReturnDailyPenalty { get; init; }
        public required decimal LateReturnDailyFee { get; init; }
    }
}
