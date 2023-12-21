using System;
using System.Collections.Generic;

#nullable disable

namespace WhMaSysApi.Models.Database
{
    public partial class BatteryAfterSale
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string ItemName { get; set; }
        public int ItemQuantity { get; set; }
        public decimal ItemPrice { get; set; }
        public string UserName { get; set; }
        public DateTime AfterSaleTime { get; set; }
        public string Status { get; set; }
        public string Remark { get; set; }
    }
}
