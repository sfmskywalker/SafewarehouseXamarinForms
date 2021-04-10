using Microsoft.EntityFrameworkCore;
using SafeWarehouseApp.Models;

namespace SafeWarehouseApp.Persistence
{
    public class SafeWarehouseContext : DbContext
    {
        public SafeWarehouseContext(DbContextOptions<SafeWarehouseContext> options) : base(options)
        {
        }

        public DbSet<DamageType> DamageTypes { get; set; } = default!;
        public DbSet<Supplier> Suppliers { get; set; } = default!;
        public DbSet<Material> Materials { get; set; } = default!;
        public DbSet<Material> Customers { get; set; } = default!;
        public DbSet<Report> Reports { get; set; } = default!;
        public DbSet<MediaItem> Files { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<DamageType>(a => { a.HasKey(b => b.Id); });
            modelBuilder.Entity<Supplier>(a => { a.HasKey(b => b.Id); });
            modelBuilder.Entity<MediaItem>(a => { a.HasKey(b => b.Id); });

            modelBuilder.Entity<Material>(a =>
            {
                a.HasKey(b => b.Id);
                a.HasIndex(b => b.SupplierId);
            });

            modelBuilder.Entity<Customer>(customer =>
            {
                customer.HasKey(x => x.Id);
                customer.Ignore(x => x.Suppliers).Property<string>("SuppliersData");
            });

            modelBuilder.Entity<Report>(report =>
            {
                report.HasKey(x => x.Id);
                report.HasIndex(x => x.CustomerId);
                report.Ignore(x => x.Locations).Property<string>("LocationsData");
            });
        }
    }
}