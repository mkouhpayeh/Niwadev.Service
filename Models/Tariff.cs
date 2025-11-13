using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace EnergyService.Api.Models
{
    public class Tariff
    {
        public long Id { get; set; }
        public long ProductId { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; } = default!;

        public int UnitId { get; set; }

        [Required]
        public DateOnly EffectiveFrom { get; set; }

        [Required]
        public DateOnly EffectiveTo { get; set; }         // null = no expiration

        [Range(0, 999999), Precision(10, 2)]
        public decimal BaseMonthly { get; set; }           // €/month fixed

        [Range(0, 99), Precision(10, 4)]
        public decimal PricePerUnit { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

        [MaxLength(500)]
        public string? Description { get; set; }


        public Unit Unit { get; set; } = default!;
        public Product Product { get; set; } = default!;
    }
}
