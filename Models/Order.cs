using System.ComponentModel.DataAnnotations;

namespace EnergyService.Api.Models
{
    public class Order
    {
        public long Id { get; set; }

        public long CustomerId { get; set; }

        [MaxLength(40), Required]
        public string OrderNumber { get; set; } 

        public DateOnly OrderDate { get; set; }
        public DateOnly ActiveDate { get; set; }

        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

        [MaxLength(500)]
        public string? Description { get; set; }


        public Customer Customer { get; set; } = default!;
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
