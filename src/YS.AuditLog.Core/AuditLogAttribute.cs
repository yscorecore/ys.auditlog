using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AspectCore.DynamicProxy;
using Microsoft.Extensions.Hosting;
using YS.AuditLog.Core;
using YS.Knife.Aop;
using YS.Time;
using Microsoft.Extensions.DependencyInjection;

namespace YS.AuditLog
{
    public class AuditLogAttribute : BaseAopAttribute
    {
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
                Arguments = buildInputArguments(context)
            };
            try
            {
                await next.Invoke(context);
                record.Success = true;
                record.Message = this.BuildMessage(context);
                record.EndTime = await timeService.Current();
                record.Result = buildResult(context.ReturnValue);
                await auditLogService.LogRecord(record);
            }
            catch (Exception e)
            {
                record.Message = e.Message;
                record.Success = false;
                record.EndTime = await timeService.Current();
                record.Result = buildResult(e);
                await auditLogService.LogRecord(record);
                throw;
            }
        }

        private Dictionary<string, object> buildInputArguments(AspectContext context)
        {
            return null;
        }

        private object buildResult(object objectOrException)
        {
            return null;
        }
        private string BuildMessage(AspectContext context)
        {
            throw new NotImplementedException();
        }
    }
}
