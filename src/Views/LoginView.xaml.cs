using SpartanShield.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace SpartanShield.Views
{
    /// <summary>
    /// Interação lógica para LoginView.xam
    /// </summary>
    public partial class LoginView : Page
    {
        public LoginView(MainWindow mainWindow)
        {
            InitializeComponent();
            ViewModel = new LoginViewModel(this);
            DataContext = ViewModel;
            MainWindow = mainWindow;
        }

        public LoginViewModel ViewModel { get; set; }
        public MainWindow MainWindow { get; set; }

    }
}
