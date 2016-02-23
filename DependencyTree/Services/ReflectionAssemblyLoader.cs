﻿using System.ComponentModel.Composition;
using System.IO;
using System.Reflection;
using System.Windows;
using DependencyTree.Domain;

namespace DependencyTree.Services
{
    [Export(typeof(IReflectionAssemblyLoader))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class ReflectionAssemblyLoader : IReflectionAssemblyLoader
    {
        private readonly IOpenFileService _openFileService;
        private readonly IAssemblyCache _assemblyCache;

        [ImportingConstructor]
        public ReflectionAssemblyLoader(IOpenFileService openFileService, IAssemblyCache assemblyCache)
        {
            _openFileService = openFileService;
            _assemblyCache = assemblyCache;
        }

        public AssemblyInfo LoadAssembly(AssemblyName assemblyName, string path)
        {
            try
            {
                return new AssemblyInfo(Assembly.ReflectionOnlyLoad(assemblyName.FullName));
            }
            catch
            {
                var assembly = GetFromCache(assemblyName);
                if (assembly == null)
                {
                    assembly = LoadAssemblyByName(path, assemblyName);
                    _assemblyCache.AddToCache(assembly);
                }

                if (assembly != null)
                    return new AssemblyInfo(assembly);
            }

            return null;
        }

        private Assembly LoadAssemblyByName(string path, AssemblyName assemblyName)
        {
            var fileName = Path.Combine(path, assemblyName.Name + ".dll");
            var assembly = TryToLoadAssemblyByName(fileName, assemblyName);
            if (assembly != null)
                return assembly;

            MessageBox.Show($"Could not find {assemblyName.Name}: {assemblyName.Version}. Please select the file!");

            fileName = _openFileService.GetSelectedFile();
            return string.IsNullOrEmpty(fileName)
                ? null
                : TryToLoadAssemblyByName(fileName, assemblyName);
        }

        private static Assembly TryToLoadAssemblyByName(string fileName, AssemblyName assemblyName)
        {
            try
            {
                var assembly = Assembly.ReflectionOnlyLoadFrom(fileName);
                var loadedAssemblyName = assembly.GetName();
                return loadedAssemblyName.Version == assemblyName.Version && loadedAssemblyName.Name == assemblyName.Name ? assembly : null;
            }
            catch
            {
                return null;
            }
        }

        private Assembly GetFromCache(AssemblyName assemblyName)
        {
            return TryToLoadAssemblyByName(_assemblyCache.GetCacheFilePath(assemblyName), assemblyName);
        }
    }
}
