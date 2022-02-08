using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SpartanShield.Pages
{
    /// <summary>
    /// Interação lógica para MenuPage.xam
    /// </summary>
    public partial class MenuPage : Page
    {
        private readonly MainWindow main;

        public MenuPage(MainWindow main)
        {
            InitializeComponent();
            this.main = main;
        }

        private void RequestFoldersPage(object sender, RoutedEventArgs e) => main.Navigate(typeof(FoldersPage));

        private void ToggleLockClick(object sender, RoutedEventArgs e)
        {
            var key = Utils.CreateKeyFromString("123", "Agentew04");
            var iv = Utils.CreateIV();
            CryptoItem cryptoItem = new()
            {
                Id = Guid.NewGuid(),
                IsDirectory = true,
                IsEncrypted = false,
                IsInRemovableDrive = false,
                Name = "Teste",
                OwnerId = Guid.Empty,
                Path = @"E:\teste"
            };
            EncryptionManager.EncryptItem(new(key,iv), cryptoItem);
        }
    }
}
