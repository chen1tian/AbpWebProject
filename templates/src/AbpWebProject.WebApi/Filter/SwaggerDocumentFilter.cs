using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AbpWebProject.WebApi.Filter
{
    public class SwaggerDocumentFilter : IDocumentFilter
    {
        private readonly string _basePath;

        public SwaggerDocumentFilter(string basePath)
        {
            _basePath = basePath;
        }
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            swaggerDoc.Servers.Add(new OpenApiServer() { Url = _basePath });
        }
    }
}
