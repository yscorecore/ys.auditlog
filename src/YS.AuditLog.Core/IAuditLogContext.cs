using System;

namespace YS.AuditLog
{
    public interface IAuditLogContext
    {
        string CurrentUserName();
    }
}
