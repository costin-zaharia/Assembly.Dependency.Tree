using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Reflection;

namespace DependencyTree.Common
{
    [Export(typeof(IEqualityComparer<AssemblyName>))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class AssemblyNameEqualityComparer : IEqualityComparer<AssemblyName>
    {
        public bool Equals(AssemblyName first, AssemblyName second)
        {
            return first.Name == second.Name && first.Version == second.Version;
        }

        public int GetHashCode(AssemblyName assemblyName)
        {
            unchecked
            {
                var hashCode = assemblyName.Name?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ (assemblyName.Version?.GetHashCode() ?? 0);
                return hashCode;
            }
        }
    }
}
