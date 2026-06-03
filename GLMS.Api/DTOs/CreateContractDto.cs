using GLMS.Enums;
using System.ComponentModel.DataAnnotations;

namespace GLMS.DTOs
{
    public class CreateContractDto
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
        public ContractStatus Status { get; set; } = ContractStatus.Draft;

        public string? SignedAgreementFilePath { get; set; }
    }
}