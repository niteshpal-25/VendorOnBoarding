namespace VendorOnboarding.Models
{
    public class UserDto
    {
        public int UserID { get; set; }
        public string Email { get; set; }
        public string? UserName { get; set; }
        public string? UserType { get; set; }
        public string Pswrd { get; internal set; }
    }
}
