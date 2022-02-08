using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SpartanShield.Pages
{
    /// <summary>
    /// Interação lógica para FoldersPage.xam
    /// </summary>
    public partial class FoldersPage : Page
    {
        private readonly MainWindow main;
        public FoldersPage(MainWindow main)
        {
            InitializeComponent();
            this.main = main;
        }
    }
}
