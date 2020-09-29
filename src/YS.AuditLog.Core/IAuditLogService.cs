using System.Threading.Tasks;

namespace YS.AuditLog.Core
{
    public interface IAuditLogService
    {
        Task LogRecord(AuditLogRecord logRecord);
    }
}
