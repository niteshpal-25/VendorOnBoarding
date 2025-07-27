namespace VendorOnboarding.Models
{
    public class DocumentUploadDto
    {
        public IFormFile Document { get; set; }
        public string DocumentName { get; set; }
        public string DocumentType { get; set; } // CERTIFICATION or CONTRACT
        public DateTime? ExpiryDate { get; set; }
    }
}
