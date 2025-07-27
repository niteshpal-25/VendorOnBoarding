using VendorOnboarding.Models;

namespace VendorOnboarding.Interface
{
    public interface IItemService
    {
        Task<ItemDetails> CreateItemAsync(ItemDetails itemDto);
    }
}
