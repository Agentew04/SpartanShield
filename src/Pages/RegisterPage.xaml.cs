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
    /// Interação lógica para RegisterPage.xam
    /// </summary>
    public partial class RegisterPage : Page
    {
        private readonly MainWindow main;

        public RegisterPage(MainWindow main)
        {
            InitializeComponent();
            this.main = main;
        }

        private void GetPortableClicked(object sender, RoutedEventArgs e)
        {
            //Process.Start(new ProcessStartInfo(PORTABLEURL));
            MessageBox.Show($"Unable to open browser. \nLink: {main.PORTABLEURL}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void RequestLoginPage(object sender, RoutedEventArgs e)
        {
            main.Navigate(typeof(LoginPage));
            GC.Collect();
        }

        private void RegisterButtonClicked(object sender, RoutedEventArgs e)
        {
            if (RepeatPasswordBox.Password != PasswordBox.Password)
            {
                //show error
                return;
            }
            var isSuccesful = UserControl.Register(UsernameTextBox.Text, PasswordBox.Password);
            if (!isSuccesful)
            {
                //show error
                return;
            }
        }


    }
}
