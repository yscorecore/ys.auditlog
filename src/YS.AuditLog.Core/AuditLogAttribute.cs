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
        public string OperationCode { get;  }
        public string Category { get;  }

        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            _ = context ?? throw new ArgumentNullException(nameof(context));
            _ = next ?? throw new ArgumentNullException(nameof(next));
            IAuditLogService auditLogService = context.ServiceProvider.GetRequiredService<IAuditLogService>();
            ITimeService timeService = context.ServiceProvider.GetRequiredService<ITimeService>();

            var record = new AuditLogRecord
            {
                Category =
                    this.Category ?? context.ServiceProvider.GetRequiredService<IHostEnvironment>().ApplicationName,
                OperationCode = this.OperationCode ?? context.ServiceMethod.Name,
                StartTime = await timeService.Current(),
                Operator = Thread.CurrentPrincipal?.Identity?.Name,
                Arguments = buildInputArguments(context)
            };
            try
            {
                await next.Invoke(context);
                record.Success = true;
                record.EndTime = await timeService.Current();
                record.Result = buildResult(context.ReturnValue);
                await auditLogService.LogRecord(record);
            }
            catch (Exception e)
            {
                record.Success = false;
                record.EndTime = await timeService.Current();
                record.Result = buildResult(e);
                await auditLogService.LogRecord(record);
                // trace;
                throw e;
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
    }
}
