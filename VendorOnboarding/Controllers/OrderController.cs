using Microsoft.AspNetCore.Mvc;
using VendorOnboarding.Interface;
using VendorOnboarding.Models;

namespace VendorOnboarding.Controllers
{    
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost("{vendorId}/orders")]
        public async Task<IActionResult> CreateOrder(int vendorId, [FromBody] Order order)
        {
            if (order == null)
            {
                return BadRequest("Order data is required.");
            }

            order.VendorId = vendorId; // Ensure the vendor ID is set
            var createdOrder = await _orderService.CreateOrderAsync(order);
            return Ok(createdOrder);
        }
    }
}
