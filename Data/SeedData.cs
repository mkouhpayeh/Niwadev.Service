using EnergyService.Api.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace EnergyService.Api.Data
{
    public static class SeedData
    {
        public static void Initialize(AppDBContext db)
        {
            db.Database.Migrate();

            using var tx = db.Database.BeginTransaction();

            try
            {
                // -------------------------------
                // Units
                // -------------------------------
                if (!db.Units.Any())
                {
                    db.Units.AddRange(
                        new Unit
                        {
                            Name = "kWh"
                        },
                        new Unit
                        {
                            Name = "m³"
                        }
                    );
                    db.SaveChanges();
                }

                var kwhUnit = db.Units.First(u => u.Name.Contains("kWh"));
                var m3Unit = db.Units.First(u => u.Name.Contains("m³"));

                // -------------------------------
                // Products
                // -------------------------------
                if (!db.Products.Any())
                {
                    db.Products.AddRange(
                        new Product
                        {
                            Name = "Electricity",
                            Description = "General electrical energy supply"
                        },
                        new Product
                        {
                            Name = "Natural Gas",
                            Description = "Household gas supply"
                        }
                    );
                    db.SaveChanges();
                }

                var electricity = db.Products.First(p => p.Name == "Electricity");
                var gas = db.Products.First(p => p.Name == "Natural Gas");

                // -------------------------------
                // Tariffs
                // -------------------------------
                if (!db.Tariffs.Any())
                {
                    db.Tariffs.AddRange(
                        new Tariff
                        {
                            ProductId = electricity.Id,
                            Name = "Electricity Basic 2025",
                            UnitId = kwhUnit.Id,
                            EffectiveFrom = new DateOnly(2025, 1, 1),
                            EffectiveTo = new DateOnly(2025, 12, 31),
                            BaseMonthly = 9.90m,
                            PricePerUnit = 0.3200m,
                            Description = "Standard electricity tariff for 2025"
                        },
                        new Tariff
                        {
                            ProductId = electricity.Id,
                            Name = "Electricity Pre 2025",
                            UnitId = kwhUnit.Id,
                            EffectiveFrom = new DateOnly(2025, 1, 1),
                            EffectiveTo = new DateOnly(2025, 12, 31),
                            BaseMonthly = 14.90m,
                            PricePerUnit = 0.2900m,
                            Description = "Premium electricity tariff for 2025"
                        },
                        new Tariff
                        {
                            ProductId = gas.Id,
                            Name = "Gas Basic 2025",
                            UnitId = m3Unit.Id,
                            EffectiveFrom = new DateOnly(2025, 1, 1),
                            EffectiveTo = new DateOnly(2025, 12, 31),
                            BaseMonthly = 8.50m,
                            PricePerUnit = 0.0700m,
                            Description = "Standard gas tariff for 2025"
                        },
                        new Tariff
                        {
                            ProductId = gas.Id,
                            Name = "Gas Pre 2025",
                            UnitId = m3Unit.Id,
                            EffectiveFrom = new DateOnly(2025, 1, 1),
                            EffectiveTo = new DateOnly(2025, 12, 31),
                            BaseMonthly = 11.50m,
                            PricePerUnit = 0.0500m,
                            Description = "Premium gas tariff for 2025"
                        }
                    );
                    db.SaveChanges();
                }

                // -------------------------------
                // A sample Customer
                // -------------------------------
                if (!db.Customers.Any())
                {
                    db.Customers.Add(new Customer
                    {
                        Name = "Mahi GmbH",
                        USt_IdNr = "DE123456789",
                        City = "Wemding",
                        ZipCode = "86650",
                        Address = "Musterstrasse 1",
                        Email = "info@mahi.de",
                        Phone = "+49 12 1234567",
                        Description = "A customer from Wemding"
                    });
                    db.SaveChanges();
                }

                var customer = db.Customers.First();

                // -------------------------------
                // Example Order + Detail
                // -------------------------------
                if (!db.Orders.Any())
                {
                    var product = db.Products.First(p => p.IsActive);
                    var tariff = db.Tariffs
                        .Include(t => t.Unit)
                        .First(t => t.ProductId == product.Id && t.IsActive);

                    var order = new Order
                    {
                        CustomerId = customer.Id,
                        OrderNumber = "ORD-20251111-0001",
                        OrderDate = DateOnly.FromDateTime(DateTime.UtcNow),
                        ActiveDate = DateOnly.Parse("2025-01-01"),
                        Description = "Sample initial order for demo",
                        IsActive = true,
                        CreatedUtc = DateTime.UtcNow
                    };

                    decimal estimatedYearly = 2400m;            // e.g., 2400 kWh per year
                    decimal estimatedMonthly = estimatedYearly / 12m;  // = 200 kWh
                    decimal lineMonthly = tariff.BaseMonthly + (tariff.PricePerUnit * estimatedMonthly);

                    var item = new OrderItem
                    {
                        ProductId = product.Id,
                        TariffId = tariff.Id,
                        EstimatedMonthlyQuantity = Math.Round(estimatedMonthly, 3)
                    };

                    order.OrderItems.Add(item);
                    db.Orders.Add(order);

                    db.SaveChanges();
                }

                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }
    }
}
