using System;
using System.Collections.Generic;

#nullable disable

namespace WhMaSysApi.Models.Database
{
    public partial class NewProduct
    {
        public int Id { get; set; }
        public int? ProId { get; set; }
        public string ProductName { get; set; }
        public decimal? NewProductPrice { get; set; }
        public decimal? ProductPrice { get; set; }
        public string ProductStock { get; set; }
        public string Categoryid { get; set; }
        public string Supplierid { get; set; }
    }
}
