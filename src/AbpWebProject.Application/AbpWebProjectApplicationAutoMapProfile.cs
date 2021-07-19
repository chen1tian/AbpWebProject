using AbpWebProject.Application.Contracts.Products;
using AbpWebProject.Domain.Domains;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbpWebProject.Application
{
    public class AbpWebProjectApplicationAutoMapProfile : Profile
    {
        public AbpWebProjectApplicationAutoMapProfile()
        {
            CreateMap<Product, ProductDto>();
            CreateMap<CreateUpdateProductDto, Product>();
        }
    }
}
