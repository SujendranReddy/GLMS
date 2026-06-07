using GLMS.Enums;
using System.ComponentModel.DataAnnotations;

namespace GLMS.DTOs
{
    public class UpdateContractDto
    {
        [Required]
        public int ClientId { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Cost must be greater than zero.")]
        public decimal Cost { get; set; }

        [Required]
        public ContractStatus Status { get; set; }

        public string? SignedAgreementFilePath { get; set; }
    }
}