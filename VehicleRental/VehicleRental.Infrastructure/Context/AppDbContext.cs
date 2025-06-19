using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using VehicleRental.Domain.Entities;
using VehicleRental.Domain.Interfaces;

namespace VehicleRental.Infrastructure.Context
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; } = default!;
        public DbSet<Vehicle> Vehicles { get; set; } = default!;
        public DbSet<Rent> Rents { get; set; } = default!;

        public AppDbContext(DbContextOptions options) : base(options) => SavingChanges += OnSavingChanges;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
            .HasKey(x => x.Id);

            modelBuilder.Entity<Vehicle>()
                        .HasKey(x => x.Id);

            modelBuilder.Entity<Rent>()
                        .HasKey(x => x.Id);
        }

        private void OnSavingChanges(object? sender, SavingChangesEventArgs e) => UpdateAuditableColunmns();
        private void UpdateAuditableColunmns()
        {
            foreach (EntityEntry<IEntity> entry in ChangeTracker.Entries<IEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        {
                            entry.Entity.CreatedAt = DateTime.UtcNow;
                            break;
                        }

                    case EntityState.Modified:
                        {
                            entry.Entity.UpdatedAt = DateTime.UtcNow;
                            break;
                        }

                    case EntityState.Deleted:
                        {
                            entry.State = EntityState.Unchanged;
                            entry.Entity.DeletedAt = DateTime.UtcNow;
                            break;
                        }
                }
            }
        }
    }
}
