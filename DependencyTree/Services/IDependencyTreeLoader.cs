using DependencyTree.Domain;

namespace DependencyTree.Services
{
    public interface IDependencyTreeLoader
    {
        AssemblyInfo LoadDependencyTree(string filePath);
    }
}