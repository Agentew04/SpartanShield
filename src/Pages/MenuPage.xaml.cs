using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

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

        private void FindDirectoryOrFile(object sender, RoutedEventArgs e)
        {
            using var opdiag = new FolderBrowserDialog();
            var result = opdiag.ShowDialog();
            if (result == DialogResult.OK)
            {
                DirectoryTextBox.Text = opdiag.SelectedPath;
            }
        }
    }
}
