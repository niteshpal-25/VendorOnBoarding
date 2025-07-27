namespace VendorOnboarding.Models
{
    public class VendorDocument
    {
        public int DocumentId { get; set; }
        public int VendorId { get; set; }
        public string DocumentName { get; set; }
        public string DocumentType { get; set; } // CERTIFICATION or CONTRACT
        public string FilePath { get; set; }
        public long FileSize { get; set; }
        public DateTime UploadDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string Status { get; set; } // PENDING, APPROVED, REJECTED
    }
}