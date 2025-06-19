

using VehicleRental.Domain.ValueObjects;

namespace VehicleRental.Domain.Interfaces.Services
{
    public interface IRentContractCatalog
    {
        IReadOnlyList<RentContract> GetAllPlans();
        RentContract? FindContractByNumber(int planNumber);
    }
}
