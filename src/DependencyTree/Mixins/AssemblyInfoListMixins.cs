using System.Collections.Generic;
using System.Text;
using DependencyTree.Domain;

namespace DependencyTree.Mixins
{
    public static class AssemblyInfoListMixins
    {
        public static string ToString(this List<AssemblyInfo> list)
        {
            var builder = new StringBuilder();

            builder.Append("[");

            foreach (var assemblyInfo in list)
            {
                builder.Append($"{assemblyInfo.Name}:{assemblyInfo.Version}, ");
            }

            if (list.Count > 0)
                builder.Remove(builder.Length - 2, 2);

            builder.Append("]");

            return builder.ToString();
        }
    }
}
