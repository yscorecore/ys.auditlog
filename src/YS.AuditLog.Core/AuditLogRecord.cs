using System;
using System.Collections.Generic;
using System.Security.Principal;

namespace YS.AuditLog
{
    public class AuditLogRecord
    {
        public string Id { get; set; }
        
        public string Operator { get; set; }
        
        public string Category { get; set; }
        
        public string OperationCode { get; set; }

        public bool Success { get; set; }
        
        public DateTimeOffset StartTime { get; set; }
        
        public DateTimeOffset EndTime { get; set; }

        public Dictionary<string, Object> Arguments { get; set; }

        public Object Result { get; set; }
        
    }
}
