using SpartanShield.Managers;
using SpartanShield.Pages;
using SpartanShield.Views;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Usb.Events;

namespace SpartanShield
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string PORTABLEURL { get; } = "http://github.com/Agentew04";

        private readonly Dictionary<Type, Page> pageMap; //needed to not overflow memory with infinite pages

        public MainWindow()
        {
            InitializeComponent();
            pageMap = new()
            {
                { typeof(LoginView), new LoginView(this) },
                { typeof(MenuView), new MenuView(this)},
            };
            Navigate(typeof(LoginView));


            // initialize services
            //USBManager.StartManager();
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
