using System.ComponentModel.DataAnnotations;
using VendorOnboarding.Interface;

namespace VendorOnboarding.Models
{
    public class RegisterDto: IAuthPayload
    {
        public string Email { get; set; }
        public string Password { get; set; }
        [Required]
        [StringLength(100)]
        public string UserName { get; set; }
    }
}
