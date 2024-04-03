using AbpWebProject.Domain.ApiResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Net;
using System.Text.Json.Serialization;
using System.Text.Json;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Json;
using System.Net.Http.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AbpWebProject.WebApi.Filter
{
    public class ResultFilter : IResultFilter, ITransientDependency
    {
        public void OnResultExecuting(ResultExecutingContext context)
        {
            // 如果是page 直接return
            if (context.ActionDescriptor.IsPageAction()) return;

            // 含不包装结果的特性，直接return
            var controllerHasDontWrapResultAttribute = HasControllerAttribute<DontWrapResultAttribute>(context);
            var ActionHasDontWrapResultAttribute = HasActionAttribute<DontWrapResultAttribute>(context);
            if (controllerHasDontWrapResultAttribute || ActionHasDontWrapResultAttribute) return;

            // 含有包装结果的特性，包装
            var controllerHasWrapResultAttribute = HasControllerAttribute<WrapResultAttribute>(context);
            var controllerActionHasWrapResultAttribute = HasActionAttribute<WrapResultAttribute>(context);

            if (controllerHasWrapResultAttribute || controllerActionHasWrapResultAttribute)
            {
                context.HttpContext.Response.StatusCode = 200;
                var result = new WrapResult<object>();
                if (context.Result is not EmptyResult)
                {
                    result.SetSuccess(((ObjectResult)context.Result).Value);
                }

                var content = JsonConvert.SerializeObject(result, new JsonSerializerSettings()
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });

                context.Result = new ContentResult()
                {
                    StatusCode = (int)HttpStatusCode.OK,
                    ContentType = "application/json;charset=utf-8",
                    Content = content
                };
            }
        }

        public void OnResultExecuted(ResultExecutedContext context)
        {
        }

        #region 私有方法
        /// <summary>
        /// Controller上是否有特性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        private static bool HasControllerAttribute<T>(ResultExecutingContext context)
            where T : Attribute
        {
            return context.ActionDescriptor.AsControllerActionDescriptor().ControllerTypeInfo.GetCustomAttributes(typeof(T), true).Any();
        }

        /// <summary>
        /// Controller上是否有特性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        private static bool HasActionAttribute<T>(ResultExecutingContext context)
            where T : Attribute
        {
            return context.ActionDescriptor.GetMethodInfo().GetCustomAttributes(typeof(T), true).Any();
        }
        #endregion
    }
}
