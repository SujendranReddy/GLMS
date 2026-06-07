using System.ComponentModel.DataAnnotations;

namespace GLMS.DTOs
{
    public class UpdateServiceRequestDto
    {
        [Required]
        public int ContractId { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Cost must be greater than zero.")]
        public decimal CostUSD { get; set; }
    }
}