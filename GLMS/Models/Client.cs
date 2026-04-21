using System.ComponentModel.DataAnnotations;

namespace GLMS.Models
{
    public class Client
    {
        [Display(Name = "Client ID")]
        public int ClientId { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Client Name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Phone]
        [Display(Name = "Phone No.")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Display(Name = "Region")]
        public string Region { get; set; } = string.Empty;

        public ICollection<Contract>? Contracts { get; set; }
    }
}