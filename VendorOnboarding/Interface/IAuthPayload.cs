using System.ComponentModel.DataAnnotations;

namespace VendorOnboarding.Interface
{
    public interface IAuthPayload
    {
        [Required]
        [EmailAddress]
        string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        string Password { get; set; }
    }
}
