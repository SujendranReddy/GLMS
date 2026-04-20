using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GLMS.Models
{
    public class ServiceRequest
    {
        public int ServiceRequestId { get; set; }

        [Required]
        public int ContractId { get; set; }

        [ForeignKey("ContractId")]
        public Contract? Contract { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal CostUSD { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal CostZAR { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}