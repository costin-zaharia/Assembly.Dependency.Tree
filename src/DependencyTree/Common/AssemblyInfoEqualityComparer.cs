using System.Collections.Generic;
using DependencyTree.Domain;

namespace DependencyTree.Common
{
    public class AssemblyInfoEqualityComparer : IEqualityComparer<AssemblyInfo>
    {
        public bool Equals(AssemblyInfo first, AssemblyInfo second)
        {
            return string.Equals(first.Name, second.Name) && string.Equals(first.Version, second.Version);
        }

        public int GetHashCode(AssemblyInfo assemblyInfo)
        {
            unchecked
            {
                var hashCode = assemblyInfo.Name?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ (assemblyInfo.Version?.GetHashCode() ?? 0);
                return hashCode;
            }
        }
    }
}
