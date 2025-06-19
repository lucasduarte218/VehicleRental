using VehicleRental.Domain.Interfaces.Services;
using VehicleRental.Domain.ValueObjects;

namespace VehicleRental.Application.Services;

public class RentContractCatalog(List<RentContract> plans) : IRentContractCatalog
{
    public IReadOnlyDictionary<int, RentContract> Plans { get; } = plans.ToDictionary(static p => p.Identifier, static p => p)
                                                                    .AsReadOnly();

    public RentContract? FindContractByNumber(int planNumber)
    {
        Plans.TryGetValue(planNumber, out RentContract? plan);
        return plan;
    }
    public IReadOnlyList<RentContract> GetAllPlans() => Plans.Values.ToList().AsReadOnly();
}
