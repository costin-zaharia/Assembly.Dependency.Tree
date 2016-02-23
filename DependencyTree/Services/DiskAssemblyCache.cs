using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Reflection;

namespace DependencyTree.Services
{
    [Export(typeof(IAssemblyCache))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class DiskAssemblyCache : IAssemblyCache
    {
        private readonly string _cacheFolder;

        public DiskAssemblyCache()
        {
            _cacheFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Cache");

            if (!Directory.Exists(_cacheFolder))
                Directory.CreateDirectory(_cacheFolder);
        }

        public void AddToCache(Assembly assembly)
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

        public string GetCacheFilePath(AssemblyName assemblyName)
        {
            return Path.Combine(_cacheFolder, assemblyName.Name, assemblyName.Version.ToString(), assemblyName.Name + ".dll");
        }
    }
}
