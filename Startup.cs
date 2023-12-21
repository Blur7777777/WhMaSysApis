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

            //���Redis����
            //redis����
            var section = Configuration.GetSection("Redis:Default");
            //�����ַ���
            string _connectionString = section.GetSection("Connection").Value;
            //ʵ������
            string _instanceName = section.GetSection("InstanceName").Value;
            //Ĭ�����ݿ� 
            int _defaultDB = int.Parse(section.GetSection("DefaultDB").Value ?? "0");
            //����
            string _password = section.GetSection("Password").Value;
            //ע������
            services.AddSingleton(new RedisHelper(_connectionString, _instanceName, _password, _defaultDB));


            //���swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "�ֿ����ϵͳ", Version = "v1" });
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Description = "���¿�����������ͷ����Ҫ���Jwt��ȨToken��Bearer Token",
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

            //����

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


            //��ȡappseting.json
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

            //ע��JWT����
            services.AddScoped<IJWTService, JWTService>();
            //ע���ȡ�û����� �����������
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
            //������֤
            app.UseAuthentication();
            // UseCors Middleware
            app.UseCors("AllowAny");

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            //ʹ��swagger
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "coreMVC3.1");
                //c.RoutePrefix = string.Empty;
                c.RoutePrefix = "swagger";     //�����Ϊ�� ����·����Ϊ ������/index.html,ע��localhost:�˿ں�/swagger�Ƿ��ʲ�����
                                               //·�����ã�����Ϊ�գ���ʾֱ���ڸ�������localhost:8001�����ʸ��ļ�
                                               // c.RoutePrefix = "swagger"; // ������뻻һ��·����ֱ��д���ּ��ɣ�����ֱ��дc.RoutePrefix = "swagger"; �����·��Ϊ ������/swagger/index.html
            });
        }
    }
}
