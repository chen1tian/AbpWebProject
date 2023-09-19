using AbpWebProject.Domain.ApiResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Net;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Json;

namespace AbpWebProject.WebApi.Filter
{
    public class ResultFilter : IResultFilter, ITransientDependency
    {
        public void OnResultExecuting(ResultExecutingContext context)
        {
            // 如果是page 直接return
            if (context.ActionDescriptor.IsPageAction()) return;

            var controllerHasWrapResultAttribute =
                context.ActionDescriptor.AsControllerActionDescriptor().ControllerTypeInfo.GetCustomAttributes(typeof(WrapResultAttribute), true).Any();
            var controllerActionHasWrapResultAttribute = context.ActionDescriptor.GetMethodInfo().GetCustomAttributes(typeof(WrapResultAttribute), true).Any();
            if (controllerHasWrapResultAttribute || controllerActionHasWrapResultAttribute)
            {
                context.HttpContext.Response.StatusCode = 200;
                var result = new WrapResult<object>();
                if (context.Result is not EmptyResult)
                {
                    result.SetSuccess(((ObjectResult)context.Result).Value);
                }

                var jsonSerializer = context.GetService<IJsonSerializer>();

                context.Result = new ContentResult()
                {
                    StatusCode = (int)HttpStatusCode.OK,
                    ContentType = "application/json;charset=utf-8",
                    Content = jsonSerializer.Serialize(result)
                };
            }
        }

        public void OnResultExecuted(ResultExecutedContext context)
        {
        }
    }
}
