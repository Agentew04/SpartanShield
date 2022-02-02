using System;
using System.Windows;
using System.Windows.Controls;

namespace SpartanShield.Pages
{
    /// <summary>
    /// Interação lógica para LoginPage.xam
    /// </summary>
    public partial class LoginPage : Page
    {
        private readonly MainWindow main;

        public LoginPage(MainWindow main)
        {
            InitializeComponent();
            this.main = main;
        }
        
        private void GetPortableClicked(object sender, RoutedEventArgs e)
        {
            //Process.Start(new ProcessStartInfo(PORTABLEURL));
            MessageBox.Show($"Unable to open browser. \nLink: {main.PORTABLEURL}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void RequestRegister(object sender, RoutedEventArgs e)
        {
            main.Navigate(typeof(RegisterPage));
            GC.Collect();
        }

        private void LoginClicked(object sender, RoutedEventArgs e)
        {
            var username = UsernameTextBox.Text;
            var password = PasswordBox.Password;

            var isSuccessful = UserControl.Login(username, password);

            if (!isSuccessful)
            {
                //show error
                return;
            }

            main.Navigate(typeof(MenuPage));
        }

    }
}
