using System.ComponentModel.DataAnnotations;

namespace EnergyService.Api.Models
{
    public class Product
    {
        public long Id { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; } = default!;      // e.g. "Electricity"

        [MaxLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

        

        public ICollection<Tariff> Tariffs { get; set; } = new List<Tariff>();
    }
}
