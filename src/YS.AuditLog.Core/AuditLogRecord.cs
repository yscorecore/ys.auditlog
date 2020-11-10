using System;
using System.Collections.Generic;

namespace YS.AuditLog
{
    public class AuditLogRecord
    {
        public string Id { get; set; }

        public string Operator { get; set; }

        public string ApplicationName { get; set; }

        public string FunctionName { get; set; }

        public string OperationName { get; set; }

        public string RequestIp { get; set; }

        public string Message { get; set; }

        public bool Success { get; set; }

        public DateTimeOffset StartTime { get; set; }

        public DateTimeOffset EndTime { get; set; }

        public Dictionary<string, object> Arguments { get; set; }

        public object Result { get; set; }

    }
}
