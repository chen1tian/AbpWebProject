using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Validation;

namespace AbpWebProject.Domain
{
    [DependsOn(
        typeof(AbpDddDomainModule),
        typeof(AbpValidationModule)
    )]
    public class AbpWebProjectDomainMobule : AbpModule
    {

    }
}
