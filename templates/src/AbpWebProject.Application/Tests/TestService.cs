using AbpWebProject.Application.Contracts.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace AbpWebProject.Application.Tests
{
    public class TestService : ApplicationService, Volo.Abp.DependencyInjection.ITransientDependency
    {
        private readonly IDaprClientProxy<IProductService> _productService;

        public TestService (IDaprClientProxy<IProductService> productService)
        {
            _productService = productService;
        }

        public async Task TestProduct(int id = 0)
        {
            await _productService.Service.GetAsync(id);
        }
    }
}
