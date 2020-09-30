using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace YS.AuditLog
{
    public interface IAuditLogService
    {
        Task LogRecord(AuditLogRecord logRecord);
    }
   
}
