using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WhMaSysApi.Models;
using WhMaSysApi.Models.Database;
using WhMaSysApi.Service;

namespace WhMaSysApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            //添加Redis服务
            //redis缓存
            var section = Configuration.GetSection("Redis:Default");
            //连接字符串
            string _connectionString = section.GetSection("Connection").Value;
            //实例名称
            string _instanceName = section.GetSection("InstanceName").Value;
            //默认数据库 
            int _defaultDB = int.Parse(section.GetSection("DefaultDB").Value ?? "0");
            //密码
            string _password = section.GetSection("Password").Value;
            //注入容器
            services.AddSingleton(new RedisHelper(_connectionString, _instanceName, _password, _defaultDB));


            //添加swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "仓库管理系统", Version = "v1" });
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Description = "在下框中输入请求头中需要添加Jwt授权Token：Bearer Token",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme{
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"}
            },new string[] { }
        }
    });
            });

            //跨域

            // Add Cors
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAny",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                               .AllowAnyMethod()
                               .AllowAnyHeader();
                    });
            });


            //读取appseting.json
            services.Configure<JWTConfig>(Configuration.GetSection("JWTConfig"));
            var tokenConfigs = Configuration.GetSection("JWTConfig").Get<JWTConfig>();
            //Authentication
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x => {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenConfigs.Secret)),
                    ValidIssuer = tokenConfigs.Issuer,
                    ValidAudience = tokenConfigs.Audience,
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });



            services.AddDbContext<WhMaSysContext>();

            //注入JWT服务
            services.AddScoped<IJWTService, JWTService>();
            //注入获取用户服务 领域驱动设计
            services.AddScoped<IUserService, UserService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            //启用认证
            app.UseAuthentication();
            // UseCors Middleware
            app.UseCors("AllowAny");

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            //使用swagger
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "coreMVC3.1");
                //c.RoutePrefix = string.Empty;
                c.RoutePrefix = "swagger";     //如果是为空 访问路径就为 根域名/index.html,注意localhost:端口号/swagger是访问不到的
                                               //路径配置，设置为空，表示直接在根域名（localhost:8001）访问该文件
                                               // c.RoutePrefix = "swagger"; // 如果你想换一个路径，直接写名字即可，比如直接写c.RoutePrefix = "swagger"; 则访问路径为 根域名/swagger/index.html
            });
        }
    }
}
