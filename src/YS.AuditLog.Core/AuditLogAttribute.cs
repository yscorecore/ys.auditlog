using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AspectCore.DynamicProxy;
using Microsoft.Extensions.Hosting;
using YS.Knife.Aop;
using YS.Time;
using Microsoft.Extensions.DependencyInjection;

namespace YS.AuditLog
{
    public class AuditLogAttribute : BaseAopAttribute
    {
        public string ApplicationCode { get; }
        public string ModuleCode { get; }
        public string OperationCode { get; }
        
        public string Message { get; }

        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            _ = context ?? throw new ArgumentNullException(nameof(context));
            _ = next ?? throw new ArgumentNullException(nameof(next));
            IAuditLogService auditLogService = context.ServiceProvider.GetRequiredService<IAuditLogService>();
            ITimeService timeService = context.ServiceProvider.GetRequiredService<ITimeService>();

            var record = new AuditLogRecord
            {
                ApplicationCode =this.GetApplicationCode(context),
                ModuleCode = this.GetModuleCode(context),
                OperationCode = this.GetOperationCode(context),
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
                record.Message = e.Message;
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

        protected virtual  string GetApplicationCode(AspectContext context)
        {
            if (string.IsNullOrEmpty(this.ApplicationCode))
            {
                return context?.ServiceProvider.GetRequiredService<IHostEnvironment>().ApplicationName;
            }
            return this.ApplicationCode;
        }

        protected virtual  string GetModuleCode(AspectContext context)
        {
            if (string.IsNullOrEmpty(this.ModuleCode))
            {
                return context?.ServiceMethod.DeclaringType?.FullName;
            }
            return this.ModuleCode;
        }

        protected virtual string GetOperationCode(AspectContext context)
        {
            if (string.IsNullOrEmpty(this.OperationCode))
            {
                return context?.ServiceMethod.Name;
            }
            return this.OperationCode;
        }
    }
}
