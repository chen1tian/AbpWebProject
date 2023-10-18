using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;
using System;
using Volo.Abp.AspNetCore.ExceptionHandling;
using Volo.Abp.Authorization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities;
using Volo.Abp.ExceptionHandling;
using Volo.Abp.Http;
using Volo.Abp.Json;
using Volo.Abp.Validation;
using Volo.Abp;
using Microsoft.AspNetCore.Mvc.Abstractions;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using AbpWebProject.Domain.ApiResults;

namespace AbpWebProject.WebApi.Filter
{
    /// <summary>
    /// 结果异常过滤器
    /// </summary>
    public class ResultExceptionFilter : IFilterMetadata, IAsyncExceptionFilter, ITransientDependency
    {
        private ILogger<ResultExceptionFilter> Logger { get; set; }
        private readonly IExceptionToErrorInfoConverter _errorInfoConverter;
        private readonly IHttpExceptionStatusCodeFinder _statusCodeFinder;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly AbpExceptionHandlingOptions _exceptionHandlingOptions;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="errorInfoConverter"></param>
        /// <param name="statusCodeFinder"></param>
        /// <param name="jsonSerializer"></param>
        /// <param name="exceptionHandlingOptions"></param>
        public ResultExceptionFilter(
            IExceptionToErrorInfoConverter errorInfoConverter,
            IHttpExceptionStatusCodeFinder statusCodeFinder,
            IJsonSerializer jsonSerializer,
            IOptions<AbpExceptionHandlingOptions> exceptionHandlingOptions,
            ILogger<ResultExceptionFilter> logger
            )
        {
            _errorInfoConverter = errorInfoConverter;
            _statusCodeFinder = statusCodeFinder;
            _jsonSerializer = jsonSerializer;
            _exceptionHandlingOptions = exceptionHandlingOptions.Value;
            Logger = logger;
        }

        /// <summary>
        /// 发生异常时
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task OnExceptionAsync(ExceptionContext context)
        {
            //if (!ShouldHandleException(context))
            //{
            //    return;
            //}
            await HandleAndWrapException(context);
        }

        /// <summary>
        /// 是否应该处理异常
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected virtual bool ShouldHandleException(ExceptionContext context)
        {
            if (context.ActionDescriptor.AsControllerActionDescriptor().ControllerTypeInfo.GetCustomAttributes(typeof(WrapResultAttribute), true).Any())
            {
                return true;
            }
            if (context.ActionDescriptor.GetMethodInfo().GetCustomAttributes(typeof(WrapResultAttribute), true).Any())
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 处理并包装异常
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected virtual async Task HandleAndWrapException(ExceptionContext context)
        {
            // 处理异常信息
            context.HttpContext.Response.Headers.Add(AbpHttpConsts.AbpErrorFormat, "true");
            var statusCode = (int)_statusCodeFinder.GetStatusCode(context.HttpContext, context.Exception);
            context.HttpContext.Response.StatusCode = 200;
            var remoteServiceErrorInfo = _errorInfoConverter.Convert(context.Exception, _exceptionHandlingOptions.SendExceptionsDetailsToClients);
            remoteServiceErrorInfo.Code = context.HttpContext.TraceIdentifier;
            remoteServiceErrorInfo.Message = SimplifyMessage(context.Exception);
            // 返回格式统一
            var result = new WrapResult<object>();
            result.SetFail(remoteServiceErrorInfo.Message);
            // HttpResponse
            context.Result = new ObjectResult(result);
            // 写日志
            var logLevel = context.Exception.GetLogLevel();
            var remoteServiceErrorInfoBuilder = new StringBuilder();
            remoteServiceErrorInfoBuilder.AppendLine($"---------- {nameof(RemoteServiceErrorInfo)} ----------");
            remoteServiceErrorInfoBuilder.AppendLine(_jsonSerializer.Serialize(remoteServiceErrorInfo, indented: true));
            Logger.LogWithLevel(logLevel, remoteServiceErrorInfoBuilder.ToString());
            Logger.LogException(context.Exception, logLevel);
            await context.HttpContext
                .RequestServices
                .GetRequiredService<IExceptionNotifier>()
                .NotifyAsync(
                    new ExceptionNotificationContext(context.Exception)
                );
            context.Exception = null; //Handled!
        }

        /// <summary>
        /// 简单异常消息
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        private string SimplifyMessage(Exception error)
        {
            string message = string.Empty;
            switch (error)
            {
                case AbpAuthorizationException e:
                    return message = "认证失败";
                case AbpValidationException e:
                    return message = e.ValidationErrors.FirstOrDefault()?.ErrorMessage ?? "参数验证失败";
                case EntityNotFoundException e:
                    return message = "未找到实体！";
                case BusinessException e:
                    return message = $"{e.Message}";
                case NotImplementedException e:
                    return message = "未实现！";
                default:
                    return message = "服务内部错误！";
            }
        }
    }
}
