using System;
using System.Threading.Tasks;
using AspectCore.DynamicProxy;
using YS.Knife.Aop;

namespace YS.AuditLog
{
    public class AuditLogAttribute:BaseAopAttribute
    {
        public override Task Invoke(AspectContext context, AspectDelegate next)
        {
            DateTimeOffset startTime = DateTimeOffset.Now;
            
           return Task.CompletedTask;
        }
    }
}
