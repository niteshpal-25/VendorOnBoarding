using System.Data;
using System.Data.SqlClient;
using VendorOnboarding.Models;

namespace VendorOnboarding
{
    public class VendorService : IVendorService
    {
        private readonly string _connectionString;
        public VendorService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("VendorBoardingConnection");
        }
        public async Task<VendorDetails> CreateVendorAsync(VendorDetails vendorDto)
        {
            // Input validation
            if (vendorDto == null)
            {
                throw new ArgumentNullException(nameof(vendorDto));
            }

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand("InsertVendor", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            // Use Add instead of AddWithValue to specify parameter types explicitly
            command.Parameters.AddWithValue("@VendorName", vendorDto.VendorName);
            command.Parameters.AddWithValue("@BusinessType", vendorDto.BusinessType);
            command.Parameters.AddWithValue("@ContactPerson", vendorDto.ContactPerson);
            command.Parameters.AddWithValue("@BusinessAddress", vendorDto.BusinessAddress);
            command.Parameters.AddWithValue("@MailingAddress", vendorDto.MailingAddress);
            command.Parameters.AddWithValue("@PhoneNumber", vendorDto.PhoneNumber);
            command.Parameters.AddWithValue("@EmailAddress", vendorDto.EmailAddress);
            command.Parameters.AddWithValue("@Website", vendorDto.Website);

            try
            {
                await command.ExecuteNonQueryAsync();
                return vendorDto;
            }
            catch (SqlException ex)
            {
                // Log the exception (implement proper logging)
                throw new Exception("An error occurred while creating the vendor.", ex);
            }
        }
    }
}
