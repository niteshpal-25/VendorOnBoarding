using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VendorOnboarding.Models;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Cors;
using VendorOnboarding.Interface;

namespace VendorOnboarding.Controllers
{
    [EnableCors("AllowAll")]
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
        public async Task<ActionResult<object>> CreateVendor([FromBody] VendorDetails vendorDto)
        {
            if (vendorDto == null)
            {
                return BadRequest("Vendor data cannot be null.");
            }

            try
            {
                var createdVendor = await _vendorService.CreateVendorAsync(vendorDto);
                var response = new
                {
                    Message = "Vendor record saved successfully.",
                    Data = createdVendor
                };
                return CreatedAtAction(nameof(CreateVendor), new { vendorName = createdVendor.VendorName }, response);
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

        [HttpPut("{vendorId}")]
        public async Task<ActionResult<object>> UpdateVendor(int vendorId, [FromBody] VendorDetails vendorDto)
        {
            if (vendorDto == null || vendorDto.VendorId != vendorId)
            {
                return BadRequest("Vendor data is invalid or mismatched ID.");
            }

            try
            {
                var updatedVendor = await _vendorService.UpdateVendorAsync(vendorDto);
                var response = new
                {
                    Message = "Vendor record updated successfully.",
                    Data = updatedVendor
                };
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the vendor.");
            }
        }

        [HttpGet]
        public async Task<ActionResult<object>> GetAllVendors()
        {
            try
            {
                var vendors = await _vendorService.GetAllVendorsAsync();
                var response = new
                {
                    Message = "Vendors retrieved successfully.",
                    Data = vendors
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                // Log the exception (implement proper logging)
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving vendors.");
            }
        }

        [HttpPut("{vendorId}/approve")]
        public async Task<ActionResult<object>> ApproveVendor(int vendorId)
        {
            if (vendorId <= 0)
            {
                return BadRequest("Invalid vendor ID.");
            }

            try
            {
                var approvedVendor = await _vendorService.ApproveVendorAsync(vendorId);
                var response = new
                {
                    Message = "Vendor approved successfully.",
                    Data = approvedVendor
                };
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                // Log the exception (implement proper logging)
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while approving the vendor.");
            }
        }

        [HttpPut("{vendorId}/hold")]
        public async Task<ActionResult<object>> HoldVendor(int vendorId)
        {
            if (vendorId <= 0)
            {
                return BadRequest("Invalid vendor ID.");
            }

            try
            {
                var heldVendor = await _vendorService.HoldVendorAsync(vendorId);
                var response = new
                {
                    Message = "Vendor placed on hold successfully.",
                    Data = heldVendor
                };
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                // Log the exception (implement proper logging)
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while placing the vendor on hold.");
            }
        }

        [HttpPost("{vendorId}/documents")]
        public async Task<IActionResult> UploadDocument(int vendorId, [FromForm] DocumentUploadDto documentDto)
        {
            try
            {
                var document = await _vendorService.UploadVendorDocumentAsync(vendorId, documentDto);
                return CreatedAtAction(nameof(GetDocument), new { vendorId, documentId = document.DocumentId }, document);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message, code = "INVALID_REQUEST" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while uploading the document.", code = "SERVER_ERROR" });
            }
        }

        [HttpGet("{vendorId}/documents/{documentId}")]
        public async Task<IActionResult> GetDocument(int vendorId, int documentId)
        {
            try
            {
                var document = await _vendorService.GetVendorDocumentAsync(vendorId, documentId);
                return Ok(new
                {
                    document.DocumentId,
                    document.VendorId,
                    document.DocumentName,
                    document.DocumentType,
                    document.UploadDate,
                    document.FileSize,
                    document.Status,
                    DownloadUrl = $"{Request.Scheme}://{Request.Host}/documents/{document.DocumentId}{Path.GetExtension(document.FilePath)}"
                });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { error = ex.Message, code = "DOCUMENT_NOT_FOUND" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while retrieving the document.", code = "SERVER_ERROR" });
            }
        }
    }
}