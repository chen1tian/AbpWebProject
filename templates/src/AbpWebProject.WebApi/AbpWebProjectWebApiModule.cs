using AbpWebProject.Application;
using AbpWebProject.Application.Contracts;
using AbpWebProject.Domain;
using AbpWebProject.EntityFramework;
using AbpWebProject.EntityFramework.SqlServer;
using AbpWebProject.WebApi.Filter;
using Autofac.Core;
using Castle.DynamicProxy;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.AspNetCore.MultiTenancy;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.AntiForgery;
using Volo.Abp.Autofac;
using Volo.Abp.AutoMapper;
using Volo.Abp.Castle.DynamicProxy;
using Volo.Abp.Data;
using Volo.Abp.DynamicProxy;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Swashbuckle;
using Volo.Abp.Threading;
using Volo.Abp.Validation;
using AbpWebProject.EntityFramework.MySql;

namespace AbpWebProject.WebApi
{
    [DependsOn(
        typeof(AbpAspNetCoreMvcModule),
        typeof(AbpAutofacModule),
        typeof(AbpSwashbuckleModule),
        typeof(AbpWebProjectDomainMobule),
        typeof(AbpWebProjectEntityFrameworkModule),
        typeof(AbpWebProjectApplicationContractsModule),
        typeof(AbpWebProjectApplicationModule),
        typeof(AbpAutoMapperModule)
    )]
    public class AbpWebProjectWebApiModule : AbpModule
    {
        public IConfiguration Configuration { get; set; }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var services = context.Services;
            Configuration = services.GetConfiguration();

            // 增加结果包装器
            services.AddMvcCore(options =>
            {
                options.Filters.Add(typeof(ResultExceptionFilter));
                options.Filters.Add(typeof(ResultFilter));
            });

            ConfigureAutoApiControllers();
            ConfigureAutoMapper();
            ConfigureSwaggerServices(services);
            ConfigureAbpAntiForgeryOptions();
            ConfigureSqlServerDbContext(services);
        }

        /// <summary>
        /// 配置数据库
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void ConfigureSqlServerDbContext(IServiceCollection services)
        {
            Configure<AbpDbContextOptions>(options =>
            {
                // 增加Db到服务中, 否则执行迁移命令时会报错
                services.AddDbContext<AbpWebProjectDbContext>();

                options.UseSqlServerDb<AbpWebProjectDbContext>();   // 修改此处
            });
        }

        /// <summary>
        /// 配置数据库
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void ConfigureMySqlDbContext(IServiceCollection services)
        {
            // 增加Db到服务中, 否则执行迁移命令时会报错
            services.AddDbContext<AbpWebProjectDbContext>();

            // 增加Db到服务中
            var connectionString = Configuration.GetConnectionString("MySql");

            Configure<AbpDbContextOptions>(options =>
            {
                options.UseMySqlDb<AbpWebProjectDbContext>(connectionString);
            });
        }

        private void ConfigureAbpAntiForgeryOptions()
        {
            Configure<AbpAntiForgeryOptions>(options =>
            {
                options.AutoValidate = false;
            });
        }

        private void ConfigureAutoApiControllers()
        {
            Configure<AbpAspNetCoreMvcOptions>(options =>
            {
                options.ConventionalControllers.Create(typeof(AbpWebProjectApplicationModule).Assembly);
            });
        }

        /// <summary>
        /// 配置AutoMapper
        /// </summary>
        private void ConfigureAutoMapper()
        {
            Configure<AbpAutoMapperOptions>(options =>
            {
                options.AddMaps<AbpWebProjectWebApiModule>(validate: true);
            });
        }

        /// <summary>
        /// 配置Swagger
        /// </summary>
        /// <param name="services"></param>
        private void ConfigureSwaggerServices(IServiceCollection services)
        {
            var basePath = Configuration["Swagger:BasePath"];

            services.AddSwaggerGen(
                options =>
                {
                    options.SwaggerDoc("v1", new OpenApiInfo { Title = "AbpWebProject API", Version = "v1" });
                    options.DocInclusionPredicate((docName, description) => true);
                    options.CustomSchemaIds(type => type.FullName);
                    options.DocumentFilter<SwaggerDocumentFilter>(basePath);

                    var baseDir = AppContext.BaseDirectory;
                    options.IncludeXmlComments(Path.Combine(baseDir, "AbpWebProject.Domain.xml"));
                    options.IncludeXmlComments(Path.Combine(baseDir, "AbpWebProject.EntityFramework.xml"));
                    options.IncludeXmlComments(Path.Combine(baseDir, "AbpWebProject.Application.Contracts.xml"));
                    options.IncludeXmlComments(Path.Combine(baseDir, "AbpWebProject.Application.xml"));
                    options.IncludeXmlComments(Path.Combine(baseDir, "AbpWebProject.WebApi.xml"));

                    // 定义认证
                    var securityScheme = new OpenApiSecurityScheme
                    {
                        Name = "JWT Authentication",
                        Description = "**_只需要_**输入token，**_不要_**加Bearer",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer", // must be lower case
                        BearerFormat = "JWT",
                        Reference = new OpenApiReference
                        {
                            Id = "Bearer",
                            Type = ReferenceType.SecurityScheme
                        }
                    };
                    options.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
                    options.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {securityScheme, new string[] { }}
                    });

                    // 将枚举类型转换为Description字符串
                    // 使用的话，会将测试用的参数也转换为字符串，导致无法测试
                    //options.SchemaFilter<SwaggerEnumSchemaFilter>();
                }
            );
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            var app = context.GetApplicationBuilder();

            // 自动迁移数据库
            app.UseAutoMigration<AbpWebProjectDbContext>();

            var env = context.GetEnvironment();

            // 跨域处理
            var allowOrigins = Configuration["AllowOrigins"];
            var allowOriginsArr = allowOrigins.Split(',');

            app.UseCors(options =>
            {
                options
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .WithOrigins(allowOriginsArr);
            });


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            app.UseRouting();

            // 如果启用swagger
            var enableSwagger = Convert.ToBoolean(Configuration["Swagger:Enabled"]);

            if (enableSwagger)
            {
                app.UseSwagger(c =>
                {
                    c.RouteTemplate = "AbpWebProject/swagger/{documentName}/swagger.json";
                });
                app.UseAbpSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/AbpWebProject/swagger/v1/swagger.json", "AbpWebProject API");
                });
            }

            // 中间件一般要放在此语句之前
            app.UseConfiguredEndpoints();

            // 数据库初始化数据
            using (var scope = context.ServiceProvider.CreateScope())
            {
                AsyncHelper.RunSync(async () =>
                {
                    var seeder = scope.ServiceProvider
                        .GetRequiredService<IDataSeeder>();

                    await seeder.SeedAsync();
                });
            }
        }
    }
}
