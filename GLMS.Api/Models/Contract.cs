using GLMS.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GLMS.Models
{
    public class Contract
    {
        [Display(Name = "Contract ID")]
        public int ContractId { get; set; }

        [Required]
        [Display(Name = "Client")]
        public int ClientId { get; set; }

        [ForeignKey("ClientId")]
        public Client? Client { get; set; }

        [Required]
        [StringLength(500)]
        [Display(Name = "Contract Description")]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Contract Cost (ZAR)")]
        public decimal Cost { get; set; }

        [Required]
        [Display(Name = "Contract Status")]
        public ContractStatus Status { get; set; }

        [Display(Name = "Signed Agreement")]
        public string? SignedAgreementFilePath { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public ICollection<ServiceRequest>? ServiceRequests { get; set; }
    }
}