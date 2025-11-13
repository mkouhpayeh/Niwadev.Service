namespace EnergyService.Api.Models
{
    public class OrderRequestModel
    {
        public long CustomerId { get; set; }
        public DateOnly? ActiveDate { get; set; }   
        public List<OrderItemRequest> Items { get; set; } = new();
    }

    public sealed class OrderItemRequest
    {
        public long ProductId { get; set; }
        public long TariffId { get; set; }               
        public decimal EstimatedYearlyQuantity { get; set; } 
    }
}
