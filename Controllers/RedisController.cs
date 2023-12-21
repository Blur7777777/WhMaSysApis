using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WhMaSysApi.Models;
using WhMaSysApi.Models.Database;
using WhMaSysApi.Service;

namespace WhMaSysApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RedisController : ControllerBase
    {
        private readonly IDatabase _redis;
        //定义数据库上下文
        private readonly WhMaSysContext _db;
       
        //注入jwt服务
        private readonly IJWTService _jwtService;
        //注入用户服务服务
        private readonly IUserService _userService;

        /// <summary>
        /// 构造方法 构造注入
        public RedisController(Models.RedisHelper client, WhMaSysContext db, IJWTService jwtService, IUserService userService)
        {
            //创建Redis连接对象
            _redis = client.GetDatabase();
            _db = db;
            _jwtService = jwtService;
            _userService = userService;
        }



        [HttpGet]
        public void SetRedisCache1111()
        {
            _redis.StringSet("RedisCache111111", "666");
        }
        /// <summary>
        /// 获取当天的营业总额
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetDailyOrderTotal()
        {
            // 使用通用的缓存键
            string redisKey = "DailyOrderTotal";
            // 从Redis缓存中获取数据
            string cachedData = _redis.StringGet(redisKey);
            if (!string.IsNullOrEmpty(cachedData))
            {
                // 如果缓存存在，直接返回缓存数据
                return Ok(cachedData);
            }
            else
            {
                // 如果缓存不存在，查询数据库计算订单总额
                decimal dailyTotal = CalculateDailyOrderTotal();

                // 更新缓存
                _redis.StringSet(redisKey, JsonConvert.SerializeObject(dailyTotal));

                // 返回计算结果
                return Ok(dailyTotal.ToString());
            }
        }


        private decimal CalculateDailyOrderTotal()
        {
            // 在这里实现从数据库中查询每日订单总额的逻辑
           

            //DateTime today = new DateTime(2023, 11, 13);
            DateTime today = DateTime.UtcNow.Date;
            DateTime tomorrow = today.AddDays(1);

            decimal dailyTotal = (decimal)_db.Orders
                .Where(o => o.OrderTime >= today && o.OrderTime < tomorrow)
                .Sum(o => o.Amount);

            return dailyTotal;
        }
        /// <summary>
        /// 一周内每天的营业额
        /// </summary>
        /// <returns></returns>
        //[HttpGet]
        //public IActionResult GetDailyOrderTotal7()
        //{
        //    // 设置Redis键，每天一个键
        //    string redisKey = $"DailyOrderTotal7:{DateTime.UtcNow.ToString("yyyyMMdd")}";
        //    // 尝试从Redis缓存中获取数据
        //    string cachedData = _redis.StringGet(redisKey);
        //    if (!string.IsNullOrEmpty(cachedData))
        //    {
        //        // 如果缓存存在，直接返回缓存数据
        //        return Ok(cachedData);
        //    }
        //    else
        //    {
        //        // 如果缓存不存在，查询数据库计算订单总额
        //        List<DailyTotalDto> dailyTotal = CalculateLast7DaysOrderTotal();

        //        // 将计算结果存入Redis缓存，设置过期时间为一天
        //        _redis.StringSet(redisKey, JsonConvert.SerializeObject(dailyTotal), TimeSpan.FromDays(1));

        //        // 返回计算结果
        //        return Ok(dailyTotal.ToString());
        //    }
        //}


        [HttpGet]
        public IActionResult GetDailyOrderTotal7()
        {
            // 使用通用的缓存键
            string redisKey = "DailyOrderTotal7";

            // 从缓存中获取数据
            string cachedData = _redis.StringGet(redisKey);
            if (!string.IsNullOrEmpty(cachedData))
            {
                // 如果缓存存在，直接返回缓存数据
                return Ok(cachedData);
            }
            else
            {
                // 如果缓存不存在，查询数据库并更新缓存
                List<DailyTotalDto> dailyTotal = CalculateLast7DaysOrderTotal();

                // 更新缓存
                _redis.StringSet(redisKey, JsonConvert.SerializeObject(dailyTotal));

                // 返回计算结果
                return Ok(dailyTotal.ToString());
            }
        }



        private List<DailyTotalDto> CalculateLast7DaysOrderTotal()
        {
            List<DailyTotalDto> dailyTotals = new List<DailyTotalDto>();

            for (int i = 0; i < 7; i++)
            {
                DateTime currentDate = DateTime.Today.AddDays(-i);
                DateTime nextDate = currentDate.AddDays(1);

                decimal dailyTotal = (decimal)_db.Orders
                    .Where(o => o.OrderTime >= currentDate && o.OrderTime < nextDate)
                    .Sum(o => o.Amount);

                dailyTotals.Add(new DailyTotalDto { Date = currentDate, Total = dailyTotal });
            }

            return dailyTotals;
        }

        /// <summary>
        /// 添加订单
        /// </summary>
        /// <param name="orders"></param>
        /// <returns></returns>
        [HttpPost]
        public string AddOrder([FromBody] List<Models.Database.Order> orders)
        {
            if (orders == null || orders.Count == 0)
            {
                return "添加失败";
            }

            // 当添加新订单时，使缓存失效
            _redis.KeyDelete("DailyOrderCount1");
            _redis.KeyDelete("DailyOrderTotal");
            _redis.KeyDelete("DailyOrderTotal7");
            _redis.KeyDelete("YesterdayCount");
            _redis.KeyDelete("MonthCount");
            _redis.KeyDelete("HotProducts");

            return _userService.AddOrders(orders);
        }


        private decimal YesterdayDailyOrderTotal()
        {
            // 在这里实现从数据库中查询昨天订单总额的逻辑


            //DateTime today = new DateTime(2023, 11, 13);
            DateTime yesterday = DateTime.UtcNow.Date.AddDays(-1);
            DateTime today = DateTime.UtcNow.Date;
            decimal dailyTotal = (decimal)_db.Orders
                .Where(o => o.OrderTime >= yesterday && o.OrderTime < today)
                .Sum(o => o.Amount);

            return dailyTotal;
        }
        /// <summary>
        /// 昨日总额
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetyesterdayTotal()
        {
            // 设置Redis键，每天一个键
            string redisKey = $"yesterdayToal:{DateTime.UtcNow.ToString("yyyyMMdd")}";
            // 尝试从Redis缓存中获取数据
            string cachedData = _redis.StringGet(redisKey);
            if (!string.IsNullOrEmpty(cachedData))
            {
                // 如果缓存存在，直接返回缓存数据
                return Ok(cachedData);
            }
            else
            {
                // 如果缓存不存在，查询数据库计算订单总额
                decimal dailyTotal = YesterdayDailyOrderTotal();

                // 将计算结果存入Redis缓存，设置过期时间为一天
                _redis.StringSet(redisKey, JsonConvert.SerializeObject(dailyTotal), TimeSpan.FromDays(1));

                // 返回计算结果
                return Ok(dailyTotal.ToString());
            }
        }

        /// <summary>
        /// 获取当天的订单条数
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetDailyOrderCount()
        {
            // 使用通用的缓存键
            string redisKey = "DailyOrderCount1";
            // 从Redis缓存中获取数据
            string cachedData = _redis.StringGet(redisKey);
            if (!string.IsNullOrEmpty(cachedData))
            {
                // 如果缓存存在，直接返回缓存数据
                return Ok(cachedData);
            }
            else
            {
                // 如果缓存不存在，查询数据库计算订单总额
                int dailyTotal = CalculateDailyOrderCount();

                // 更新缓存
                _redis.StringSet(redisKey, JsonConvert.SerializeObject(dailyTotal));

                // 返回计算结果
                return Ok(dailyTotal.ToString());
            }
        }


        private int CalculateDailyOrderCount()
        {
            // 在这里实现从数据库中查询每日订单总条数


            //DateTime today = new DateTime(2023, 11, 13);
            DateTime today = DateTime.UtcNow.Date;
            DateTime tomorrow = today.AddDays(1);

            int dailyTotal = _db.Orders
                .Count(o => o.OrderTime >= today && o.OrderTime < tomorrow);
               

            return dailyTotal;
        }



        /// <summary>
        /// 获取昨天的订单条数
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetYesterdayCount()
        {
            // 使用通用的缓存键
            string redisKey = "YesterdayCount";
            // 从Redis缓存中获取数据
            string cachedData = _redis.StringGet(redisKey);
            if (!string.IsNullOrEmpty(cachedData))
            {
                // 如果缓存存在，直接返回缓存数据
                return Ok(cachedData);
            }
            else
            {
                // 如果缓存不存在，查询数据库计算订单总额
                int dailyTotal = YesterdayCount();

                // 更新缓存
                _redis.StringSet(redisKey, JsonConvert.SerializeObject(dailyTotal));

                // 返回计算结果
                return Ok(dailyTotal.ToString());
            }
        }


        private int YesterdayCount()
        {
            // 在这里实现从数据库中查询每日订单总条数


            //DateTime today = new DateTime(2023, 11, 13);
            DateTime yesterday = DateTime.UtcNow.Date.AddDays(-1);
            DateTime today = DateTime.UtcNow.Date;

            int dailyTotal = _db.Orders
                .Count(o => o.OrderTime >= yesterday && o.OrderTime < today);


            return dailyTotal;
        }
        //[HttpGet]
        //public List<MonthCount> monthCount()
        //{
        //    var product = _db.Products;
        //    var list = product.Join(_db.ProductCates, x => x.Categoryid, y => y.Id, (x, y) => new { product = x, ProductCates = y })
        //        .Join(_db.Orders, xy => xy.ProductCates.ProductName, z => z.ItemName, (xy, z) => new MonthCount
        //        {

        //            CategoryName=xy.CateName
        //            OrderCount=z.
        //        }).ToList();
        //    return list;

        //}



        private List<MonthCount> MonthCount()
        {
            var result = (from product in _db.Products
                          join productCate in _db.ProductCates on product.Categoryid equals productCate.Cateid
                          join order in _db.Orders on product.ProductName equals order.ItemName
                          where order.OrderTime.Month == DateTime.Now.Month
                          group new { productCate, order } by new { productCate.CateName } into grouped
                          select new MonthCount
                          {
                              CategoryName = grouped.Key.CateName,
                              OrderCount = grouped.Count()
                          }).ToList();

            return result;
        }
        /// <summary>
        /// 查询当前月每种配件的条数
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetMonthCount()
        {
            // 使用通用的缓存键
            string redisKey = "MonthCount";
            // 从Redis缓存中获取数据
            string cachedData = _redis.StringGet(redisKey);
            if (!string.IsNullOrEmpty(cachedData))
            {
                // 如果缓存存在，直接返回缓存数据
                return Ok(cachedData);
            }
            else
            {
                // 如果缓存不存在，查询数据库
                List<MonthCount> dailyTotal = MonthCount();

                // 更新缓存
                _redis.StringSet(redisKey, JsonConvert.SerializeObject(dailyTotal));

                // 返回计算结果
                return Ok(dailyTotal.ToString());
            }
        }
        /// <summary>
        /// 当前月份 总营业额 成本价 利润 总订单条数 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public List<MonthAmount> MonthAmount()
        {
            // 获取当前月份的第一天和最后一天
            var firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            // 计算营业额
            decimal totalRevenue = _db.Orders
                .Where(o => o.OrderTime >= firstDayOfMonth && o.OrderTime <= lastDayOfMonth)
                .Sum(o => o.Amount ?? 0);

            // 计算售出订单数
            int totalOrders = _db.Orders
                .Where(o => o.OrderTime >= firstDayOfMonth && o.OrderTime <= lastDayOfMonth)
                .Count();
         
            // 获取本月售出的订单
            var soldOrders = _db.Orders
                .Where(o => o.OrderTime >= firstDayOfMonth && o.OrderTime <= lastDayOfMonth)
                .ToList();

            // 计算售出订单的成本
            decimal totalCost = 0;
            foreach (var order in soldOrders)
            {
                // 进货价*售出数量
                var product = _db.NewProducts.FirstOrDefault(p => p.ProductName == order.ItemName);
                if (product != null && product.NewProductPrice.HasValue && product.ProductStock != null)
                {
                    totalCost += product.NewProductPrice.Value * (decimal.Parse(order.ItemQuantity));
                }
            }
            // 计算利润
            decimal profit = totalRevenue - totalCost;
            var monthAmountList = new List<MonthAmount>
             {
                new MonthAmount
                {
                    TotalRevenue = totalRevenue,
                    TotalCost = totalCost,
                    TotalOrders = totalOrders,
                    Profit=profit
                }
            };

            return monthAmountList;
        }
        /// <summary>
        /// 上个月份 总营业额 成本价 利润 总订单条数 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public List<MonthAmount> LastMonthAmount()
        {
            // 获取上个月的第一天和最后一天
            var firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            // 计算营业额
            decimal totalRevenue = _db.Orders
                .Where(o => o.OrderTime >= firstDayOfMonth && o.OrderTime <= lastDayOfMonth)
                .Sum(o => o.Amount ?? 0);

            // 计算售出订单数
            int totalOrders = _db.Orders
                .Where(o => o.OrderTime >= firstDayOfMonth && o.OrderTime <= lastDayOfMonth)
                .Count();

            // 获取本月售出的订单
            var soldOrders = _db.Orders
                .Where(o => o.OrderTime >= firstDayOfMonth && o.OrderTime <= lastDayOfMonth)
                .ToList();

            // 计算售出订单的成本
            decimal totalCost = 0;
            foreach (var order in soldOrders)
            {
                // 进货价*售出数量
                var product = _db.NewProducts.FirstOrDefault(p => p.ProductName == order.ItemName);
                if (product != null && product.NewProductPrice.HasValue && product.ProductStock != null)
                {
                    totalCost += product.NewProductPrice.Value * (decimal.Parse(order.ItemQuantity));
                }
            }
            // 计算利润
            decimal profit = totalRevenue - totalCost;
            var monthAmountList = new List<MonthAmount>
             {
                new MonthAmount
                {
                    TotalRevenue = totalRevenue,
                    TotalCost = totalCost,
                    TotalOrders = totalOrders,
                    Profit=profit
                }
            };

            return monthAmountList;
        }


        //[HttpGet]
        //public List<MonthProTotal> MonthProTotal()
        //{
        //    // 获取当前月份的第一天和最后一天
        //    var firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        //    var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

        //    // 获取本月的订单数据
        //    var orders = _db.Orders
        //        .Where(o => o.OrderTime >= firstDayOfMonth && o.OrderTime <= lastDayOfMonth)
        //        .ToList();

        //    // 统计热销商品排名
        //    var hotProducts = orders
        //        .GroupBy(o => o.ItemName)
        //        .Select(group => new MonthProTotal
        //        {
        //            ItemName = group.Key,
        //            ItemQuantity = group.Sum(o => int.Parse(o.ItemQuantity)) 
        //    })
        //        .OrderByDescending(mpt => mpt.ItemQuantity)
        //        .ToList();

        //    return hotProducts;
        //}

        /// <summary>
        /// 热销商品排名
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public List<MonthProTotal> MonthProTotal()
        {
            // 在Redis中定义排序的键
            string redisKey = "HotProducts";

            // 获取已排序的商品
            var hotProducts = _redis.SortedSetRangeByRankWithScores(redisKey, 0, -1);

            if (hotProducts == null || hotProducts.Length == 0)
            {
                // 如果不存在，计算排名并存储在Redis中

                // 获取当前月份的第一天和最后一天
                var firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

                // 获取本月的订单数据
                var orders = _db.Orders
                    .Where(o => o.OrderTime >= firstDayOfMonth && o.OrderTime <= lastDayOfMonth)
                    .ToList();

                // 统计热销商品排名
                foreach (var group in orders.GroupBy(o => o.ItemName))
                {
                    var itemQuantity = group.Sum(o => int.Parse(o.ItemQuantity)); 
                    _redis.SortedSetAdd(redisKey, group.Key, itemQuantity);
                }

                // 重新获取排名
                //hotProducts = _redis.SortedSetRangeByRankWithScores(redisKey, 0, -1);
                hotProducts = _redis.SortedSetRangeByRankWithScores(redisKey, 0, -1)
                  .OrderByDescending(item => item.Score)
                  .ToArray();
            }

            // 将Redis中的排序集结果转换为所需的格式
            var result = hotProducts
                .Select(item => new MonthProTotal
                {
                    ItemName = item.Element,
                    ItemQuantity = (int)item.Score
                })
                .OrderByDescending(item => item.ItemQuantity) // 倒序排序
                .Take(7) // 只取前7个
                .ToList();

            return result;
        }



    }
}