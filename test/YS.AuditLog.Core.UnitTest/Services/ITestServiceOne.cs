using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace YS.AuditLog.Core.UnitTest.Services
{
    public interface ITestServiceOne
    {
        int Function(int a, int b);

        void Sub(int a, int b);

        Task TaskSub(int a, int b);

        Task<int> TaskFunction(int a, int b);
    }

    [YS.Knife.Service]
    public class TestServiceOne : ITestServiceOne
    {
        [AuditLog("some function")]
        public int Function(int a, int b)
        {
            Assert(a);
            Assert(b);
            return a + b;
        }
        [AuditLog("some sub")]
        public void Sub(int a, int b)
        {
            Assert(a);
            Assert(b);
        }
        [AuditLog("some task function")]
        public Task<int> TaskFunction(int a, int b)
        {
            Assert(a);
            Assert(b);

            return Task.FromResult(a + b);
        }
        [AuditLog("some task sub")]
        public Task TaskSub(int a, int b)
        {
            Assert(a);
            Assert(b);
            return Task.CompletedTask;
        }
        private void Assert(int num)
        {
            if (num < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(num));
            }
        }
    }
}
