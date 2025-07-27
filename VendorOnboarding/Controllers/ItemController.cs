using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VendorOnboarding.Interface;
using VendorOnboarding.Models;

namespace VendorOnboarding.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {

        private readonly IItemService _ItemService;

        public ItemController(IItemService ItemService)
        {
            _ItemService = ItemService ?? throw new ArgumentNullException(nameof(ItemService));
        }

        [HttpPost("create-item")]
        public async Task<ActionResult<object>> CreateItem([FromBody] ItemDetails itemDto)
        {
            if (itemDto == null)
                return BadRequest("Item data cannot be null.");

            try
            {
                var createdItem = await _ItemService.CreateItemAsync(itemDto);
                var response = new
                {
                    Message = "Item saved successfully.",
                    Data = createdItem
                };
                return CreatedAtAction(nameof(CreateItem), new { id = createdItem.ItemID }, response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while saving the item.");
            }
        }
    }
}
