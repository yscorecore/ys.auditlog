using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YS.AuditLog
{
    public class ExceptionInfo
    {
        public string Type { get; set; }
        public string Message { get; set; }
        public string Stack { get; set; }
        public ExceptionInfo Inner { get; set; }
        public List<ExceptionInfo> Inners { get; set; }

        public static ExceptionInfo FromException(Exception exception, int maxDepth = 20)
        {
            if (exception == null || maxDepth == 0) return null;
            if (exception is AggregateException aggregateException)
            {
                var flatten = aggregateException.Flatten();
                if (flatten.InnerExceptions.Count == 1)
                {
                    return FromException(flatten.InnerExceptions[0], maxDepth);
                }
                else
                {
                    //mutil exceptions
                    return new ExceptionInfo()
                    {
                        Type = flatten.GetType().FullName,
                        Message = flatten.Message,
                        Stack = flatten.StackTrace,
                        Inners = flatten.InnerExceptions.Select(ex => FromException(ex, maxDepth - 1)).Where(e => e != null).ToList()
                    };
                }
            }

            return new ExceptionInfo()
            {
                Message = exception.Message,
                Type = exception.GetType().FullName,
                Stack = exception.StackTrace,
                Inner = FromException(exception.InnerException, maxDepth - 1)
            };
        }
    }
}
