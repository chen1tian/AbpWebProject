using AbpWebProject.Domain;
using System;
using Volo.Abp.Application;
using Volo.Abp.Modularity;

namespace AbpWebProject.Application.Contracts
{
    [DependsOn(
        typeof(AbpDddApplicationContractsModule),
        typeof(AbpWebProjectDomainMobule)
        )]
    public class AbpWebProjectApplicationContractsModule : AbpModule
    {
    }
}
