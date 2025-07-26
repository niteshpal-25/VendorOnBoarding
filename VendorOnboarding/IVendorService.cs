using VendorOnboarding.Models;

namespace VendorOnboarding
{
    public interface IVendorService
    {
        Task<VendorDetails> CreateVendorAsync(VendorDetails vendorDto);
    }
}
