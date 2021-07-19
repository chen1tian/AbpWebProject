using AbpWebProject.Application.Contracts.Products;
using AbpWebProject.Domain.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace AbpWebProject.Application.Products
{
    public class ProductService : CrudAppService<Product, ProductDto, int, PagedAndSortedResultRequestDto, CreateUpdateProductDto>, IProductService
    {
        public ProductService(IRepository<Product, int> repository) : base(repository)
        {
           
        }

        public override Task<PagedResultDto<ProductDto>> GetListAsync(PagedAndSortedResultRequestDto input)
        {
            return base.GetListAsync(input);
        }
    }
}
