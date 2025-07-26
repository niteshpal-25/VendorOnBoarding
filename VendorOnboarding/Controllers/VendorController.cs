using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VendorOnboarding.Models;

namespace VendorOnboarding.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VendorController : ControllerBase
    {
        private readonly IVendorService _vendorService;
        public VendorController(IVendorService vendorService)
        {
            _vendorService = vendorService ?? throw new ArgumentNullException(nameof(vendorService));
        }

        [HttpPost]
        public async Task<ActionResult<VendorDetails>> CreateVendor([FromBody] VendorDetails vendorDto)
        {
            if (vendorDto == null)
            {
                return BadRequest("Vendor data cannot be null.");
            }
            try
            {
                var createdVendor = await _vendorService.CreateVendorAsync(vendorDto);
                return CreatedAtAction(nameof(CreateVendor), new { vendorName = createdVendor.VendorName }, createdVendor);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                // Log the exception (implement proper logging)
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating the vendor.");
            }
        }
    }
}
