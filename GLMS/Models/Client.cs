using System.ComponentModel.DataAnnotations;

namespace GLMS.Models
{
    public class Client
    {
        public int ClientId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Region { get; set; } = string.Empty;

        public ICollection<Contract>? Contracts { get; set; }
    }
}