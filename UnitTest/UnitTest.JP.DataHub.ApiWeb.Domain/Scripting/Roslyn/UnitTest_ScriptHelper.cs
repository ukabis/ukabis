using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Unity;
using JP.DataHub.Com.Unity;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.ScriptRuntimeLog;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.ApiWeb.Domain.Scripting.Roslyn;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.ApiWeb.Domain.Scripting.Roslyn
{
    [TestClass()]
    public class UnitTest_ScriptHelper : UnitTestBase
    {
        private Uri _uri = new Uri("https://google.com");
        private Guid _vendorId = Guid.NewGuid();


        [TestInitialize()]
        public override void TestInitialize()
        {
            base.TestInitialize(true);

            UnityContainer.RegisterType<IPerRequestDataContainer, PerRequestDataContainer>("multiThread");
        }


        [TestMethod()]
        public void ScriptHelper_Printf_正常系()
        {
            var mockFileRepository = new Mock<IScriptRuntimeLogFileRepository>();
            mockFileRepository.Setup(x => x.AppendAsync(It.IsAny<ScriptRuntimeLogAppendFile>())).ReturnsAsync(_uri);
            UnityContainer.RegisterInstance<IScriptRuntimeLogFileRepository>(mockFileRepository.Object);

            var logContent = "hogehoge";
            var mockEventPublisher = new Mock<IDomainEventPublisher>();
            mockEventPublisher.Setup(x => x.Publish<ScriptRuntimeLogWriteEventData>(It.IsAny<ScriptRuntimeLogWriteEventData>()))
                .Callback<ScriptRuntimeLogWriteEventData>((data) =>
                {
                    data.AppendFile.ContentType.Value.Is("text/plain");
                    data.AppendFile.FilePath.Value.Is($"{_vendorId.ToString()}\\{data.AppendFile.ScriptRuntimeLogId.ToString()}");
                    data.AppendFile.Name.Value.Is("Runtime.log");
                    Assert.IsTrue(data.AppendFile.AppendContent.Value.EndsWith(logContent + System.Environment.NewLine));
                });
            UnityContainer.RegisterInstance(mockEventPublisher.Object);
            UnityContainer.RegisterInstance<bool>("IsEnableScriptRuntimeLogService", true);

            var helper = new ScriptHelper(_vendorId);
            helper.Printf(logContent);
            mockEventPublisher.Verify(x => x.Publish<ScriptRuntimeLogWriteEventData>(
                It.IsAny<ScriptRuntimeLogWriteEventData>()), Times.Once);
        }

        [TestMethod()]
        public void ScriptHelper_Printf_正常系_本文null()
        {
            var mockFileRepository = new Mock<IScriptRuntimeLogFileRepository>();
            mockFileRepository.Setup(x => x.AppendAsync(It.IsAny<ScriptRuntimeLogAppendFile>())).ReturnsAsync(_uri);
            UnityContainer.RegisterInstance<IScriptRuntimeLogFileRepository>(mockFileRepository.Object);

            var logContent = "hogehoge";
            var mockEventPublisher = new Mock<IDomainEventPublisher>();
            mockEventPublisher.Setup(x => x.Publish<ScriptRuntimeLogWriteEventData>(It.IsAny<ScriptRuntimeLogWriteEventData>()))
                .Callback<ScriptRuntimeLogWriteEventData>((data) =>
                {
                    data.AppendFile.ContentType.Value.Is("text/plain");
                    data.AppendFile.FilePath.Value.Is($"{_vendorId.ToString()}\\{data.AppendFile.ScriptRuntimeLogId.ToString()}");
                    data.AppendFile.Name.Value.Is("Runtime.log");
                    Assert.IsTrue(data.AppendFile.AppendContent.Value.EndsWith(logContent + System.Environment.NewLine));
                });
            UnityContainer.RegisterInstance(mockEventPublisher.Object);
            UnityContainer.RegisterInstance<bool>("IsEnableScriptRuntimeLogService", true);

            var helper = new ScriptHelper(_vendorId);
            helper.Printf("");
            mockEventPublisher.Verify(x => x.Publish<ScriptRuntimeLogWriteEventData>(
                It.IsAny<ScriptRuntimeLogWriteEventData>()), Times.Never);
        }

        [TestMethod()]
        public void ScriptHelper_Printf_正常系_3件()
        {
            var mockFileRepository = new Mock<IScriptRuntimeLogFileRepository>();
            mockFileRepository.Setup(x => x.AppendAsync(It.IsAny<ScriptRuntimeLogAppendFile>())).ReturnsAsync(_uri);
            UnityContainer.RegisterInstance<IScriptRuntimeLogFileRepository>(mockFileRepository.Object);

            var logContent = "hogehoge";
            var mockEventPublisher = new Mock<IDomainEventPublisher>();
            mockEventPublisher.Setup(x => x.Publish<ScriptRuntimeLogWriteEventData>(It.IsAny<ScriptRuntimeLogWriteEventData>()))
                .Callback<ScriptRuntimeLogWriteEventData>((data) =>
                {
                    data.AppendFile.ContentType.Value.Is("text/plain");
                    data.AppendFile.FilePath.Value.Is($"{_vendorId.ToString()}\\{data.AppendFile.ScriptRuntimeLogId.ToString()}");
                    data.AppendFile.Name.Value.Is("Runtime.log");
                    Assert.IsTrue(data.AppendFile.AppendContent.Value.EndsWith(logContent + System.Environment.NewLine));
                });
            UnityContainer.RegisterInstance(mockEventPublisher.Object);
            UnityContainer.RegisterInstance<bool>("IsEnableScriptRuntimeLogService", true);

            var helper = new ScriptHelper(_vendorId);
            helper.Printf(logContent);
            helper.Printf(logContent);
            helper.Printf(logContent);
            mockEventPublisher.Verify(x => x.Publish<ScriptRuntimeLogWriteEventData>(
                It.IsAny<ScriptRuntimeLogWriteEventData>()), Times.Exactly(3));
        }

        [TestMethod]
        public void ScriptHelper_Printf_異常系_config無()
        {
            var mockFileRepository = new Mock<IScriptRuntimeLogFileRepository>();
            mockFileRepository.Setup(x => x.AppendAsync(It.IsAny<ScriptRuntimeLogAppendFile>())).ReturnsAsync(_uri);
            UnityContainer.RegisterInstance<IScriptRuntimeLogFileRepository>(mockFileRepository.Object);

            var logContent = "hogehoge";
            var mockEventPublisher = new Mock<IDomainEventPublisher>();
            mockEventPublisher.Setup(x => x.Publish<ScriptRuntimeLogWriteEventData>(It.IsAny<ScriptRuntimeLogWriteEventData>()))
                .Callback<ScriptRuntimeLogWriteEventData>((data) =>
                {
                    data.AppendFile.ContentType.Value.Is("text/plain");
                    data.AppendFile.FilePath.Value.Is($"{_vendorId.ToString()}\\{data.AppendFile.ScriptRuntimeLogId.ToString()}");
                    data.AppendFile.Name.Value.Is("Runtime.log");
                    Assert.IsTrue(data.AppendFile.AppendContent.Value.EndsWith(logContent + System.Environment.NewLine));
                });
            UnityContainer.RegisterInstance(mockEventPublisher.Object);
            UnityContainer.RegisterInstance<bool>("IsEnableScriptRuntimeLogService", false);

            var helper = new ScriptHelper(_vendorId);
            helper.Printf(logContent);
            mockEventPublisher.Verify(x => x.Publish<ScriptRuntimeLogWriteEventData>(
                It.IsAny<ScriptRuntimeLogWriteEventData>()), Times.Never);
        }
    }
}