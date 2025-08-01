using System.Data;
using System.Data.SqlClient;
using VendorOnboarding.Interface;
using VendorOnboarding.Models;

namespace VendorOnboarding.Service
{
    public class VendorService : IVendorService
    {
        private readonly string _connectionString;
        private readonly string _storagePath;
        public VendorService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("VendorBoardingConnection");
            _storagePath = configuration["Storage:DocumentPath"] ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/documents");
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

        public async Task<VendorDetails> UpdateVendorAsync(VendorDetails vendorDto)
        {
            // Input validation
            if (vendorDto == null)
            {
                throw new ArgumentNullException(nameof(vendorDto));
            }

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand("UpdateVendor", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            // Ensure you pass the VendorID or primary key to identify which vendor to update
            command.Parameters.AddWithValue("@VendorID", vendorDto.VendorId);
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
                throw new Exception("An error occurred while updating the vendor.", ex);
            }
        }


        public async Task<List<VendorDetails>> GetAllVendorsAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand("GetAllVendors", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            try
            {
                var vendors = new List<VendorDetails>();
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    vendors.Add(new VendorDetails
                    {
                        VendorId = reader.GetInt32(reader.GetOrdinal("VendorId")),
                        VendorName = reader.GetString(reader.GetOrdinal("VendorName")),
                        BusinessType = reader.GetString(reader.GetOrdinal("BusinessType")),
                        ContactPerson = reader.GetString(reader.GetOrdinal("ContactPerson")),
                        BusinessAddress = reader.GetString(reader.GetOrdinal("BusinessAddress")),
                        MailingAddress = reader.GetString(reader.GetOrdinal("MailingAddress")),
                        PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber")),
                        EmailAddress = reader.GetString(reader.GetOrdinal("EmailAddress")),
                        Website = reader.GetString(reader.GetOrdinal("Website")),
                        ApprovalStatus = reader.GetString(reader.GetOrdinal("ApprovalStatus"))
                    });
                }

                return vendors;
            }
            catch (SqlException ex)
            {
                // Log the exception (implement proper logging)
                throw new Exception("An error occurred while retrieving vendors.", ex);
            }
        }

        public async Task<VendorDetails> ApproveVendorAsync(int vendorId)
        {
            if (vendorId <= 0)
            {
                throw new ArgumentException("Invalid vendor ID", nameof(vendorId));
            }

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand("ApproveVendor", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@VendorId", vendorId);

            // Output parameter to get updated vendor details
            var outputParam = new SqlParameter("@UpdatedVendor", SqlDbType.Structured)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(outputParam);

            try
            {
                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new VendorDetails
                    {
                        VendorId = reader.GetInt32(reader.GetOrdinal("VendorId")),
                        VendorName = reader.GetString(reader.GetOrdinal("VendorName")),
                        BusinessType = reader.GetString(reader.GetOrdinal("BusinessType")),
                        ContactPerson = reader.GetString(reader.GetOrdinal("ContactPerson")),
                        BusinessAddress = reader.GetString(reader.GetOrdinal("BusinessAddress")),
                        MailingAddress = reader.GetString(reader.GetOrdinal("MailingAddress")),
                        PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber")),
                        EmailAddress = reader.GetString(reader.GetOrdinal("EmailAddress")),
                        Website = reader.GetString(reader.GetOrdinal("Website")),
                        ApprovalStatus = reader.GetString(reader.GetOrdinal("ApprovalStatus"))
                    };
                }

                throw new Exception("Vendor not found or could not be approved.");
            }
            catch (SqlException ex)
            {
                // Log the exception (implement proper logging)
                throw new Exception("An error occurred while approving the vendor.", ex);
            }
        }

        public async Task<VendorDetails> HoldVendorAsync(int vendorId)
        {
            if (vendorId <= 0)
            {
                throw new ArgumentException("Invalid vendor ID", nameof(vendorId));
            }

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand("HoldVendor", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@VendorId", vendorId);

            // Output parameter to get updated vendor details
            var outputParam = new SqlParameter("@UpdatedVendor", SqlDbType.Structured)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(outputParam);

            try
            {
                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new VendorDetails
                    {
                        VendorId = reader.GetInt32(reader.GetOrdinal("VendorId")),
                        VendorName = reader.GetString(reader.GetOrdinal("VendorName")),
                        BusinessType = reader.GetString(reader.GetOrdinal("BusinessType")),
                        ContactPerson = reader.GetString(reader.GetOrdinal("ContactPerson")),
                        BusinessAddress = reader.GetString(reader.GetOrdinal("BusinessAddress")),
                        MailingAddress = reader.GetString(reader.GetOrdinal("MailingAddress")),
                        PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber")),
                        EmailAddress = reader.GetString(reader.GetOrdinal("EmailAddress")),
                        Website = reader.GetString(reader.GetOrdinal("Website")),
                        ApprovalStatus = reader.GetString(reader.GetOrdinal("ApprovalStatus"))
                    };
                }

                throw new Exception("Vendor not found or could not be placed on hold.");
            }
            catch (SqlException ex)
            {
                // Log the exception (implement proper logging)
                throw new Exception("An error occurred while placing the vendor on hold.", ex);
            }
        }


        public async Task<VendorDocument> UploadVendorDocumentAsync(int vendorId, DocumentUploadDto documentDto)
        {
            // Basic validation
            if (documentDto?.Document == null || string.IsNullOrEmpty(documentDto.DocumentName) || string.IsNullOrEmpty(documentDto.DocumentType))
            {
                throw new ArgumentException("Document, name, and type are required.");
            }

            // Validate document type
            string docType = documentDto.DocumentType.ToUpper();
            if (docType != "CERTIFICATION" && docType != "CONTRACT")
            {
                throw new ArgumentException("Document type must be CERTIFICATION or CONTRACT.");
            }

            // Validate file extension and size
            string extension = Path.GetExtension(documentDto.Document.FileName).ToLower();
            if (!new[] { ".pdf", ".doc", ".docx" }.Contains(extension))
            {
                throw new ArgumentException("Only PDF, DOC, and DOCX files are allowed.");
            }
            if (documentDto.Document.Length > 10 * 1024 * 1024) // 10MB
            {
                throw new ArgumentException("File size cannot exceed 10MB.");
            }

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            // Check vendor exists using stored procedure
            using var checkCommand = new SqlCommand("CheckVendorExists", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            checkCommand.Parameters.AddWithValue("@VendorId", vendorId);
            var existsParam = new SqlParameter("@Exists", SqlDbType.Bit) { Direction = ParameterDirection.Output };
            checkCommand.Parameters.Add(existsParam);
            await checkCommand.ExecuteNonQueryAsync();
            if (!(bool)existsParam.Value)
            {
                throw new ArgumentException("Vendor not found.");
            }

            var timestamp = DateTime.UtcNow.Ticks;
            var fileName = $"{vendorId}_{timestamp}{extension}";
            var filePath = Path.Combine(_storagePath, fileName);
            Directory.CreateDirectory(_storagePath);

            try
            {
                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await documentDto.Document.CopyToAsync(stream);
                }

                // Insert document metadata using stored procedure
                using var command = new SqlCommand("InsertVendorDocument", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@VendorId", vendorId);
                command.Parameters.AddWithValue("@DocumentName", documentDto.DocumentName);
                command.Parameters.AddWithValue("@DocumentType", docType);
                command.Parameters.AddWithValue("@FilePath", filePath);
                command.Parameters.AddWithValue("@FileSize", documentDto.Document.Length);
                command.Parameters.AddWithValue("@UploadDate", DateTime.UtcNow);
                command.Parameters.AddWithValue("@ExpiryDate", documentDto.ExpiryDate ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Status", "PENDING");
                var documentIdParam = new SqlParameter("@DocumentId", SqlDbType.Int) { Direction = ParameterDirection.Output };
                command.Parameters.Add(documentIdParam);

                await command.ExecuteNonQueryAsync();
                int documentId = (int)documentIdParam.Value;

                return new VendorDocument
                {
                    DocumentId = documentId,
                    VendorId = vendorId,
                    DocumentName = documentDto.DocumentName,
                    DocumentType = docType,
                    FilePath = filePath,
                    FileSize = documentDto.Document.Length,
                    UploadDate = DateTime.UtcNow,
                    ExpiryDate = documentDto.ExpiryDate,
                    Status = "PENDING"
                };
            }
            catch (Exception ex)
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                throw new Exception("Failed to upload document.", ex);
            }
        }

        public async Task<VendorDocument> GetVendorDocumentAsync(int vendorId, int documentId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand("GetVendorDocument", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@VendorId", vendorId);
            command.Parameters.AddWithValue("@DocumentId", documentId);

            using var reader = await command.ExecuteReaderAsync();
            if (!reader.Read())
            {
                throw new ArgumentException("Document not found.");
            }

            var vendorDocument = new VendorDocument
            {
                DocumentId = reader.GetInt32(reader.GetOrdinal("DocumentId")),
                VendorId = reader.GetInt32(reader.GetOrdinal("VendorId")),
                DocumentName = reader.GetString(reader.GetOrdinal("DocumentName")),
                DocumentType = reader.GetString(reader.GetOrdinal("DocumentType")),
                FilePath = reader.GetString(reader.GetOrdinal("FilePath")),
                FileSize = reader.GetInt64(reader.GetOrdinal("FileSize")),
                UploadDate = reader.GetDateTime(reader.GetOrdinal("UploadDate")),
                ExpiryDate = reader.IsDBNull(reader.GetOrdinal("ExpiryDate")) ? null : reader.GetDateTime(reader.GetOrdinal("ExpiryDate")),
                Status = reader.GetString(reader.GetOrdinal("Status"))
            };

            return vendorDocument;
        }
    }
}
