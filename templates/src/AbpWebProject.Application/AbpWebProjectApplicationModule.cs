using AbpWebProject.Application.Contracts;
using AbpWebProject.Domain;
using AbpWebProject.EntityFramework;
using Microsoft.Extensions.DependencyInjection;
using System;
using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace AbpWebProject.Application
{
    [DependsOn(
        typeof(AbpWebProjectDomainMobule),
        typeof(AbpWebProjectEntityFrameworkModule),
        typeof(AbpWebProjectApplicationContractsModule),
        typeof(AbpAutoMapperModule),
        typeof(AbpDddApplicationModule)
        )]
    public class AbpWebProjectApplicationModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            // 对象映射
            context.Services.AddAutoMapperObjectMapper<AbpWebProjectApplicationModule>();
            Configure<AbpAutoMapperOptions>(options =>
            {
                options.AddMaps<AbpWebProjectApplicationModule>(validate: true);
            });
        }
    }
}
