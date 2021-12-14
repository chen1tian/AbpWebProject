using AbpWebProject.Domain;
using AbpWebProject.EntityFramework;
using System;
using Volo.Abp.Application;
using Volo.Abp.Modularity;

namespace AbpWebProject.Application.Contracts
{
    [DependsOn(
        typeof(AbpDddApplicationContractsModule),
        typeof(AbpWebProjectDomainMobule),
        typeof(AbpWebProjectEntityFrameworkModule)        
        )]
    public class AbpWebProjectApplicationContractsModule : AbpModule
    {
    }
}
