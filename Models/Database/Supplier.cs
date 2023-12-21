using System;
using System.Collections.Generic;

#nullable disable

namespace WhMaSysApi.Models.Database
{
    public partial class Supplier
    {
        public int Id { get; set; }
        public string SupId { get; set; }
        public string SupplierName { get; set; }
        public string SupplierAddress { get; set; }
        public string SupplierPhone { get; set; }
        public DateTime? DateAdded { get; set; }
    }
}
