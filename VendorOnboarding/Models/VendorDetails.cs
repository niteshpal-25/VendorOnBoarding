using System.ComponentModel.DataAnnotations;

namespace VendorOnboarding.Models
{
    public class VendorDetails
    {
        [Required]
        [StringLength(100)]
        public string VendorName { get; set; }

        [Required]
        [StringLength(50)]
        public string BusinessType { get; set; }

        [Required]
        [StringLength(100)]
        public string ContactPerson { get; set; }

        [Required]
        [StringLength(200)]
        public string BusinessAddress { get; set; }

        [StringLength(200)]
        public string MailingAddress { get; set; }

        [StringLength(50)]
        public string PhoneNumber { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string EmailAddress { get; set; }

        [Url]
        [StringLength(200)]
        public string Website { get; set; }
    }
}
