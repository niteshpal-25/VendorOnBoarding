namespace VendorOnboarding.Models
{
    public class ItemDetails
    {
        public int ItemID { get; set; }
        public string ItemCode { get; set; }
        public string HSNCode { get; set; }
        public string ItemDescription { get; set; }
        public string Category { get; set; }
        public string UnitOfMeasure { get; set; }
        public decimal Price { get; set; }
        public decimal CostPrice { get; set; }
        public decimal TaxPercentage { get; set; }
        public decimal ReorderLevel { get; set; }
        public string Brand { get; set; }
        public bool Active { get; set; }
    }

}
