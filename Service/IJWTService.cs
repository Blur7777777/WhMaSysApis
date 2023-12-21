using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WhMaSysApi.Service
{
     public interface IJWTService
    {
        string CreateToken(int Id, string userName, string role);
    }
}
