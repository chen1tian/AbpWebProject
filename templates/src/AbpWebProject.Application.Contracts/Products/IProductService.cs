using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace AbpWebProject.Application.Contracts.Products
{
    public interface IProductService : ICrudAppService<ProductDto, int, PagedAndSortedResultRequestDto, CreateUpdateProductDto>
    {
    }
}
