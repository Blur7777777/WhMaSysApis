using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WhMaSysApi.Models;

namespace WhMaSysApi.Service
{
    public class JWTService:IJWTService
    {
        private readonly JWTConfig _jwtConfig;

        public JWTService(IOptions<JWTConfig> jwtConfig)
        {
            this._jwtConfig = jwtConfig.Value;
        }

        public string CreateToken(int Id,string userName,string role)
        {
            ////把有需要的信息写到Token
            //var claims = new[] {
            //    //用户id
            //    new Claim(ClaimTypes.NameIdentifier,Id.ToString()),
            //    //用户名
            //    new Claim(ClaimTypes.Name, userName),
            //    // 用户角色
            //    new Claim(ClaimTypes.Role, role),
            //};

            // 把有需要的信息写到 Token  短键
            var claims = new[] {
                // 用户id
                new Claim("nameid", Id.ToString()),
                // 用户名
                new Claim("name", userName),
                // 用户角色
                new Claim("role", role),
    };

            //创建密钥
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Secret));
            //密钥加密
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            //token配置
            var jwtToken = new JwtSecurityToken(_jwtConfig.Issuer,
                _jwtConfig.Audience,
                claims,
                expires: DateTime.Now.AddMinutes(_jwtConfig.AccessExpiration),
                signingCredentials: credentials);

            //获取token
            var token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            return token;
        }
    }
}
