namespace EnergyService.Api.Models
{
    public class ResponseModel
    {
        public object? Data { get; set; }
        public required ResponseStatusEnum Status { get; set; }
        public required string Message { get; set; }
    }
}
