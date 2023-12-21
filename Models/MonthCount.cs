using System.Collections.Generic;

namespace WhMaSysApi.Controllers
{
    public class MonthCount
    {
        public string CategoryName { get; set; }
        public int OrderCount { get; set; }
    }
     
    public class MonthAmount
    {
        public decimal TotalRevenue { get; set; }
        public decimal TotalCost { get; set; }
        public decimal Profit { get; set; }
        public int TotalOrders { get; set; }
        
      
    }
    public class MonthProTotal
    {
        public string ItemName { get; set; }
        public int ItemQuantity { get; set; }
    }
}