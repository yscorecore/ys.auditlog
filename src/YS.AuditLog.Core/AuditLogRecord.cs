using System.Collections.Generic;
using System.Security.Principal;

namespace YS.AuditLog
{
    public class AuditLogRecord
    {
        public string Id { get; set; }
        
        public string UserName { get; set; }

        public string Operator { get; set; }
        
        public int LogType { get; set; }

        public string LogCode { get; set; }

        public string Message { get; set; }

        public Dictionary<string, object> Properties { get; set; }
    }
}
