using System.Windows.Controls;
using System;
using System.IO;
using System.Windows;
using SpartanShield.Managers;

namespace SpartanShield.Pages
{
    /// <summary>
    /// Interação lógica para FoldersPage.xam
    /// </summary>
    public partial class FoldersPage : Page
    {

#pragma warning disable IDE0052 // Remover membros particulares não lidos
        private readonly MainWindow main;
#pragma warning restore IDE0052 // Remover membros particulares não lidos
        public FoldersPage(MainWindow main)
        {
            InitializeComponent();
            this.main = main;
        }

        private void AddFolderRequest(object sender, RoutedEventArgs e)
        {
            var path = FolderPathTextbox.Text;
            var isFromUsb = USBManager.IsPathFromUsb(path);
            CryptoItem item;
            if (isFromUsb)
            {
                item = new()
                {
                    Path = path,
                    
                }
            }
            
        }

        private void RemoveFolderRequest(object sender, RoutedEventArgs e)
        {
            // get selected item
            // get id from item
            // decrypt if needed and restore folder
        }

        private void RefreshFolderRequest(object sender, RoutedEventArgs e)
        {
            // fetch idmappings from db
            // parse to tree
        }
    }
}
