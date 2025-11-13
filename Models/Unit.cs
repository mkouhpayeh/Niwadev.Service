using System.ComponentModel.DataAnnotations;

namespace EnergyService.Api.Models
{
    public class Unit
    {
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; } = default!;

    }
}
