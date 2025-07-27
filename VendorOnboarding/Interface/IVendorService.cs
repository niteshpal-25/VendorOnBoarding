using VendorOnboarding.Models;

namespace VendorOnboarding.Interface
{
    public interface IVendorService
    {
        Task<VendorDetails> CreateVendorAsync(VendorDetails vendorDto);
        Task<VendorDetails> UpdateVendorAsync(VendorDetails vendorDto);

        Task<List<VendorDetails>> GetAllVendorsAsync();
        Task<VendorDetails> ApproveVendorAsync(int vendorId);

        Task<VendorDocument> UploadVendorDocumentAsync(int vendorId, DocumentUploadDto documentDto);
        Task<VendorDocument> GetVendorDocumentAsync(int vendorId, int documentId);
        Task<VendorDetails> HoldVendorAsync(int vendorId);
    }
}
