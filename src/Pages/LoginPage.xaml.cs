using SpartanShield.Managers;
using System;
using System.Threading.Tasks;
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

        private async void LoginClicked(object sender, RoutedEventArgs e)
        {
            ProgressBar.Visibility = Visibility.Visible;
            var username = UsernameTextBox.Text;
            var password = PasswordBox.Password;

            var isSuccessful = await Task.Run(() =>
                UserManager.Login(username, password));

            switch (isSuccessful)
            {
                case UserManager.AuthResult.Success:
                    main.Navigate(typeof(MenuPage));
                    break;
                case UserManager.AuthResult.UserNotExist:
                    ErrorText.Text = "This user does not exist!";
                    ErrorText.Visibility = Visibility.Visible;
                    break;
                case UserManager.AuthResult.WrongPassword:
                    ErrorText.Text = "Wrong password!";
                    ErrorText.Visibility = Visibility.Visible;
                    break;
                case UserManager.AuthResult.MissingInfo:
                    ErrorText.Text = "Fill all fields!";
                    ErrorText.Visibility = Visibility.Visible;
                    break;
                case UserManager.AuthResult.UnknownError:
                    ErrorText.Text = "An unknown error has ocurred!";
                    ErrorText.Visibility = Visibility.Visible;
                    break;
                default:
                    break;
            }
            ProgressBar.Visibility = Visibility.Collapsed;
        }

    }
}
