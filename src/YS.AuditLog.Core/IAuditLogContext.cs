using System;

namespace YS.AuditLog
{
    public interface IAuditLogContext
    {
        string UserName { get; }
        string RequestIp { get; }
    }
}
