using System.ComponentModel.DataAnnotations;

namespace VendorOnboarding.Models
{
    public class VendorDetails
    {
        public int VendorId { get; set; }
        public string VendorName { get; set; }
        public string BusinessType { get; set; }
        public string ContactPerson { get; set; }
        public string BusinessAddress { get; set; }
        public string MailingAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string Website { get; set; }
        public string ApprovalStatus { get; set; }
        public List<VendorDocument> Documents { get; set; } = new List<VendorDocument>();
    }
}
