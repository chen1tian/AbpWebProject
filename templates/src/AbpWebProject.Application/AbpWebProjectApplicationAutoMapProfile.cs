using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.AutoMapper;

namespace AbpWebProject.Application
{
    public class AbpWebProjectApplicationAutoMapProfile : Profile
    {
        public AbpWebProjectApplicationAutoMapProfile()
        {
            //CreateMap<Product, ProductDto>();

            //CreateMap<CreateUpdateProductDto, Product>()
            //    .IgnoreAuditedObjectProperties()
            //    .Ignore(x => x.ExtraProperties)
            //    .Ignore(x => x.ConcurrencyStamp)
            //    ;
        }
    }
}
