using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Reflection;
using DependencyTree.Services.AssemblyLoad;

namespace DependencyTree.Services.Cache
{
    [Export(typeof(IAssemblyCache))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class DiskAssemblyCache : IAssemblyCache
    {
        private readonly string _cacheFolder;
        private readonly IAssemblyWrapper _assemblyWrapper;

        [ImportingConstructor]
        public DiskAssemblyCache(IAssemblyWrapper assemblyWrapper)
        {
            _assemblyWrapper = assemblyWrapper;
            _cacheFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Cache");

            if (!Directory.Exists(_cacheFolder))
                Directory.CreateDirectory(_cacheFolder);
        }

        public void Add(Assembly assembly)
        {
            if (assembly == null)
                return;

            var assemblyName = assembly.GetName();

            if (!Directory.Exists(Path.Combine(_cacheFolder, assemblyName.Name)))
                Directory.CreateDirectory(Path.Combine(_cacheFolder, assemblyName.Name));

            if (!Directory.Exists(Path.Combine(_cacheFolder, assemblyName.Name, assemblyName.Version.ToString())))
                Directory.CreateDirectory(Path.Combine(_cacheFolder, assemblyName.Name, assemblyName.Version.ToString()));

            File.Copy(assembly.Location, Path.Combine(_cacheFolder, GetCacheFilePath(assemblyName)));
        }

        public Assembly Get(AssemblyName assemblyName)
        {
            return _assemblyWrapper.TryReflectionOnlyLoadFrom(assemblyName, GetCacheFilePath(assemblyName));
        }

        private string GetCacheFilePath(AssemblyName assemblyName)
        {
            return Path.Combine(_cacheFolder, assemblyName.Name, assemblyName.Version.ToString(), assemblyName.Name + ".dll");
        }
    }
}
