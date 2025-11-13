using Microsoft.EntityFrameworkCore;

namespace EnergyService.Api.Models
{
    public class OrderItem
    {
        public long Id { get; set; }

        public long OrderId { get; set; }

        public long ProductId { get; set; }

        public long TariffId { get; set; } 

        [Precision(12, 3)]
        public decimal EstimatedMonthlyQuantity { get; set; }

        
        
        public Tariff Tariff { get; set; }
        public Product Product { get; set; } = default!;
        public Order Order { get; set; } = default!;
    }
}
