using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GLMS.Models
{
    public class ServiceRequest
    {
        [Display(Name = "Request ID")]
        public int ServiceRequestId { get; set; }

        [Required]
        [Display(Name = "Contract")]
        public int ContractId { get; set; }

        [ForeignKey("ContractId")]
        public Contract? Contract { get; set; }

        [Required]
        [StringLength(500)]
        [Display(Name = "Service Description")]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Cost (USD)")]
        public decimal CostUSD { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Cost (ZAR)")]
        public decimal CostZAR { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}