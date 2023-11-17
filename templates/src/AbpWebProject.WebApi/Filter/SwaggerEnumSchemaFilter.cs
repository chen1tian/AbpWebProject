using AbpWebProject.Domain.Extensions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Linq;

namespace AbpWebProject.WebApi.Filter
{
    public class SwaggerEnumSchemaFilter : ISchemaFilter
    {
        /// <summary>
        /// 实现接口
        /// </summary>
        /// <param name="model"></param>
        /// <param name="context"></param>

        public void Apply(OpenApiSchema model, SchemaFilterContext context)
        {
            if (context.Type.IsEnum)
            {
                model.Enum.Clear();
                Enum.GetNames(context.Type)
                    .ToList()
                    .ForEach(name =>
                    {
                        Enum e = (Enum)Enum.Parse(context.Type, name);
                        model.Enum.Add(new OpenApiString($"{name}({e.GetDescription()})={Convert.ToInt64(Enum.Parse(context.Type, name))}"));
                    });
            }
        }
    }
}
