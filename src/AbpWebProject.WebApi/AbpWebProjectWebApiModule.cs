using AbpWebProject.Application;
using AbpWebProject.Application.Contracts;
using AbpWebProject.Application.Contracts.Products;
using AbpWebProject.Domain;
using AbpWebProject.EntityFramework;
using Castle.DynamicProxy;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.AspNetCore.MultiTenancy;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Autofac;
using Volo.Abp.AutoMapper;
using Volo.Abp.Castle.DynamicProxy;
using Volo.Abp.DynamicProxy;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Swashbuckle;
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
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var services = context.Services;

            ConfigureAutoApiControllers();
            ConfigureAutoMapper();
            ConfigureSwaggerServices(services);

            ConfigureDaprService(services);
        }

        private static readonly ProxyGenerator ProxyGeneratorInstance = new ProxyGenerator();

        private void ConfigureDaprService(IServiceCollection services)
        {
            var type = typeof(IProductService);
            var interceptorType = typeof(DynamicDaprProxyInterceptor<>).MakeGenericType(type);
            services.AddTransient(interceptorType);

            var validationInterceptorAdapterType =
                typeof(AbpAsyncDeterminationInterceptor<>).MakeGenericType(typeof(ValidationInterceptor));
            var interceptorAdapterType = typeof(AbpAsyncDeterminationInterceptor<>).MakeGenericType(interceptorType);

            services.AddTransient(
                typeof(IDaprClientProxy<>).MakeGenericType(type),
                serviceProvider =>
                {
                    var service = ProxyGeneratorInstance
                        .CreateInterfaceProxyWithoutTarget(
                            type,
                            (IInterceptor)serviceProvider.GetRequiredService(validationInterceptorAdapterType),
                            (IInterceptor)serviceProvider.GetRequiredService(interceptorAdapterType)
                        );

                    return Activator.CreateInstance(
                        typeof(DaprClientProxy<>).MakeGenericType(type),
                        service
                    );
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
            services.AddSwaggerGen(
                options =>
                {
                    options.SwaggerDoc("v1", new OpenApiInfo { Title = "AbpWebProject API", Version = "v1" });
                    options.DocInclusionPredicate((docName, description) => true);
                    options.CustomSchemaIds(type => type.FullName);
                }
            );
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            var app = context.GetApplicationBuilder();
            var env = context.GetEnvironment();

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


            app.UseSwagger();
            app.UseAbpSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "AbpWebProject API");
            });

            app.UseConfiguredEndpoints();
        }
    }
}
