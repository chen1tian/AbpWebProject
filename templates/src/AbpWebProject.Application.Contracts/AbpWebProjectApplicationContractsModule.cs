using AbpWebProject.Domain;
using System;
using Volo.Abp.Application;
using Volo.Abp.Modularity;
using Volo.Abp.SettingManagement;

namespace AbpWebProject.Application.Contracts
{
    [DependsOn(
        typeof(AbpDddApplicationContractsModule),
        typeof(AbpWebProjectDomainMobule),
        typeof(AbpSettingManagementApplicationContractsModule)
        )]
    public class AbpWebProjectApplicationContractsModule : AbpModule
    {
    }
}
