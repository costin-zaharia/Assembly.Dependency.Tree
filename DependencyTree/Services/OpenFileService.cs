using Microsoft.Win32;

namespace DependencyTree.Services
{
    public class OpenFileService : IOpenFileService
    {
        private readonly OpenFileDialog _openFileDialog = new OpenFileDialog
        {
            Filter = "Assembly (*.dll)|*.dll"
        };

        public string GetSelectedFile()
        {
            return _openFileDialog.ShowDialog() == true
                ? _openFileDialog.FileName
                : string.Empty;
        }
    }
}
