using System.Reflection;

namespace DependencyTree.Services.Cache
{
    public interface IAssemblyCache
    {
        void Add(Assembly assembly);

        Assembly Get(AssemblyName assemblyName);
    }
}