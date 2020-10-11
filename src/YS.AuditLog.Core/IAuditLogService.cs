using System;
using System.Threading.Tasks;

namespace YS.AuditLog
{
    public interface IAuditLogService
    {
        Task LogRecord(AuditLogRecord logRecord);
    }


}
