using System.Reflection;
using DependencyTree.Domain;

namespace DependencyTree.Services
{
    public interface IReflectionAssemblyLoader
    {
        AssemblyInfo LoadAssembly(AssemblyName assemblyName, string path);
    }
}