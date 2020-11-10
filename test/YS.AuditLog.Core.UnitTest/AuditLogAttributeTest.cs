using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using YS.AuditLog.Core.UnitTest.Services;
using YS.Knife.Hosting;

namespace YS.AuditLog.Core.UnitTest
{
    [TestClass]
    public class AuditLogAttributeTest : KnifeHost
    {
        IAuditLogService mockedAuditLog = Moq.Mock.Of<IAuditLogService>();
        protected override void OnConfigureCustomService(HostBuilderContext builder, IServiceCollection serviceCollection)
        {
            base.OnConfigureCustomService(builder, serviceCollection);
            serviceCollection.AddSingleton(mockedAuditLog);
        }

        [TestMethod]
        public void ShouldInvokeFunctionAndWriteSuccessAuditLog()
        {
            var service = this.GetService<ITestServiceOne>();
            service.Function(10, 20);
            Mock.Get(mockedAuditLog).Verify(p => p.LogRecord(IsSuccessAuditLog("some function", 30)), Times.Once);
        }


        [TestMethod]
        public void ShouldInvokeSubAndWriteSuccessAuditLog()
        {
            var service = this.GetService<ITestServiceOne>();
            service.Sub(10, 20);
            Mock.Get(mockedAuditLog).Verify(p => p.LogRecord(IsSuccessAuditLog("some sub", null)), Times.Once);
        }

        [TestMethod]
        public async Task ShouldInvokeTaskFunctionAndWriteSuccessAuditLog()
        {
            var service = this.GetService<ITestServiceOne>();
            var val = await service.TaskFunction(10, 20);
            Mock.Get(mockedAuditLog).Verify(p => p.LogRecord(IsSuccessAuditLog("some task function", 30)), Times.Once);
        }
        [TestMethod]
        public async Task ShouldInvokeTaskSubAndWriteSuccessAuditLog()
        {
            var service = this.GetService<ITestServiceOne>();
            await service.TaskSub(10, 20);
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentOutOfRangeException))]
        public void ShouldInvokeFunctionWhenExceptionAndWriteFailureAuditLog()
        {
            var service = this.GetService<ITestServiceOne>();
            service.Function(-10, -20);
            Mock.Get(mockedAuditLog).Verify(p => p.LogRecord(IsFailureAuditLog()), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentOutOfRangeException))]
        public void ShouldInvokeSubWhenExceptionAndWriteFailureAuditLog()
        {
            var service = this.GetService<ITestServiceOne>();
            service.Sub(-10, -20);
            Mock.Get(mockedAuditLog).Verify(p => p.LogRecord(IsFailureAuditLog()), Times.Once);
        }
        [TestMethod]
        [ExpectedException(typeof(System.ArgumentOutOfRangeException))]
        public async Task ShouldInvokeTaskFunctionWhenExceptionAndWriteFailureAuditLog()
        {
            var service = this.GetService<ITestServiceOne>();
            await service.TaskFunction(-10, -20);
            Mock.Get(mockedAuditLog).Verify(p => p.LogRecord(IsFailureAuditLog()), Times.Once);
        }
        [TestMethod]
        [ExpectedException(typeof(System.ArgumentOutOfRangeException))]
        public async Task ShouldInvokeTaskSubWhenExceptionAndWriteFailureAuditLog()
        {
            var service = this.GetService<ITestServiceOne>();
            await service.TaskSub(-10, -20);
            Mock.Get(mockedAuditLog).Verify(p => p.LogRecord(IsFailureAuditLog()), Times.Once);
        }

        private AuditLogRecord IsSuccessAuditLog(string message, object result)
        {
            return It.Is<AuditLogRecord>(p =>
                 p.Success == true
                 && p.EndTime > p.StartTime
                 && p.ApplicationName != null
                 && p.FunctionName != null
                 && p.OperationName != null
                 && p.Message == message
                 && p.Arguments != null
                 && Equals(p.Result, result));
        }
        private AuditLogRecord IsFailureAuditLog()
        {
            return It.Is<AuditLogRecord>(p =>
                 p.Success == false
                 && p.EndTime > p.StartTime
                 && p.ApplicationName != null
                 && p.FunctionName != null
                 && p.OperationName != null
                 && p.Message != null
                 && p.Arguments != null
                 && p.Result.GetType() == typeof(ExceptionInfo));
        }
    }
}
