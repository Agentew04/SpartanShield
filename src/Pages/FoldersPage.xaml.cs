using System.Windows.Controls;

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
