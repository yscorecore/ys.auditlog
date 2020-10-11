using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AspectCore.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using YS.Knife.Aop;
using YS.Time;

namespace YS.AuditLog
{
    public class AuditLogAttribute : BaseAopAttribute
    {
        public AuditLogAttribute(string message)
        {
            this.Message = message;
        }
        public string Message { get; set; }
        public string OperationName { get; set; }
        public string ApplicationName { get; set; }
        public string FunctionName { get; set; }
        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            _ = context ?? throw new ArgumentNullException(nameof(context));
            _ = next ?? throw new ArgumentNullException(nameof(next));
            IAuditLogService auditLogService = context.ServiceProvider.GetRequiredService<IAuditLogService>();
            ITimeService timeService = context.ServiceProvider.GetRequiredService<ITimeService>();

            var record = new AuditLogRecord
            {
                ApplicationName =
                    this.ApplicationName ?? context.ServiceProvider.GetRequiredService<IHostEnvironment>().ApplicationName,
                FunctionName = this.FunctionName ?? context.ImplementationMethod.DeclaringType.FullName,
                OperationName = this.OperationName ?? context.ServiceMethod.Name,
                StartTime = await timeService.Current(),
                Operator = Thread.CurrentPrincipal?.Identity?.Name,
                RequestIp = "127.0.0.1",
                Arguments = buildInputArguments(context)
            };
            try
            {
                await next.Invoke(context);
                record.Success = true;
                record.Message = this.BuildMessage(context);
                record.EndTime = await timeService.Current();
                record.Result = buildResult(context);
                await auditLogService.LogRecord(record);
            }
            catch (Exception e)
            {
                record.Message = e.Message;
                record.Success = false;
                record.EndTime = await timeService.Current();
                record.Result = ExceptionInfo.FromException(e);
                await auditLogService.LogRecord(record);
                throw;
            }
        }

        private Dictionary<string, object> buildInputArguments(AspectContext context)
        {
            return context.ServiceMethod.GetParameters()
                .Zip(context.Parameters, (k, v) => new KeyValuePair<string, object>(k.Name, v))
                 .ToDictionary(p => p.Key, p => p.Value);
        }

        private object buildResult(AspectContext context)
        {
            return context.IsAsync() ? context.UnwrapAsyncReturnValue().Result : context.ReturnValue;
        }
        private string BuildMessage(AspectContext _)
        {
            return this.Message;
        }
    }
}
