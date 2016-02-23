using System.Reflection;

namespace DependencyTree.Services.AssemblyLoad
{
    public interface IReflectionAssemblyLoader
    {
        Assembly LoadAssembly(AssemblyName assemblyName, string path);
    }
}