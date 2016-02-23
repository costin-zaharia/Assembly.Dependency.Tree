using System.ComponentModel.Composition;
using Microsoft.Win32;

namespace DependencyTree.Services.Notifications
{
    [Export(typeof(IOpenFileService))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class OpenFileService : IOpenFileService
    {
        public string GetSelectedFile(string tile)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Assembly (*.dll)|*.dll",
                Title = tile
            };

            return openFileDialog.ShowDialog() == true
                ? openFileDialog.FileName
                : string.Empty;
        }
    }
}
