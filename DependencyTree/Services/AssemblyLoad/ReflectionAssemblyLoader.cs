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
            var assembly = LoadByName(assemblyName.FullName);
            if (assembly != null)
                return assembly;

            assembly = LoadFromPath(assemblyName, path);
            if (assembly != null)
                return assembly;

            return LoadFromUserSelectedPath(assemblyName);
        }

        private Assembly LoadByName(string fullName)
        {
            return _assemblyWrapper.TryReflectionOnlyLoad(fullName);
        }

        private Assembly LoadFromPath(AssemblyName assemblyName, string path)
        {
            var fileName = Path.Combine(path, assemblyName.Name + ".dll");

            return _assemblyWrapper.TryReflectionOnlyLoadFrom(assemblyName, fileName);
        }

        private Assembly LoadFromUserSelectedPath(AssemblyName assemblyName)
        {
            _notificationService.ShowMessage($"Could not find {assemblyName.Name}: {assemblyName.Version}. Please select the file!", $"{assemblyName.Name}: {assemblyName.Version}");

            var fileName = _openFileService.GetSelectedFile();
            return string.IsNullOrEmpty(fileName)
                ? null
                : _assemblyWrapper.TryReflectionOnlyLoadFrom(assemblyName, fileName);
        }
    }
}
