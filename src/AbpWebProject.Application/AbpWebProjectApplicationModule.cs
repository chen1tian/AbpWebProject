using AbpWebProject.Application.Contracts;
using AbpWebProject.Domain;
using AbpWebProject.EntityFramework;
using System;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace AbpWebProject.Application
{
    [DependsOn(
        typeof(AbpWebProjectDomainMobule),
        typeof(AbpWebProjectEntityFrameworkModule),
        typeof(AbpWebProjectApplicationContractsModule)
        )]
    public class AbpWebProjectApplicationModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            // 对象映射
            Configure<AbpAutoMapperOptions>(options =>
            {
                options.AddMaps<AbpWebProjectApplicationModule>();
            });
        }
    }
}
