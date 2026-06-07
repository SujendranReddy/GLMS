using GLMS.Enums;
using System.ComponentModel.DataAnnotations;

namespace GLMS.DTOs
{
    public class UpdateContractStatusDto
    {
        [Required]
        public ContractStatus Status { get; set; }
    }
}