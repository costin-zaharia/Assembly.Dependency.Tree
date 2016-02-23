using System.Reflection;

namespace DependencyTree.Services
{
    public interface IAssemblyCache
    {
        void AddToCache(Assembly assembly);

        string GetCacheFilePath(AssemblyName assemblyName);
    }
}