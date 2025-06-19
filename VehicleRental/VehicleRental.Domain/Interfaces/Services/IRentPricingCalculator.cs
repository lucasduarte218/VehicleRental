using VehicleRental.Domain.Entities;

namespace VehicleRental.Domain.Interfaces.Services;

public interface IRentPricingCalculator
{
    decimal CalculateRentalCost(Rent rent, DateTime rentalEndDate);
}
