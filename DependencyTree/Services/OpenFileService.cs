﻿using System.ComponentModel.Composition;
using Microsoft.Win32;

namespace DependencyTree.Services
{
    [Export(typeof(IOpenFileService))]
    [PartCreationPolicy(CreationPolicy.Shared)]
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
