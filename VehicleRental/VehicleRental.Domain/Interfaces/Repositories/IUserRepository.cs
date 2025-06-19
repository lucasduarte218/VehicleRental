using VehicleRental.Domain.Entities;
using VehicleRental.Domain.Enums;
using VehicleRental.Domain.Filters.User.Find;

namespace VehicleRental.Domain.Interfaces.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<List<User>> GetByRoleAsync(UserType type, CancellationToken cancellationToken = default);
        Task<List<User>> FindAsync(UserFindFilter filter, CancellationToken cancellationToken = default);
        Task<User?> GetByLicenseNumberAsync(string licenseNumber, CancellationToken cancellationToken = default);
        Task<User?> GetByTaxNumberAsync(string taxNumber, CancellationToken cancellationToken);
    }
}
