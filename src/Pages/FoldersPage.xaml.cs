using System.Windows.Controls;

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
    }
}
