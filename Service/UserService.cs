using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WhMaSysApi.Models;
using WhMaSysApi.Models.Database;

namespace WhMaSysApi.Service
{
    public class UserService: IUserService
    {
        private readonly WhMaSysContext _db;

        public  UserService(WhMaSysContext db)
        {
            _db = db;
        }
        /// <summary>
        /// 判断手机号是否存在
        /// </summary>
        /// <param name="UserName"></param>
        /// <returns></returns>
        public bool TfUserPhone(string Phone)
        {
            return _db.Users.Any(x => x.Phone == Phone);
        }
      

        public string Reg(string Phone, string username, string password)
        {
            
            //随机数1000-9999
            var salt = new Random().Next(1000, 9999);

            //123456+随机值   “E1ADC3949BA59ABBE56E057F2F883E”  //结果变化
            //222222    "E1xxxxxxxxxxxxxxxxxxxxxxxxxxxx"   //所有的组合记录下来
            //在对密码进行MD5 md5(密码+随机数)

            var md5 = MD5Helper.MD5Encrypt32(password + salt.ToString());
            var user = new User()
            {
                Phone=Phone,
                UserName=username,
                Password = md5,
                Time = DateTime.Now,
                Salt = salt.ToString(),
                Role="业务员"

            };
            //上下文.表名.操作方法(需要插入的数据对象)
            _db.Users.Add(user);
            var rows = _db.SaveChanges();
            
            if (rows > 0)
            {
                return user.UserName;
            }
            return "注册失败";

        }

        public User GetphoneByphone(string phone)
        {
            return _db.Users.FirstOrDefault(x => x.Phone == phone);
        }

        public GetProResponse GetGoodShow(string Keyword,int? Proid,int? Cateid,  int PageIndex, int PageSize)
        {
            //查询所有数据
            var query = _db.Products.AsQueryable();

            //根据条件查询
            
                //query = query.Where(x => x.ProductName.Contains(Keyword));
           query = query.Where(x =>
               (string.IsNullOrEmpty(Keyword) || x.ProductName.Contains(Keyword)) &&
               (Proid == null || x.ProId==Proid) &&
               (Cateid == null || x.Categoryid == Convert.ToString(Cateid))
           );

            
            var total = query.Count();
            var list = query.OrderByDescending(x => x.ProductPrice).Skip((PageIndex - 1) * PageSize).Take(PageSize)
                .Join(_db.ProductCates,o=>o.Categoryid,y=>y.Cateid,(o,y)=>new { Product=o,ProductCate=y })
                .Join(_db.Suppliers,oy=>oy.Product.Supplierid,z=>z.SupId,(oy,z)=>new { oy.Product,oy.ProductCate,Supplier=z})
                //形成自定义类
                .Select(x => new ProShowModel
                {
                    Id = (int)x.Product.ProId,
                    ProductName=x.Product.ProductName,
                    ProductPrice=x.Product.ProductPrice,
                    ProductStock=x.Product.ProductStock,
                    Categoryid=x.ProductCate.CateName,
                    Supplierid=x.Supplier.SupplierName
                    

                })
                .ToList();
            //构造返回对象
            var response = new GetProResponse()
            {
                List = list,
                Total = total
            };
            return response;
        }
        public string Addpro(NewProduct newproduct)
        {
            var item = _db.Products.FirstOrDefault(x => x.ProId == newproduct.ProId );
            var items= _db.Products.FirstOrDefault(x => x.ProductName == newproduct.ProductName);
            var itempro = _db.Products.FirstOrDefault(x => x.ProId == newproduct.ProId && x.ProductName == newproduct.ProductName);
            if (itempro != null )
            {
                
                item.ProductPrice = (decimal)newproduct.ProductPrice;

                // 更新 ProductStock
                if (int.TryParse(item.ProductStock, out int currentStock) && int.TryParse(newproduct.ProductStock, out int newStock))
                {
                    // 使用显式类型转换将整数相加，并将结果转换为字符串
                    item.ProductStock = (currentStock + newStock).ToString();
                }
                _db.Products.Update(item);
            }
            else
            {
                if(item != null)
                {
                    return "商品编号重复";
                }
                if (items != null)
                {
                    return "商品名称重复";
                }
                else
                {
                    var products = new Product()
                    {
                        ProId = newproduct.ProId,
                        ProductName = newproduct.ProductName,
                        ProductPrice = (decimal)newproduct.ProductPrice,
                        ProductStock = newproduct.ProductStock,
                        Categoryid = newproduct.Categoryid,
                        Supplierid = newproduct.Supplierid
                    };
                    _db.Products.Add(products);
                }
                
            }
            _db.NewProducts.Add(newproduct);
            var row = _db.SaveChanges();
            if (row > 0)
            {
                return "添加成功";
            }
            return "添加失败";
        }
        //public string AddProduct(NewProduct newProduct)
        //{
        //    // 先检查数据库中是否已存在相同的商品ID
        //    var existingProduct = _db.Products.FirstOrDefault(p => p.Id == newProduct.ProId);

        //    if (existingProduct != null)
        //    {
        //        // 如果存在相同的商品ID，更新价格和增加库存
        //        existingProduct.ProductPrice = newProduct.NewProductPrice ?? existingProduct.ProductPrice;
        //        existingProduct.ProductStock += newProduct.ProductStock ;

        //        // 更新数据库
        //        _db.Products.Update(existingProduct);
        //    }
        //    else
        //    {
        //        // 如果不存在相同的商品ID，将新商品添加到商品表
        //        var newProductEntry = new Product
        //        {
        //            ProductName = newProduct.ProductName,
        //            ProductPrice = newProduct.NewProductPrice ?? 0,
        //            ProductStock = newProduct.ProductStock ,
        //            Categoryid = newProduct.Categoryid,
        //            Supplierid = newProduct.Supplierid
        //        };

        //        _db.Products.Add(newProductEntry);
        //    }

        //    // 将新货物添加到来货单表
        //    _db.NewProducts.Add(newProduct);

        //    // 保存更改
        //    var rowsAffected = _db.SaveChanges();

        //    // 根据受影响的行数判断是否成功
        //    return rowsAffected > 0 ? "添加成功" : "添加失败";
        //}

        public string Updatepro(int Id, string name, decimal price, string category_id, string stock, string Supplierid)
        {
            var product = _db.Products.FirstOrDefault(x => x.ProId == Id);
            if (product == null)
                return "没有该商品";

            product.ProductName = name;
            product.ProductPrice = price;
            product.ProductStock = stock;
            product.Categoryid = category_id;
            product.Supplierid = Supplierid;
               

            var row = _db.SaveChanges();
            if (row > 0)
            {
                return "更新成功";
            }
            return "更新失败";

        }
        public string DelProduct(int id)
        {

            Console.WriteLine($"Received ID: {id}");


            var product = _db.Products.FirstOrDefault(x => x.ProId == id);
            if (product == null)
                return "没有该商品";

            _db.Products.Remove(product);
            var row = _db.SaveChanges();
            if (row > 0)
            {
                return "删除成功";
            }
            return "删除失败";
        }
        /// <summary>
        /// 模糊查询客户名
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public List<Customer> Customername(string customername)
        {
            return _db.Customers.Where(x=>x.CustomerName.Contains(customername)).ToList();

        }


        //public int AddOrders(List<Order> orders)
        //{
        //    foreach (var order in orders)
        //    {
        //        // 为每个订单设置 OrderTime
        //        order.OrderTime = DateTime.Now;
        //       // order.Amount = order.ItemPrice * Convert.ToInt32(order.ItemQuantity);
        //        _db.Orders.Add(order);
        //        // 更新产品库存
        //        var product = _db.Products.FirstOrDefault(x => x.ProductName == order.ItemName);
        //        if (product != null)
        //        {
        //            // 检查库存是否足够
        //            int orderedQuantity = Convert.ToInt32(order.ItemQuantity);
        //            int ProductStock = Convert.ToInt32(product.ProductStock);
        //            if (Convert.ToInt32(product.ProductStock) >= orderedQuantity)
        //            {
        //                // 减少库存
        //                ProductStock -= orderedQuantity;
        //            }
        //            else
        //            {
        //                // 库存不足
        //                throw new InvalidOperationException("库存不足");
        //            }
        //        }
        //        else
        //        {

        //            throw new InvalidOperationException("找不到对应的产品");
        //        }
        //    }

        //    var rowsAffected = _db.SaveChanges();

        //    if (rowsAffected > 0)
        //    {
        //        // 成功添加订单
        //        return rowsAffected;
        //    }

        //    // 未能添加订单
        //    return 0;
        //}




        public string AddOrders(List<Order> orders)
        {
            foreach (var order in orders)
            {
                // 为每个订单设置 OrderTime
                order.OrderTime = DateTime.Now;
                // order.Amount = order.ItemPrice * Convert.ToInt32(order.ItemQuantity);
                _db.Orders.Add(order);
                // 更新产品库存
                var product = _db.Products.FirstOrDefault(x => x.ProductName == order.ItemName);
                if (product != null)
                {
                    // 检查库存是否足够
                    int orderedQuantity = Convert.ToInt32(order.ItemQuantity);
                    int ProductStock = Convert.ToInt32(product.ProductStock);
                    if (Convert.ToInt32(product.ProductStock) >= orderedQuantity)
                    {
                        // 减少库存
                        ProductStock -= orderedQuantity;
                        product.ProductStock = Convert.ToString(ProductStock);
                    }
                    else
                    {
                        // 库存不足
                        return "库存不足";
                    }
                }
                else
                {

                    throw new InvalidOperationException("找不到对应的产品");
                }
            }

            var rowsAffected = _db.SaveChanges();

            if (rowsAffected > 0)
            {
                // 成功添加订单
                return "添加成功";
            }

            // 未能添加订单
            return "添加失败";
        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="PartAfterSales"></param>
        /// <returns></returns>
        public int AddPartAfterSale(List<PartAfterSale> PartAfterSales)
        {
            foreach (var order in PartAfterSales)
            {
                // 为每个订单设置 OrderTime
                order.AfterSaleTime = DateTime.Now;
                // order.Amount = order.ItemPrice * Convert.ToInt32(order.ItemQuantity);
                _db.PartAfterSales.Add(order);
            }

            var rowsAffected = _db.SaveChanges();

            if (rowsAffected > 0)
            {
                // 成功添加订单
                return rowsAffected;
            }

            // 未能添加订单
            return 0;
        }
        /// <summary>
        /// 分页查询订单
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="ItemName"></param>
        /// <param name="OrderTime"></param>
        /// <param name="CustomerName"></param>
        /// <param name="UserName"></param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        public GetProResponse GetOrderShow(int? Id, string ItemName, DateTime OrderTime, string CustomerName, string UserName, int PageIndex, int PageSize)
        {
            //查询所有数据
            var query = _db.Orders.AsQueryable();

            
            // 根据条件联合查询
            query = query.Where(x =>
                (string.IsNullOrEmpty(ItemName) || x.ItemName.Contains(ItemName)) &&
                (string.IsNullOrEmpty(CustomerName) || x.CustomerName.Contains(CustomerName)) &&
                (string.IsNullOrEmpty(UserName) || x.UserName.Contains(UserName)) &&
                (Id == null  || x.Id == Id) && (OrderTime==null || OrderTime== DateTime.MinValue || x.OrderTime.Date == OrderTime)
            ); 

            var total = query.Count();
            var list = query
                .OrderByDescending(x => x.OrderTime) .Skip((PageIndex - 1) * PageSize).Take(PageSize)
                .Select(x => new OrderModel
                {
                    Id=x.Id,
                    CustomerName=x.CustomerName,
                    ItemName=x.ItemName,
                    ItemQuantity=x.ItemQuantity,
                    ItemPrice=x.ItemPrice,
                    Amount=x.Amount,
                    UserName=x.UserName,
                    OrderTime=x.OrderTime,
                    Status=x.Status,
                    Remark=x.Remark
                })
                .ToList();
            ///构造返回对象
            var response = new GetProResponse()
            {
                List2 = list,
                Total = total
            };
            return response;
        }
        /// <summary>
        /// 修改订单
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="CustomerName"></param>
        /// <param name="ItemName"></param>
        /// <param name="ItemQuantity"></param>
        /// <param name="ItemPrice"></param>
        /// <param name="UserName"></param>
        /// <param name="Status"></param>
        /// <param name="Remark"></param>
        /// <returns></returns>
        public string UpdateOrder(int Id, string CustomerName, string ItemName, string ItemQuantity, decimal ItemPrice, string UserName, string Status, string Remark)
        {
            var item = _db.Orders.FirstOrDefault(x => x.Id == Id);
            if (item == null)
                return "没有该订单";

            item.CustomerName = CustomerName;
            item.ItemName = ItemName;
            item.ItemQuantity = ItemQuantity;
            item.ItemPrice = ItemPrice;
            item.Amount = Convert.ToInt32(ItemQuantity) * ItemPrice;
            item.UserName = UserName;
            item.Status = Status;
            item.Remark = Remark;


            var row = _db.SaveChanges();
            if (row > 0)
            {
                return "更新成功";
            }
            return "更新失败";

        }
        /// <summary>
        /// 删除订单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string DelOrder(int id)
        {
            var Order = _db.Orders.FirstOrDefault(x => x.Id == id);
            if (Order == null)
                return "没有该订单";

            _db.Orders.Remove(Order);
            var row = _db.SaveChanges();
            if (row > 0)
            {
                return "删除成功";
            }
            return "删除失败";
        }
        /// <summary>
        /// 分页查询售后单
        /// </summary>
        /// <param name="Keyword"></param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        public GetProResponse GetPartAfterSale(int? Id, string ItemName, DateTime OrderTime, string CustomerName, string UserName, int PageIndex, int PageSize)
        {
            //查询所有数据
            var query = _db.PartAfterSales.AsQueryable();

            query = query.Where(x =>
               (string.IsNullOrEmpty(ItemName) || x.ItemName.Contains(ItemName)) &&
               (string.IsNullOrEmpty(CustomerName) || x.CustomerName.Contains(CustomerName)) &&
               (string.IsNullOrEmpty(UserName) || x.UserName.Contains(UserName)) &&
               (Id == null || x.Id == Id) && (OrderTime == null || OrderTime == DateTime.MinValue || x.AfterSaleTime.Date == OrderTime)
           );
            var total = query.Count();
            var list = query.OrderByDescending(x => x.AfterSaleTime).Skip((PageIndex - 1) * PageSize).Take(PageSize).Select(x => new PartAfterSaleModel
                {
                    Id = x.Id,
                    CustomerName = x.CustomerName,
                    ItemName = x.ItemName,
                    ItemQuantity = x.ItemQuantity,
                    ItemPrice = x.ItemPrice,
                    UserName = x.UserName,
                    AfterSaleTime=x.AfterSaleTime,
                    Status = x.Status,
                    Remark = x.Remark
                })
                .ToList();
            ///构造返回对象
            var response = new GetProResponse()
            {
                List3 = list,
                Total = total
            };
            return response;
        }

        /// <summary>
        /// 模糊查询工作人员
        /// </summary>
        /// <param name="Phone"></param>
        /// <param name="UserName"></param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        public GetProResponse GetWorker(string Phone,string UserName, int PageIndex, int PageSize)
        {
            //查询所有数据
            var query = _db.Users.AsQueryable();

            query = query.Where(x =>
               (string.IsNullOrEmpty(UserName) || x.UserName.Contains(UserName)) &&
                (string.IsNullOrEmpty(Phone) || x.Phone.Contains(Phone))

           );
            var total = query.Count();
            var list = query.OrderByDescending(x => x.Time).Skip((PageIndex - 1) * PageSize).Take(PageSize).Select(x => new WorkerModel
            {
                Id = x.Id,
                Phone=x.Phone,
                UserName = x.UserName,
                Role=x.Role,
                Time=x.Time  

            })
                .ToList();
            ///构造返回对象
            var response = new GetProResponse()
            {
                List4 = list,
                Total = total
            };
            return response;
        }


        /// <summary>
        /// 修改配件售后单
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="CustomerName"></param>
        /// <param name="ItemName"></param>
        /// <param name="ItemQuantity"></param>
        /// <param name="ItemPrice"></param>
        /// <param name="UserName"></param>
        /// <param name="Status"></param>
        /// <param name="Remark"></param>
        /// <returns></returns>
        public string UpdateSale(int Id, string CustomerName, string ItemName, string ItemQuantity, decimal ItemPrice, string UserName, string Status, string Remark)
        {
            var item = _db.PartAfterSales.FirstOrDefault(x => x.Id == Id);
            if (item == null)
                return "没有该订单";

            item.CustomerName = CustomerName;
            item.ItemName = ItemName;
            item.ItemQuantity = ItemQuantity;
            item.ItemPrice = ItemPrice;
            item.Amount = Convert.ToInt32(ItemQuantity) * ItemPrice;
            item.UserName = UserName;
            item.Status = Status;
            item.Remark = Remark;


            var row = _db.SaveChanges();
            if (row > 0)
            {
                return "更新成功";
            }
            return "更新失败";

        }
        /// <summary>
        /// 修改工作人员信息
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="UserName"></param>
        /// <param name="Phone"></param>
        /// <param name="Password"></param>
        /// <param name="Role"></param>
        /// <returns></returns>
        public string UpdateWorker(int Id, string UserName,string Phone,  string Role, string Sex, DateTime? Birthday, string Nation, string Idnumber, string Education, string Address)
        {
            var item = _db.Users.FirstOrDefault(x => x.Id == Id);
            if (item == null)
                return "没有该人员";

            item.UserName = UserName;
            item.Phone = Phone;
            item.Role = Role;
            item.Sex = Sex;
            item.Birthday = Birthday;
            item.Nation = Nation;
            item.Idnumber = Idnumber;
            item.Education = Education;
            item.Address = Address;
         

            var row = _db.SaveChanges();
            if (row > 0)
            {
                return "更新成功";
            }
            return "更新失败";

        }
        /// <summary>
        /// 删除售后单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string DelSale(int id)
        {
            var Order = _db.PartAfterSales.FirstOrDefault(x => x.Id == id);
            if (Order == null)
                return "没有该订单";

            _db.PartAfterSales.Remove(Order);
            var row = _db.SaveChanges();
            if (row > 0)
            {
                return "删除成功";
            }
            return "删除失败";
        }
        /// <summary>
        /// 删除工作人员
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string DelWorker(int id)
        {
            var worker = _db.Users.FirstOrDefault(x => x.Id == id);
            if (worker == null)
                return "没有该人员";

            _db.Users.Remove(worker);
            var row = _db.SaveChanges();
            if (row > 0)
            {
                return "删除成功";
            }
            return "删除失败";
        }
       
       




    }
}
