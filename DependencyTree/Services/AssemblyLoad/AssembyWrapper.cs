using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Reflection;

namespace DependencyTree.Services.AssemblyLoad
{
    [Export(typeof(IAssemblyWrapper))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class AssemblyWrapper : IAssemblyWrapper
    {
        private readonly IEqualityComparer<AssemblyName> _assemblyNamEqualityComparer;

        [ImportingConstructor]
        public AssemblyWrapper(IEqualityComparer<AssemblyName> assemblyNamEqualityComparer)
        {
            _assemblyNamEqualityComparer = assemblyNamEqualityComparer;
        }

        public Assembly ReflectionOnlyLoadFrom(string path)
        {
            return Assembly.ReflectionOnlyLoadFrom(path);
        }

        public Assembly ReflectionOnlyLoad(string path)
        {
            return Assembly.ReflectionOnlyLoad(path);
        }

        public Assembly TryReflectionOnlyLoadFrom(string fileName, AssemblyName assemblyName)
        {
            try
            {
                var assembly = Assembly.ReflectionOnlyLoadFrom(fileName);

                return _assemblyNamEqualityComparer.Equals(assembly.GetName(), assemblyName) ? assembly : null;
            }
            catch
            {
                return null;
            }
        }
    }
}
