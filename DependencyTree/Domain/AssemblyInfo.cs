using System.Collections.Generic;
using System.Reflection;
using DependencyTree.Common;

namespace DependencyTree.Domain
{
    public class AssemblyInfo
    {
        public string Name { get; }

        public string Version { get; }

        public List<AssemblyInfo> Children { get; } = new List<AssemblyInfo>();

        public Assembly Assembly { get; }

        public AssemblyInfo(Assembly assembly)
        {
            Assembly = assembly;
            Name = assembly.GetName().Name;
            Version = assembly.GetName().Version.ToString();
        }

        public override bool Equals(object obj)
        {
            var second = obj as AssemblyInfo;
            if (second == null)
                return false;

            var comparer = new AssemblyInfoEqualityComparer();
            return comparer.Equals(this, second);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Name?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ (Version?.GetHashCode() ?? 0);
                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"{Name} ({Version})";
        }
    }
}
