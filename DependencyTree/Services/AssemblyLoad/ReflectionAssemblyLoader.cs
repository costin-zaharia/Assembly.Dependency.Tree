using System.ComponentModel.Composition;
using System.IO;
using System.Reflection;
using DependencyTree.Services.Notifications;

namespace DependencyTree.Services.AssemblyLoad
{
    [Export(typeof(IReflectionAssemblyLoader))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class ReflectionAssemblyLoader : IReflectionAssemblyLoader
    {
        private readonly IOpenFileService _openFileService;
        private readonly IAssemblyWrapper _assemblyWrapper;
        private readonly INotificationService _notificationService;

        [ImportingConstructor]
        public ReflectionAssemblyLoader(IAssemblyWrapper assemblyWrapper, IOpenFileService openFileService,  INotificationService notificationService)
        {
            _assemblyWrapper = assemblyWrapper;
            _openFileService = openFileService;
            _notificationService = notificationService;
        }

        public Assembly LoadAssembly(AssemblyName assemblyName, string path)
        {
            try
            {
                return _assemblyWrapper.ReflectionOnlyLoad(assemblyName.FullName);
            }
            catch
            {
                var assembly = LoadAssembly(path, assemblyName);
                if (assembly != null)
                    return assembly;
            }

            return null;
        }

        private Assembly LoadAssembly(string path, AssemblyName assemblyName)
        {
            var fileName = Path.Combine(path, assemblyName.Name + ".dll");
            var assembly = _assemblyWrapper.TryReflectionOnlyLoadFrom(fileName, assemblyName);
            if (assembly != null)
                return assembly;

            _notificationService.ShowMessage($"Could not find {assemblyName.Name}: {assemblyName.Version}. Please select the file!", $"{assemblyName.Name}: {assemblyName.Version}");

            fileName = _openFileService.GetSelectedFile();
            return string.IsNullOrEmpty(fileName)
                ? null
                : _assemblyWrapper.TryReflectionOnlyLoadFrom(fileName, assemblyName);
        }
    }
}
