namespace VendorOnboarding.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int VendorId { get; set; }
        public string VendorName { get; set; } // Add if needed for sorting
        public decimal Amount { get; set; }
        public DateTime OrderDate { get; set; }
        public List<OrderItem> Items { get; set; } // Collection of items in the order
    }

    public class OrderItem
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
