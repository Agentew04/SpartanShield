using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

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

        private async void RegisterButtonClicked(object sender, RoutedEventArgs e)
        {
            ProgressBar.Visibility = Visibility.Visible;
            var username = UsernameTextBox.Text;
            var password = PasswordBox.Password;
            var passwordAgain = RepeatPasswordBox.Password;

            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(passwordAgain)) // does not check if a field is
                                                          // empty or null
            {
                ErrorText.Text = "Fill all fields!";
                ErrorText.Visibility = Visibility.Visible;
                ProgressBar.Visibility = Visibility.Collapsed;
                return;
            }

            var isSuccesful = await Task.Run(() => 
                UserControl.Register(username, password, passwordAgain));

            switch (isSuccesful)
            {
                case UserControl.AuthResult.Success:
                    main.Navigate(typeof(MenuPage));
                    break;
                case UserControl.AuthResult.UserAlreadyExist:
                    ErrorText.Text = "This user already exists!";
                    ErrorText.Visibility = Visibility.Visible;
                    break;
                case UserControl.AuthResult.PasswordNotMatch:
                    ErrorText.Text = "The two passwords doesn't match!";
                    ErrorText.Visibility = Visibility.Visible;
                    break;
                case UserControl.AuthResult.MissingInfo:
                    ErrorText.Text = "Fill all fields!";
                    ErrorText.Visibility = Visibility.Visible;
                    break;
                case UserControl.AuthResult.UnknownError:
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
