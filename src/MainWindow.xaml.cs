using SpartanShield.Pages;
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

namespace SpartanShield
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string PORTABLEURL {get;}= "http://github.com/Agentew04";

        private Dictionary<Type, Page> pageMap; //needed to not overflow memory

        public MainWindow()
        {
            InitializeComponent();

            pageMap = new()
            {
                { typeof(LoginPage), new LoginPage(this) },
                { typeof(RegisterPage), new RegisterPage(this) },
                { typeof(MenuPage), new MenuPage()}
            };


            contentFrame.Navigate(new LoginPage(this));
        }
        public void Navigate(Type pageType)
        {
            if (pageMap.ContainsKey(pageType))
            {
                contentFrame.Navigate(pageMap[pageType]);
            }
        }
    }
}
