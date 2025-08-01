using System.Data.SqlClient;
using System.Data;
using VendorOnboarding.Models;
using VendorOnboarding.Interface;

namespace VendorOnboarding.Service
{
    public class OrderService : IOrderService
    {
        private readonly IConfiguration _configuration;

        public OrderService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<Order> CreateOrderAsync(Order order)
        {
            using SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("VendorBoardingConnection"));
            using SqlCommand cmd = new SqlCommand("sp_CreateOrder", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@VendorId", order.VendorId);
            cmd.Parameters.AddWithValue("@VendorName", (object)order.VendorName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Amount", order.Amount);
            cmd.Parameters.AddWithValue("@OrderDate", order.OrderDate);

            // Handle the Items collection
            var itemTable = new DataTable();
            itemTable.Columns.Add("ItemId", typeof(int));
            itemTable.Columns.Add("ItemName", typeof(string));
            itemTable.Columns.Add("Quantity", typeof(int));
            itemTable.Columns.Add("Price", typeof(decimal));

            if (order.Items != null)
            {
                foreach (var item in order.Items)
                {
                    itemTable.Rows.Add(item.ItemId, item.ItemName, item.Quantity, item.Price);
                }
            }
            cmd.Parameters.AddWithValue("@Items", itemTable);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                var createdOrder = new Order
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    VendorId = Convert.ToInt32(reader["VendorId"]),
                    VendorName = reader["VendorName"].ToString(),
                    Amount = Convert.ToDecimal(reader["Amount"]),
                    OrderDate = Convert.ToDateTime(reader["OrderDate"]),
                    Items = new List<OrderItem>() // Initialize the list
                };

                // Populate Items if returned by the stored procedure
                if (await reader.NextResultAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        createdOrder.Items.Add(new OrderItem
                        {
                            ItemId = Convert.ToInt32(reader["ItemId"]),
                            ItemName = reader["ItemName"].ToString(),
                            Quantity = Convert.ToInt32(reader["Quantity"]),
                            Price = Convert.ToDecimal(reader["Price"])
                        });
                    }
                }

                return createdOrder;
            }

            throw new Exception("Order creation failed.");
        }
    }
}