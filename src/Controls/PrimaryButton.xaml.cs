using System.Windows;

namespace SpartanShield.Controls
{
    /// <summary>
    /// Interação lógica para PrimaryButton.xam
    /// </summary>
    public partial class PrimaryButton : System.Windows.Controls.UserControl
    {
        public PrimaryButton()
        {
            InitializeComponent();
        }



        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(PrimaryButton), new PropertyMetadata(""));

        public event RoutedEventHandler? Click;

        void OnButtonClick(object sender, RoutedEventArgs e)
        {
            Click?.Invoke(this, e);
        }


    }
}
