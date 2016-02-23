using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using System.Windows.Input;
using DependencyTree.Annotations;
using DependencyTree.Common;
using DependencyTree.Domain;
using DependencyTree.Mixins;
using DependencyTree.Services;
using DependencyTree.Services.Notifications;
using Prism.Commands;

namespace DependencyTree.Controls
{
    [Export(typeof(DependencyTreeControlViewModel))]
    public class DependencyTreeControlViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly List<AssemblyInfoViewModel> _dependencies = new List<AssemblyInfoViewModel>();
        private readonly IOpenFileService _openFileService;
        private readonly IDependencyTreeLoader _dependencyTreeLoader;

        private AssemblyInfo _root;
        private string _dependenciesFilter;
        private string _fileName;

        [ImportingConstructor]
        public DependencyTreeControlViewModel(IOpenFileService openFileService, IDependencyTreeLoader dependencyTreeLoader)
        {
            _openFileService = openFileService;
            _dependencyTreeLoader = dependencyTreeLoader;

            SelectFileCommand = new DelegateCommand(HandleSelectFileCommand);

            Dependencies = new ListCollectionView(_dependencies)
            {
                Filter = FilterDependencies
            };
        }

        public ICommand SelectFileCommand { get; }

        public ICollectionView Dependencies { get; }

        public AssemblyInfo Root
        {
            get { return _root; }
            private set
            {
                _root = value;
                OnPropertyChanged();
            }
        }

        public string FileName
        {
            get { return _fileName; }
            private set
            {
                _fileName = value;
                OnPropertyChanged();
            }
        }

        public string DependenciesFilter
        {
            get { return _dependenciesFilter; }
            set
            {
                _dependenciesFilter = value;
                Dependencies.Refresh();
            }
        }

        private bool FilterDependencies(object obj)
        {
            if (string.IsNullOrWhiteSpace(DependenciesFilter))
                return true;

            var filter = DependenciesFilter.ToLowerInvariant();

            var viewModel = (AssemblyInfoViewModel)obj;
            return viewModel.Description.ToLowerInvariant().Contains(filter) ||
                   viewModel.DependenciesDescription.ToLowerInvariant().Contains(filter);
        }

        private void HandleSelectFileCommand()
        {
            var selectedFile = _openFileService.GetSelectedFile();
            if (string.IsNullOrEmpty(selectedFile))
                return;

            FileName = Path.GetFileNameWithoutExtension(selectedFile);

            Root = _dependencyTreeLoader.LoadDependencyTree(selectedFile);

            UpdateDependencies();
        }

        private void UpdateDependencies()
        {
            var dependenciesMap = new Dictionary<AssemblyInfo, List<AssemblyInfo>>();

            BuildDependenciesMap(Root, dependenciesMap);

            _dependencies.Clear();

            foreach (var keyValue in dependenciesMap.OrderBy(pair => pair.Key.Name))
            {
                _dependencies.Add(new AssemblyInfoViewModel(keyValue.Key.ToString(), AssemblyInfoListMixins.ToString(keyValue.Value)));
            }

            Dependencies.Refresh();
        }

        private static void BuildDependenciesMap(AssemblyInfo root, IDictionary<AssemblyInfo, List<AssemblyInfo>> dependenciesMap)
        {
            foreach (var currentAssembly in root.Children)
            {
                if(!dependenciesMap.ContainsKey(currentAssembly))
                    dependenciesMap.Add(currentAssembly, new List<AssemblyInfo>());

                if (!dependenciesMap[currentAssembly].Contains(root, new AssemblyInfoEqualityComparer()))
                    dependenciesMap[currentAssembly].Add(root);

                BuildDependenciesMap(currentAssembly, dependenciesMap);
            }
        }

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
