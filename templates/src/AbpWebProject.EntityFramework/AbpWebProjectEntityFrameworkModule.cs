﻿using AbpWebProject.Domain;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.Settings;

namespace AbpWebProject.EntityFramework
{
    [DependsOn(
        typeof(AbpWebProjectDomainMobule),
        typeof(AbpMultiTenancyModule),
        typeof(AbpEntityFrameworkCoreModule),
        typeof(AbpAutofacModule),
        typeof(AbpSettingManagementEntityFrameworkCoreModule)
        )]
    public class AbpWebProjectEntityFrameworkModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddAbpDbContext<AbpWebProjectDbContext>(options =>
            {
                /* Remove "includeAllEntities: true" to create
                 * default repositories only for aggregate roots */
                options.AddDefaultRepositories(includeAllEntities: true);
            });            
        }
    }
}
