using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WhMaSysApi.Models;
using WhMaSysApi.Models.Database;
using WhMaSysApi.Service;

namespace WhMaSysApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        //Scaffold-DbContext "Data Source=.;Initial Catalog=WhMaSys;Integrated Security=SSPI" Microsoft.EntityFrameworkCore.SqlServer -o ./Models/Database -Force
        //定义数据库上下文
        private readonly WhMaSysContext _db;
        //注入jwt服务
        private readonly IJWTService _jwtService;
        //注入用户服务服务
        private readonly IUserService _userService;

        /// <summary>
        /// 构造方法 构造注入
        /// </summary>
        public HomeController(WhMaSysContext db, IJWTService jwtService, IUserService userService)
        {
            _db = db;
            _jwtService = jwtService;
            _userService = userService;

        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public string Reg(Risrequest request)
        {
            if (_userService.TfUserPhone(request.Phone))
                return "用户存在";
            //if (request.Password != request.CirPassword)
            //    return "两次密码输入不一致";
            //使用服务
            if (!PhoneNumber(request.Phone))
            {
                return "手机号格式不正确";
            }
            var UserId = _userService.Reg(request.Phone, request.username, request.Password);
            if (UserId != null)
            {
                return "注册成功";
            }
            return "注册失败";
        }
        // 验证手机号是否为11位纯数字
        private bool PhoneNumber(string phone)
        {
            const string PhoneNumberRegex = @"^1\d{10}$";

            if (string.IsNullOrEmpty(phone))
            {
                // 手机号为空，不通过验证
                return false;
            }

            // 使用正则表达式进行验证
            return Regex.IsMatch(phone, PhoneNumberRegex);
        }
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost]
        public string Login([FromBody] LoginModel loginModel)
        {
           
            var user = _userService.GetphoneByphone(loginModel.Phone);
            if (user == null)
                return "用户不存在";

            //生成md5字符串                      输入的         数据库的盐值
            var md5 = MD5Helper.MD5Encrypt32(loginModel.Password + user.Salt.ToString());
            if (user.Password != md5)
                return "密码不正确";

            return _jwtService.CreateToken(user.Id,user.UserName,user.Role);

        }
        /// <summary>
        /// 修改密码 salt md5
        /// </summary>
        /// <param name="updatapassword"></param>
        /// <returns></returns>
        [HttpPost]
        public string Updatapassword([FromBody] updatapassword updatapassword)
        {
            var item = _db.Users.FirstOrDefault(x => x.Id == updatapassword.Id);
            if (item == null)
                return "没有该人员";

            var salt = item.Salt;
            var oldPassword = MD5Helper.MD5Encrypt32(updatapassword.OldPassword + salt.ToString());
            var newPassword = MD5Helper.MD5Encrypt32(updatapassword.NewPassword + salt.ToString());
            if (oldPassword != item.Password)
                return "旧密码错误";

            item.Password = newPassword;
            var row = _db.SaveChanges();
            if (row > 0)
            {
                return "修改成功";
            }
            return "修改失败";
        }
        /// <summary>
        /// 解析token
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public string UserInfo()
        {
            //获取用户信息
            return Response.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name).Value;
        }
        /// <summary>
        /// 分页查询商品
        /// </summary>
        /// <param name="Keyword"></param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        [HttpGet]
        public GetProResponse GetGoodShow(string Keyword, int? Proid, int? Cateid, int PageIndex, int PageSize)
        {
            return _userService.GetGoodShow(Keyword, Proid, Cateid, PageIndex, PageSize);
        }
        /// <summary>
        /// 新增商品
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [Authorize(Roles = "管理员")]
        [HttpPost]
        public string AddProduct([FromBody] NewProduct newproduct)
        {
            return _userService.Addpro(newproduct);

        }
        /// <summary>
        /// 更新商品
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [Authorize(Roles = "管理员")]
        [HttpPost]
        public string Updateproduct([FromBody] Product product)
        {
            return _userService.Updatepro(product.Id, product.ProductName,product.ProductPrice,product.ProductStock, product.Categoryid, product.Supplierid);
        }
        /// <summary>
        /// 根据id删除商品
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [Authorize(Roles = "管理员")]
        [HttpPost]
        public string DelProduct([FromBody] Product product)
        {
            return _userService.DelProduct(product.Id);
        }
        /// <summary>
        /// 模糊查询客户名
        /// </summary>
        /// <param name="customername"></param>
        /// <returns></returns>
        //[Authorize(Roles = "管理员")]
        [HttpGet]
        public List<Customer> Customername(string customername)
        {
            return _userService.Customername(customername);
        }

        //[HttpPost]
        //public int AddOrder(string CustomerName, string ItemName, int ItemQuantity, decimal ItemPrice, string UserName, string Status, string Remark)
        //{
        //    return _userService.AddOrder(CustomerName, ItemName, ItemQuantity, ItemPrice, UserName, Status, Remark);
        //}
        /// <summary>
        /// 订单
        /// </summary>
        /// <param name="orders"></param>
        /// <returns></returns>
        //[HttpPost]
        //public int AddOrder([FromBody] List<Order> orders)
        //{
            
        //    if (orders == null || orders.Count == 0)
        //    {
                
        //        return 0;
        //    }
        //    return _userService.AddOrders(orders);
        //}

        /// <summary>
        /// 售后单
        /// </summary>
        /// <param name="PartAfterSales"></param>
        /// <returns></returns>
        [HttpPost]
        public int AddPartAfterSale([FromBody] List<PartAfterSale> PartAfterSales)
        {

            if (PartAfterSales == null || PartAfterSales.Count == 0)
            {

                return 0;
            }
            return _userService.AddPartAfterSale(PartAfterSales);
        }
        /// <summary>
        /// 分页查询订单
        /// </summary>
        /// <param name="id"></param>
        /// <param name="itemName"></param>
        /// <param name="orderTime"></param>
        /// <param name="customerName"></param>
        /// <param name="userName"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        public GetProResponse GetOrderShow(int? id, string itemName, DateTime orderTime, string customerName, string userName, int pageIndex, int pageSize)
        {
            return _userService.GetOrderShow(id,itemName,orderTime,customerName,userName,pageIndex,pageSize) ;
        }
        /// <summary>
        /// 分页查询工作人员
        /// </summary>
        /// <param name="Phone"></param>
        /// <param name="UserName"></param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        [HttpGet]
        public GetProResponse GetWorker(string Phone, string UserName, int PageIndex, int PageSize)
        {
            return _userService.GetWorker(Phone, UserName, PageIndex, PageSize);
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
        //[HttpPost]
        //public string UpdateOrder(int Id, string CustomerName, string ItemName, string ItemQuantity, decimal ItemPrice, string UserName, string Status, string Remark)
        //{
        //    return _userService.UpdateOrder(Id, CustomerName, ItemName, ItemQuantity, ItemPrice, UserName, Status, Remark);
        //}
        [Authorize(Roles = "管理员")]
        [HttpPost]
        public string UpdateOrder([FromBody] Order order)
        {
            return _userService.UpdateOrder(order.Id, order.CustomerName, order.ItemName, order.ItemQuantity, (decimal)order.ItemPrice, order.UserName, order.Status, order.Remark);
        }
        /// <summary>
        /// 接收对象值id删除订单
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = "管理员")]
        [HttpPost]
        public string DelOrder([FromBody] OrderModel model)
        {
            int id = model.Id;
            return _userService.DelOrder(id);
        }
        /// <summary>
        /// 查询分类表的数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetProCate()
        {
            var procate = _db.ProductCates.ToList();
            return new JsonResult(procate);//将 procate 对象序列化为 JSON 格式并返回
        }
        /// <summary>
        /// 查询供货商分类
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetSupCate()
        {
            var supcate = _db.SupplierCategories.ToList();
            return new JsonResult(supcate);//将 procate 对象序列化为 JSON 格式并返回
        }
        /// <summary>
        /// 查询业务员
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetUser()
        {
            var supcate = _db.Users.ToList();
            return new JsonResult(supcate);//将 procate 对象序列化为 JSON 格式并返回
        }
        /// <summary>
        /// 分页查询售后单
        /// </summary>
        /// <param name="Keyword"></param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        [HttpGet]
        public GetProResponse GetPartAfterSale(int? id, string itemName, DateTime orderTime, string customerName, string userName, int PageIndex, int PageSize)
        {
            return _userService.GetPartAfterSale(id, itemName, orderTime, customerName, userName, PageIndex, PageSize);
        }
        /// <summary>
        /// 修改配件售后单
        /// </summary>
        /// <param name="afterSale"></param>
        /// <returns></returns>
        [Authorize(Roles = "管理员")]
        [HttpPost]
        public string UpdateSale([FromBody] PartAfterSale afterSale )
        {
            return _userService.UpdateSale(afterSale.Id, afterSale.CustomerName, afterSale.ItemName, afterSale.ItemQuantity, (decimal)afterSale.ItemPrice, afterSale.UserName, afterSale.Status, afterSale.Remark);
        }
        /// <summary>
        /// 删除售后单
        /// </summary>
        /// <param name="afterSale"></param>
        /// <returns></returns>
        [Authorize(Roles = "管理员")]
        [HttpPost]
        public string DelSale([FromBody] PartAfterSale afterSale)
        {
           
            return _userService.DelSale(afterSale.Id);
        }

        /// <summary>
        /// 管理员添加工作人员
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>

        [Authorize(Roles = "管理员")]
        [HttpPost]
        public string InsertWoker(Risrequest request)
        {
            if (_userService.TfUserPhone(request.Phone))
                return "用户存在";
            //if (request.Password != request.CirPassword)
            //    return "两次密码输入不一致";
            //使用服务

            var UserId = _userService.Reg(request.Phone, request.username, request.Password);
            if (UserId != null)
            {
                return "注册成功";
            }
            return "注册失败";
        }

        /// <summary>
        /// 修改工作人员信息
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [Authorize(Roles = "管理员")]
        [HttpPost]
        public string UpdateWorker([FromBody] User user)
        {
            return _userService.UpdateWorker(user.Id, user.UserName, user.Phone, user.Role, user.Sex, user.Birthday, user.Nation, user.Idnumber, user.Education, user.Address);
        }
        /// <summary>
        /// 删除工作人员
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [Authorize(Roles = "管理员")]
        [HttpPost]
        public string DelWorker([FromBody] User user)
        {

            return _userService.DelWorker(user.Id);
        }
        /// <summary>
        /// 添加供应商信息
        /// </summary>
        /// <param name="supplier"></param>
        /// <returns></returns>
        [Authorize(Roles = "管理员")]
        [HttpPost]
        public string InsertSupplier([FromBody] Supplier supplier)
        {
            try
            {
                var suppliers = new Supplier()
                {
                    SupId = GenerateRandom5DigitString(),
                    SupplierName = supplier.SupplierName,
                    SupplierAddress = supplier.SupplierAddress,
                    SupplierPhone = supplier.SupplierPhone,
                    DateAdded = DateTime.Now,
                };

                //上下文.表名.操作方法(需要插入的数据对象)
                _db.Suppliers.Add(suppliers);
                var rowsAffected = _db.SaveChanges();
                if (rowsAffected > 0)
                {
                    return "添加成功";
                }
                else
                {
                    return "添加失败";
                }
            }
            catch (Exception ex)
            {

                return $"添加异常: {ex.Message}";
            }
        }
        private string GenerateRandom5DigitString()
        {
            Random random = new Random();
            int randomNumber = random.Next(10000, 99999); // 生成在10000到99999之间的随机数
            return randomNumber.ToString();
        }
        /// <summary>
        /// 修改供应商信息
        /// </summary>
        /// <param name="supplier"></param>
        /// <returns></returns>
        [Authorize(Roles = "管理员")]
        [HttpPost]
       
        public string UpdateSupplier([FromBody] Supplier supplier)
        {
            try
            {
                var existingSupplier = _db.Suppliers.Find(supplier.Id);

                if (existingSupplier != null)
                {
                    existingSupplier.SupplierName = supplier.SupplierName;
                    existingSupplier.SupplierAddress = supplier.SupplierAddress;
                    existingSupplier.SupplierPhone = supplier.SupplierPhone;
                    _db.SaveChanges();
                    return "更新成功";
                }
                else
                {
                    return "未找到供应商";
                }
            }
            catch (Exception ex)
            {
               
                return $"更新异常: {ex.Message}";
            }
        }
        /// <summary>
        /// 删除供应商
        /// </summary>
        /// <param name="supplierId"></param>
        /// <returns></returns>
        [Authorize(Roles = "管理员")]
        [HttpPost]
        public string DeleteSupplier([FromBody] Supplier supplier)
        {
            try
            {
                var supplierToDelete = _db.Suppliers.Find(supplier.Id);

                if (supplierToDelete != null)
                {
                    _db.Suppliers.Remove(supplierToDelete);
                    _db.SaveChanges();
                    return "删除成功";
                }
                else
                {
                    return "未找到供应商";
                }
            }
            catch (Exception ex)
            {
               
                return $"删除异常: {ex.Message}";
            }
        }
        /// <summary>
        /// 查询供应商
        /// </summary>
        /// <param name="SupplierName"></param>
        /// <param name="SupplierPhone"></param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        [Authorize(Roles = "管理员")]
        [HttpGet]
        public GetProResponse GetSupplier(string SupplierPhone, string SupplierName, int PageIndex, int PageSize)
        {
            //查询所有数据
            var query = _db.Suppliers.AsQueryable();

            query = query.Where(x =>
               (string.IsNullOrEmpty(SupplierPhone) || x.SupplierPhone.Contains(SupplierPhone)) &&
                (string.IsNullOrEmpty(SupplierName) || x.SupplierName.Contains(SupplierName))

           );
            var total = query.Count();
            var list = query.OrderByDescending(x => x.DateAdded).Skip((PageIndex - 1) * PageSize).Take(PageSize).Select(x => new SupplierModel
            {
                Id = x.SupId,
                SupplierName = x.SupplierName,
                SupplierAddress = x.SupplierAddress,
                SupplierPhone = x.SupplierPhone,
                DateAdded = x.DateAdded

            })
                .ToList();
            ///构造返回对象
            var response = new GetProResponse()
            {
                List5 = list,
                Total = total
            };
            return response;
        }
        /// <summary>
        /// 添加客户
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        [Authorize(Roles = "管理员")]
        [HttpPost]
        public string Customer([FromBody] Customer customer)
        {
            try
            {
                var customers = new Customer()
                {
                    CustomerName=customer.CustomerName,
                    CustomerPhone = customer.CustomerPhone,
                    CustomerAddress = customer.CustomerAddress,
                    DateAdded = DateTime.Now.Date,
                };

                //上下文.表名.操作方法(需要插入的数据对象)
                _db.Customers.Add(customers);
                var rowsAffected = _db.SaveChanges();
                if (rowsAffected > 0)
                {
                    return "添加成功";
                }
                else
                {
                    return "添加失败";
                }
            }
            catch (Exception ex)
            {

                return $"添加异常: {ex.Message}";
            }
        }
        /// <summary>
        /// 修改客户信息
        /// </summary>
        /// <param name="updatedCustomer"></param>
        /// <returns></returns>
        [Authorize(Roles = "管理员")]
        [HttpPost]
        public string UpdateCustomer([FromBody] Customer updatedCustomer)
        {
            try
            {
                var existingCustomer = _db.Customers.Find(updatedCustomer.Id);

                if (existingCustomer != null)
                {
                    existingCustomer.CustomerName = updatedCustomer.CustomerName;
                    existingCustomer.CustomerPhone = updatedCustomer.CustomerPhone;
                    existingCustomer.CustomerAddress = updatedCustomer.CustomerAddress;

                    

                    _db.SaveChanges();
                    return "更新成功";
                }
                else
                {
                    return "未找到客户";
                }
            }
            catch (Exception ex)
            {
               
                return $"更新异常: {ex.Message}";
            }
        }
        /// <summary>
        /// 删除客户
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        [Authorize(Roles = "管理员")]
        [HttpPost]
        public string DeleteCustomer(int customerId)
        {
            try
            {
                var customerToDelete = _db.Customers.Find(customerId);

                if (customerToDelete != null)
                {
                    _db.Customers.Remove(customerToDelete);
                    _db.SaveChanges();
                    return "删除成功";
                }
                else
                {
                    return "未找到客户";
                }
            }
            catch (Exception ex)
            {
                
                return $"删除异常: {ex.Message}";
            }
        }
        /// <summary>
        /// 分页查询客户
        /// </summary>
        /// <param name="CustomerName"></param>
        /// <param name="CustomerPhone"></param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        [HttpGet]
        public GetProResponse GetCustomer(string CustomerName, string CustomerPhone, int PageIndex, int PageSize)
        {
            //查询所有数据
            var query = _db.Customers.AsQueryable();

            query = query.Where(x =>
               (string.IsNullOrEmpty(CustomerName) || x.CustomerName.Contains(CustomerName)) &&
                (string.IsNullOrEmpty(CustomerPhone) || x.CustomerPhone.Contains(CustomerPhone))

           );
            var total = query.Count();
            var list = query.OrderByDescending(x => x.DateAdded).Skip((PageIndex - 1) * PageSize).Take(PageSize).Select(x => new CustomerModel
            {
                Id = x.Id,
                CustomerName = x.CustomerName,
                CustomerPhone = x.CustomerPhone,
                CustomerAddress = x.CustomerAddress,
                DateAdded = x.DateAdded

            })
                .ToList();
            ///构造返回对象
            var response = new GetProResponse()
            {
                List6 = list,
                Total = total
            };
            return response;
        }
        /// <summary>
        /// 根据id查找用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetUserById(int id)
        {
            var user = _db.Users.FirstOrDefault(u => u.Id == id);

            if (user == null)
            {
                return NotFound(); 
            }

            return Ok(user);
        }

    }
}
