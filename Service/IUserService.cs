using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WhMaSysApi.Models;
using WhMaSysApi.Models.Database;

namespace WhMaSysApi.Service
{
    public interface IUserService
    {
        bool TfUserPhone(string Phone);
        string Reg(string Phone, string password, string username);
        User GetphoneByphone(string phone);

        GetProResponse GetGoodShow(string Keyword, int? Proid, int? Cateid, int PageIndex, int PageSize);
        string Addpro(NewProduct newproduct);
        string Updatepro(int Id, string name, decimal price, string category_id, string stock, string Supplierid);
        string DelProduct(int id);
        List<Customer> Customername(string customername);
        //int AddOrder(string CustomerName, string ItemName, int ItemQuantity, decimal ItemPrice, string UserName, string Status, string Remark);

        string AddOrders(List<Order> orders);
        int AddPartAfterSale(List<PartAfterSale> PartAfterSales);
        //GetProResponse GetOrderShow(int id, string itemName, DateTime orderTime, string customerName, string userName, int pageIndex, int pageSize);
        GetProResponse GetOrderShow(int? Id, string ItemName, DateTime OrderTime, string CustomerName, string UserName, int PageIndex, int PageSize);
        string UpdateOrder(int Id, string CustomerName, string ItemName, string ItemQuantity, decimal ItemPrice, string UserName, string Status, string Remark);
        string DelOrder(int id);
        GetProResponse GetPartAfterSale(int? Id, string ItemName, DateTime OrderTime, string CustomerName, string UserName, int PageIndex, int PageSize);

        string UpdateSale(int Id, string CustomerName, string ItemName, string ItemQuantity, decimal ItemPrice, string UserName, string Status, string Remark);
        string DelSale(int id);
        GetProResponse GetWorker(string Phone, string UserName, int PageIndex, int PageSize);
        string UpdateWorker(int Id, string UserName, string Phone,string Role, string Sex, DateTime? Birthday, string Nation, string Idnumber, string Education, string Address);
        string DelWorker(int id);
    }
}
