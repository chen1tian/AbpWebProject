using AbpWebProject.Application;
using AbpWebProject.Application.Contracts;
using AbpWebProject.Application.Contracts.Products;
using AbpWebProject.Domain;
using AbpWebProject.EntityFramework;
using Castle.DynamicProxy;
using Microsoft.AspNetCore.Builder;
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
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Swashbuckle;
using Volo.Abp.Threading;
using Volo.Abp.Validation;

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

            ConfigureAutoApiControllers();
            ConfigureAutoMapper();
            ConfigureSwaggerServices(services);
            ConfigureAbpAntiForgeryOptions();
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
                }
            );
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            var app = context.GetApplicationBuilder();
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
            app.UseConfiguredEndpoints();


            app.UseSwagger(c =>
            {
                c.RouteTemplate = "AbpWebProject/swagger/{documentName}/swagger.json";
            });
            app.UseAbpSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/AbpWebProject/swagger/v1/swagger.json", "AbpWebProject API");
            });

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
