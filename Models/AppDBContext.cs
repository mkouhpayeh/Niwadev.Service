using Microsoft.EntityFrameworkCore;

namespace EnergyService.Api.Models
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options) { }

        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Tariff> Tariffs => Set<Tariff>();
        public DbSet<Unit> Units => Set<Unit>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            // -----------------------
            // Global Query Filter
            // -----------------------
            b.Entity<Customer>().HasQueryFilter(c => c.IsActive);
            b.Entity<Product>().HasQueryFilter(c => c.IsActive);
            b.Entity<Tariff>().HasQueryFilter(c => c.IsActive);
            b.Entity<Order>().HasQueryFilter(c => c.IsActive);


            // -----------------------
            // Customer
            // -----------------------
            b.Entity<Customer>(e =>
            {
                e.ToTable("Customer");

                e.Property(p => p.CreatedUtc)
                    .HasDefaultValueSql("SYSUTCDATETIME()");

                e.Property(p => p.IsActive)
                    .HasDefaultValue(true);

                e.HasIndex(p => p.Name);
                e.HasIndex(p => p.USt_IdNr);
                e.HasIndex(p => p.Email)
                    .IsUnique()
                    .HasFilter("[Email] IS NOT NULL");
            });

            // -----------------------
            // Product
            // -----------------------
            b.Entity<Product>(e =>
            {
                e.ToTable("Product");

                e.Property(p => p.CreatedUtc)
                    .HasDefaultValueSql("SYSUTCDATETIME()");

                e.Property(p => p.IsActive)
                    .HasDefaultValue(true);

                e.HasIndex(p => p.Name);
            });

            // -----------------------
            // Tariff
            // -----------------------
            b.Entity<Tariff>(e =>
            {
                e.ToTable("Tariff", tb =>
                {
                    tb.HasCheckConstraint("CK_Tariff_Range",
                        "(EffectiveFrom IS NOT NULL AND EffectiveTo IS NOT NULL AND EffectiveFrom < EffectiveTo)");
                });

                e.Property(p => p.CreatedUtc).HasDefaultValueSql("SYSUTCDATETIME()");
                e.Property(p => p.IsActive).HasDefaultValue(true);
                e.Property(p => p.BaseMonthly).HasColumnType("decimal(10,2)");
                e.Property(p => p.PricePerUnit).HasColumnType("decimal(10,4)");

                e.HasOne(t => t.Product)
                 .WithMany(p => p.Tariffs)
                 .HasForeignKey(t => t.ProductId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(t => t.Unit)
                 .WithMany()
                 .HasForeignKey(t => t.UnitId)
                 .OnDelete(DeleteBehavior.Restrict);


                e.HasIndex(t => new { t.ProductId, t.Name, t.EffectiveFrom }).IsUnique();
                e.HasIndex(t => new { t.ProductId, t.Name, t.IsActive });
                e.HasIndex(t => new { t.ProductId, t.Name, t.EffectiveTo });

            });

            // -----------------------
            // Unit
            // -----------------------
            b.Entity<Unit>(e =>
            {
                e.ToTable("Unit");

                e.HasIndex(u => u.Name).IsUnique(false);
            });

            // -----------------------
            // Order
            // -----------------------
            b.Entity<Order>(e =>
            {
                e.ToTable("Order");

                e.Property(p => p.CreatedUtc)
                    .HasDefaultValueSql("SYSUTCDATETIME()");

                e.Property(p => p.IsActive)
                    .HasDefaultValue(true);

                e.HasIndex(p => p.OrderNumber)
                    .IsUnique()
                    .HasFilter("[OrderNumber] IS NOT NULL");

                e.HasOne(o => o.Customer)
                    .WithMany()
                    .HasForeignKey(o => o.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // -----------------------
            // OrderDetail
            // -----------------------
            b.Entity<OrderItem>(e =>
            {
                e.ToTable("OrderItem");

                e.Property(p => p.EstimatedMonthlyQuantity)
                    .HasColumnType("decimal(12,3)");

                e.HasOne(od => od.Order)
                    .WithMany(o => o.OrderItems)
                    .HasForeignKey(od => od.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(od => od.Product)
                    .WithMany()
                    .HasForeignKey(od => od.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(od => od.Tariff)
                    .WithMany()
                    .HasForeignKey(od => od.TariffId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
