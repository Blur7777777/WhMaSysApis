﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WhMaSysApi.Models
{
    public class JWTConfig
    {
        /// <summary>
        /// 密钥 长度不少于16位
        /// </summary>
        public string Secret { get; set; }

        /// <summary>
        /// 发行人
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// 观众
        /// </summary>
        public string Audience { get; set; }

        /// <summary>
        /// 访问过期
        /// </summary>
        public int AccessExpiration { get; set; }

        /// <summary>
        /// 刷新过期
        /// </summary>
        public int RefreshExpiration { get; set; }
    }
}
