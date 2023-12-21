using System;
using System.Collections.Generic;

#nullable disable

namespace WhMaSysApi.Models.Database
{
    public partial class User
    {
        public int Id { get; set; }
        public string Phone { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public DateTime Time { get; set; }
        public string Role { get; set; }
        public string Sex { get; set; }
        public DateTime? Birthday { get; set; }
        public string Nation { get; set; }
        public string Idnumber { get; set; }
        public string Education { get; set; }
        public string Address { get; set; }
    }
}
