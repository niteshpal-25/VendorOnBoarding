using VendorOnboarding.Models;
namespace VendorOnboarding.Interface
{
    public interface IOrderService
    {       
            Task<Order> CreateOrderAsync(Order order);        
    }
}
