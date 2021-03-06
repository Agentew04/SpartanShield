using System.Windows;
using System.Windows.Controls;

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
        }
    }
}
