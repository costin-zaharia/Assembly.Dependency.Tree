using System.Reflection;

namespace DependencyTree.Services.AssemblyLoad
{
    public interface IAssemblyWrapper
    {
        Assembly ReflectionOnlyLoad(string path);

        Assembly ReflectionOnlyLoadFrom(string path);

        Assembly TryReflectionOnlyLoadFrom(string fileName, AssemblyName assemblyName);
    }
}