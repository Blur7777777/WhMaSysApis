using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WhMaSysApi.Models
{
    public class GetProRequest
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string ItemName { get; set; }
        public string UserName { get; set; }
        
        public DateTime? OrderTime { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }

       
    }
    public class updatapassword
    {
        public int Id { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }

}
