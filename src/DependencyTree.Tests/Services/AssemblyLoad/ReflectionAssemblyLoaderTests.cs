using System.Reflection;
using DependencyTree.Services.AssemblyLoad;
using DependencyTree.Services.Notifications;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DependencyTree.Tests.Services.AssemblyLoad
{
    [TestClass]
    public class ReflectionAssemblyLoaderTests
    {
        private const string AssemblyFileName = "Microsoft.VisualStudio.TestPlatform.Extensions.VSTestIntegration.dll";

        [TestMethod]
        public void GivenAssemblyCanBeLoadedByName_WhenLoadAssemblyIsCalled_ItReturnsAssembly()
        {
            const string assemblyName = "testAssembly";

            var assembly = Assembly.GetCallingAssembly();
            var assemblyWrapper = Mock.Of<IAssemblyWrapper>(mock => mock.TryReflectionOnlyLoad(assemblyName) == assembly);

            var sut = new ReflectionAssemblyLoader(assemblyWrapper, Mock.Of<IOpenFileService>(), Mock.Of<INotificationService>());
            var result = sut.LoadAssembly(new AssemblyName { Name = assemblyName }, string.Empty);

            Assert.AreEqual(assembly, result);
        }

        [TestMethod]
        public void GivenAssemblyCanNotBeLoadedByNameButFromPath_WhenLoadAssemblyIsCalled_ItReturnsAssembly()
        {
            var assembly = Assembly.GetCallingAssembly();
            var assemblyName = assembly.GetName();

            var assemblyWrapperMock = new Mock<IAssemblyWrapper>();
            assemblyWrapperMock.Setup(mock => mock.TryReflectionOnlyLoad(It.IsAny<string>())).Returns<Assembly>(null);
            assemblyWrapperMock.Setup(mock => mock.TryReflectionOnlyLoadFrom(assemblyName, AssemblyFileName)).Returns(assembly);

            var sut = new ReflectionAssemblyLoader(assemblyWrapperMock.Object, Mock.Of<IOpenFileService>(), Mock.Of<INotificationService>());
            var result = sut.LoadAssembly(assemblyName, string.Empty);

            Assert.AreEqual(assembly, result);
        }

        [TestMethod]
        public void GivenAssemblyCanNotBeLoadedByNameOrFromPathButFromUserSelectedLocation_WhenLoadAssemblyIsCalled_ItReturnsAssembly()
        {
            var assembly = Assembly.GetCallingAssembly();
            var assemblyName = assembly.GetName();
            var selectedFileName = $"User.{AssemblyFileName}";

            var assemblyWrapperMock = new Mock<IAssemblyWrapper>();
            assemblyWrapperMock.Setup(mock => mock.TryReflectionOnlyLoad(It.IsAny<string>())).Returns<Assembly>(null);
            assemblyWrapperMock.Setup(mock => mock.TryReflectionOnlyLoadFrom(assemblyName, AssemblyFileName)).Returns<Assembly>(null);
            assemblyWrapperMock.Setup(mock => mock.TryReflectionOnlyLoadFrom(assemblyName, selectedFileName)).Returns(assembly);

            var openFileService = Mock.Of<IOpenFileService>(mock => mock.GetSelectedFile(It.IsAny<string>()) == selectedFileName);

            var sut = new ReflectionAssemblyLoader(assemblyWrapperMock.Object, openFileService, Mock.Of<INotificationService>());
            var result = sut.LoadAssembly(assemblyName, string.Empty);

            Assert.AreEqual(assembly, result);
        }

        [TestMethod]
        public void GivenAssemblyCanNotFound_WhenLoadAssemblyIsCalled_ItReturnsNull()
        {
            var assemblyWrapperMock = new Mock<IAssemblyWrapper>();
            assemblyWrapperMock.Setup(mock => mock.TryReflectionOnlyLoad(It.IsAny<string>())).Returns<Assembly>(null);
            assemblyWrapperMock.Setup(mock => mock.TryReflectionOnlyLoadFrom(It.IsAny<AssemblyName>(), It.IsAny<string>())).Returns<Assembly>(null);

            var openFileService = Mock.Of<IOpenFileService>(mock => mock.GetSelectedFile(It.IsAny<string>()) == AssemblyFileName);

            var sut = new ReflectionAssemblyLoader(assemblyWrapperMock.Object, openFileService, Mock.Of<INotificationService>());
            var result = sut.LoadAssembly(Assembly.GetCallingAssembly().GetName(), string.Empty);

            Assert.IsNull(result);
        }
    }
}
