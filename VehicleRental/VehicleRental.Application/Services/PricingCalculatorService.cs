using VehicleRental.Domain.Entities;
using VehicleRental.Domain.Interfaces.Services;

namespace VehicleRental.Application.Services
{
    public class DefaultRentPricingCalculator : IRentPricingCalculator
    {
        public decimal CalculateRentalCost(Rent rent, DateTime rentalEndDate)
        {
            if (rent.StartDate.Date > rentalEndDate.Date)
            {
                throw new ArgumentException("A data de devolução deve ser após a data de inicio do aluguel");
            }

            decimal baseCost = CalculateBaseCost(rent, rentalEndDate);
            decimal earlyPenalty = CalculateEarlyReturnPenalty(rent, rentalEndDate);
            decimal lateFee = CalculateLateReturnFee(rent, rentalEndDate);

            decimal total = baseCost + earlyPenalty + lateFee;
            return Math.Max(total, 0);
        }

        private static decimal CalculateBaseCost(Rent rent, DateTime rentalEndDate)
        {
            DateTime costEndDate = IsLateReturn(rent, rentalEndDate)
                                   ? rent.EstimatedEndDate
                                   : rentalEndDate;

            int usedDays = GetUsedDays(rent.StartDate, costEndDate);
            return usedDays * rent.DailyRate;
        }

        private static decimal CalculateEarlyReturnPenalty(Rent rent, DateTime rentalEndDate)
        {
            if (!IsEarlyReturn(rent, rentalEndDate))
            {
                return 0;
            }

            int unusedDays = GetUnusedDays(rent.EstimatedEndDate, rentalEndDate);
            return unusedDays * rent.DailyRate * rent.EarlyReturnDailyPenalty;
        }

        private static decimal CalculateLateReturnFee(Rent rent, DateTime rentalEndDate)
        {
            if (!IsLateReturn(rent, rentalEndDate))
            {
                return 0;
            }

            int extraDays = GetExtraDays(rent.EstimatedEndDate, rentalEndDate);
            return extraDays * rent.LateReturnDailyFee;
        }

        private static int GetUsedDays(DateTime startDate, DateTime endDate) =>
            Math.Max(0, (int)Math.Ceiling((endDate - startDate).TotalDays));

        private static int GetExtraDays(DateTime estimatedEnd, DateTime actualEnd) =>
            Math.Max(0, (int)Math.Ceiling((actualEnd - estimatedEnd).TotalDays));

        private static int GetUnusedDays(DateTime estimatedEnd, DateTime actualEnd) =>
            Math.Max(0, (int)Math.Ceiling((estimatedEnd - actualEnd).TotalDays));

        private static bool IsEarlyReturn(Rent rent, DateTime endDate) => endDate < rent.EstimatedEndDate;

        private static bool IsLateReturn(Rent rent, DateTime endDate) => endDate > rent.EstimatedEndDate;
    }
}
