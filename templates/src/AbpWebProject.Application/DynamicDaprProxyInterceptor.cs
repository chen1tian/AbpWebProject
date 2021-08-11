using Dapr.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Content;
using Volo.Abp.DependencyInjection;
using Volo.Abp.DynamicProxy;
using Volo.Abp.Http;
using Volo.Abp.Http.Client;
using Volo.Abp.Http.Client.Authentication;
using Volo.Abp.Http.Client.DynamicProxying;
using Volo.Abp.Http.Modeling;
using Volo.Abp.Http.ProxyScripting.Generators;
using Volo.Abp.Json;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Threading;
using Volo.Abp.Tracing;

namespace AbpWebProject.Application
{
    public class DynamicDaprProxyInterceptor<TService> : AbpInterceptor, ITransientDependency
    {
        private readonly DaprClient _daprClient;

        public DynamicDaprProxyInterceptor(DaprClient daprClient)
        {
            _daprClient = daprClient;
        }

        public override Task InterceptAsync(IAbpMethodInvocation invocation)
        {
            string appId = null;
            var httpClient = DaprClient.CreateInvokeHttpClient(appId);

            var isGenericMethod = invocation.Method.IsGenericMethod;


            var pArr = invocation.Method.GetParameters().Select(x => x.ParameterType).ToArray();

            var invokeMethod = _daprClient.GetType().GetMethod("CreateInvokeHttpClient", new Type[] {
                                typeof(string),
                typeof(string),
                typeof(string)

            });


            Console.WriteLine(invocation.Method.Name);
            return Task.CompletedTask;
        }
    }
}
