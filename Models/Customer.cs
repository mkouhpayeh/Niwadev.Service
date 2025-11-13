using System.ComponentModel.DataAnnotations;

namespace EnergyService.Api.Models
{
    public class Customer
    {
        public long Id { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; } = default!;   // Assumes that all Customers are Companies without any data of Persons(FN, LN)

        [MaxLength(32)]
        [RegularExpression(@"^DE[0-9]{9}$", ErrorMessage = "Invalid German VAT ID format.")]
        public string? USt_IdNr { get; set; }  // Umsatzsteuer-Identifikationsnummer

        [Required, MaxLength(200)]
        public string City { get; set; } = default!;

        [Required, MaxLength(5)]
        public string ZipCode { get; set; } = default!; // Assumes that all addresses are in DE

        [Required, MaxLength(400)]
        public string Address { get; set; } = default!;

        [MaxLength(320)] // RFC limit
        [EmailAddress]
        public string? Email { get; set; }

        [MaxLength(32)]
        public string? Phone { get; set; } 

        [MaxLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;


    }
}
