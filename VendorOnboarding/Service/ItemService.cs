using System.Data.SqlClient;
using System.Data;
using VendorOnboarding.Models;
using VendorOnboarding.Interface;

namespace VendorOnboarding.Service
{
    public class ItemService : IItemService
    {
        private readonly IConfiguration _configuration;

        public ItemService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<ItemDetails> CreateItemAsync(ItemDetails itemDto)
        {
            using SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("VendorBoardingConnection"));
            using SqlCommand cmd = new SqlCommand("sp_CreateItem", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@ItemCode", itemDto.ItemCode);
            cmd.Parameters.AddWithValue("@HSNCode", itemDto.HSNCode);
            cmd.Parameters.AddWithValue("@ItemDescription", itemDto.ItemDescription);
            cmd.Parameters.AddWithValue("@Category", itemDto.Category);
            cmd.Parameters.AddWithValue("@UnitOfMeasure", itemDto.UnitOfMeasure);
            cmd.Parameters.AddWithValue("@Price", itemDto.Price);
            cmd.Parameters.AddWithValue("@CostPrice", itemDto.CostPrice);
            cmd.Parameters.AddWithValue("@TaxPercentage", itemDto.TaxPercentage);
            cmd.Parameters.AddWithValue("@ReorderLevel", itemDto.ReorderLevel);
            cmd.Parameters.AddWithValue("@Brand", itemDto.Brand);
            cmd.Parameters.AddWithValue("@Active", itemDto.Active);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new ItemDetails
                {
                    ItemID = Convert.ToInt32(reader["ItemID"]),
                    ItemCode = reader["ItemCode"].ToString(),
                    HSNCode = reader["HSNCode"].ToString(),
                    ItemDescription = reader["ItemDescription"].ToString(),
                    Category = reader["Category"].ToString(),
                    UnitOfMeasure = reader["UnitOfMeasure"].ToString(),
                    Price = Convert.ToDecimal(reader["Price"]),
                    CostPrice = Convert.ToDecimal(reader["CostPrice"]),
                    TaxPercentage = Convert.ToDecimal(reader["TaxPercentage"]),
                    ReorderLevel = Convert.ToDecimal(reader["ReorderLevel"]),
                    Brand = reader["Brand"].ToString(),
                    Active = Convert.ToBoolean(reader["Active"])
                };
            }

            throw new Exception("Item creation failed.");
        }
    }
}
