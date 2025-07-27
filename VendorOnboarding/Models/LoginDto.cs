using VendorOnboarding.Interface;

namespace VendorOnboarding.Models
{
    public class LoginDto : IAuthPayload
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
