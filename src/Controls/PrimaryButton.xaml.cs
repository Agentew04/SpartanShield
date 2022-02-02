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

namespace SpartanShield.Controls
{
    /// <summary>
    /// Interação lógica para PrimaryButton.xam
    /// </summary>
    public partial class PrimaryButton : System.Windows.Controls.UserControl
    {
#pragma warning disable CS8618 // O campo não anulável precisa conter um valor não nulo ao sair do construtor. Considere declará-lo como anulável.
        public PrimaryButton()
#pragma warning restore CS8618 // O campo não anulável precisa conter um valor não nulo ao sair do construtor. Considere declará-lo como anulável.
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

        public event RoutedEventHandler Click;

        void OnButtonClick(object sender, RoutedEventArgs e)
        {
            Click?.Invoke(this, e);
        }


    }
}
