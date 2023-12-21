using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WhMaSysApi.Models
{
    public class GetProResponse
    {
        /// <summary>
        /// 列表
        /// </summary>
        public List<ProShowModel> List { get; set; }

        /// <summary>
        /// 订单列表
        /// </summary>
        public List<OrderModel> List2 { get; set; }
        public List<PartAfterSaleModel> List3 { get; set; }

        public List<WorkerModel> List4 { get; set; }

        public List<SupplierModel> List5 { get; set; }

        public List<CustomerModel> List6 { get; set; }
        /// <summary>
        /// 总数
        /// </summary>
        public int Total { get; internal set; }
    }
    public class ProShowModel
    {
        public int Id { get; set; }
        
        public string ProductName { get; set; }
        public decimal ProductPrice { get; set; }
        public string ProductStock { get; set; }
        public string Categoryid { get; set; }
        public string Supplierid { get; set; }

    }
    public class OrderModel
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string ItemName { get; set; }
        public string ItemQuantity { get; set; }
        public decimal? ItemPrice { get; set; }
        public decimal? Amount { get; set; }
        public string UserName { get; set; }

        

        public DateTime? OrderTime { get; set; }

        public string Status { get; set; }
        public string Remark { get; set; }
    }
    public class PartAfterSaleModel
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string ItemName { get; set; }
        public string ItemQuantity { get; set; }
        public decimal ItemPrice { get; set; }
        public string UserName { get; set; }
        public DateTime AfterSaleTime { get; set; }
        public string Status { get; set; }
        public string Remark { get; set; }
    }

    public class WorkerModel
    {
        public int Id { get; set; }
        public string Phone { get; set; }
        public string UserName { get; set; }
        public DateTime Time { get; set; }

        public string Role { get; set; }

    }

    public class SupplierModel
    {
        public string Id { get; set; }
        public string SupplierName { get; set; }
        public string SupplierAddress { get; set; }
        public string SupplierPhone { get; set; }
        public DateTime? DateAdded { get; set; }
    }
    public class CustomerModel
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerAddress { get; set; }
        public DateTime? DateAdded { get; set; }
    }
}
