namespace EnergyService.Api.Models
{
    public class InvoiceModel
    {
        public long OrderId { get; set; }
        public string OrderNumber { get; set; }
        public DateOnly? ActiveDate { get; set; }
        public DateOnly? InvoiceDate { get; set; }
        public long CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerAddress { get; set; }
        public decimal InvoiceTotalPrice { get; set; }
        public List<InvoiceItemModel> InvoiceItems { get; set; }

        public InvoiceModel() => InvoiceItems = new List<InvoiceItemModel>();

    }

    public class InvoiceItemModel
    {
        public long OrderItemId { get; set; }
        public long ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal EstimatedMonthlyQuantity { get; set; }
        public string UnitName { get; set; }
        public long TariffId { get; set; }
        public string TariffName { get; set; }
        public decimal TariffMonthlyPrice { get; set; }

    }
}
