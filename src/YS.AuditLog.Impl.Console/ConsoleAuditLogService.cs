using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace YS.AuditLog.Impl.Console
{
    [YS.Knife.Service(Lifetime = ServiceLifetime.Singleton)]
    public class ConsoleAuditLogService : IAuditLogService
    {
        private readonly ILogger<ConsoleAuditLogService> logger;

        public ConsoleAuditLogService(ILogger<ConsoleAuditLogService> logger)
        {
            this.logger = logger;
        }
        public Task LogRecord(AuditLogRecord logRecord)
        {
            var content = JsonSerializer.Serialize(logRecord);
            logger.LogInformation($"Receive auditLog {content}");
            return Task.CompletedTask;
        }
    }
}
