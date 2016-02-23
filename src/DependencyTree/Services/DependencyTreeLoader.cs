using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using DependencyTree.Common;
using DependencyTree.Domain;
using DependencyTree.Services.AssemblyLoad;
using DependencyTree.Services.Cache;

namespace DependencyTree.Services
{
    [Export(typeof(IDependencyTreeLoader))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class DependencyTreeLoader : IDependencyTreeLoader
    {
        private readonly IReflectionAssemblyLoader _reflectionAssemblyLoader;
        private readonly IAssemblyCache _assemblyCache;

        // TODO: These assemblyes can have cyclic dependencies. We need to handle this, but for now, we will skip searching of dependencies for them.
        private readonly List<string> _ignoreList = new List<string> { "System", "IKVM", "Presentation" };
        private readonly List<string> _userIgnored = new List<string> { "DevExpress" };

        [ImportingConstructor]
        public DependencyTreeLoader(IReflectionAssemblyLoader reflectionAssemblyLoader, IAssemblyCache assemblyCache)
        {
            _reflectionAssemblyLoader = reflectionAssemblyLoader;
            _assemblyCache = assemblyCache;
        }

        public AssemblyInfo LoadDependencyTree(string filePath)
        {
            var root = new AssemblyInfo(Assembly.ReflectionOnlyLoadFrom(filePath));

            LoadDependencyTree(root, Path.GetDirectoryName(filePath));

            return root;
        }

        private void LoadDependencyTree(AssemblyInfo assemblyInfo, string path)
        {
            foreach (var referencedAssembly in assemblyInfo.Assembly.GetReferencedAssemblies().OrderBy(a => a.Name))
            {
                if (_userIgnored.Any(referencedAssembly.Name.StartsWith))
                    continue;

                var childAssembly = LoadAssembly(referencedAssembly, path);
                if (childAssembly == null || assemblyInfo.Children.Contains(childAssembly, new AssemblyInfoEqualityComparer()))
                    continue;

                if (CanIgnore(childAssembly.Name))
                    continue;

                assemblyInfo.Children.Add(childAssembly);

                LoadDependencyTree(childAssembly, path);
            }
        }

        private AssemblyInfo LoadAssembly(AssemblyName assemblyName, string path)
        {
            var childAssembly = _assemblyCache.Get(assemblyName);

            if(childAssembly != null)
                return new AssemblyInfo(childAssembly);

            childAssembly = _reflectionAssemblyLoader.LoadAssembly(assemblyName, path);

            if (childAssembly == null)
            {
                _userIgnored.Add(assemblyName.Name);
                return null;
            }

            _assemblyCache.Add(childAssembly);

            return new AssemblyInfo(childAssembly);
        }

        private bool CanIgnore(string assemblyName)
        {
            return _ignoreList.Any(assemblyName.StartsWith);
        }
    }
}
