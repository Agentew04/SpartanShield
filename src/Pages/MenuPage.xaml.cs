using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
            if(result == DialogResult.OK)
            {
                DirectoryTextBox.Text = opdiag.SelectedPath;
            }
        }
    }
}
