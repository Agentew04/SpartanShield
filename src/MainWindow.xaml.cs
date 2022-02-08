using SpartanShield.Pages;
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
        public IUsbEventWatcher UsbEventWatcher { get; set; } = new UsbEventWatcher();

        public MainWindow()
        {
            InitializeComponent();

            pageMap = new()
            {
                { typeof(LoginPage), new LoginPage(this) },
                { typeof(RegisterPage), new RegisterPage(this) },
                { typeof(MenuPage), new MenuPage(this) },
                { typeof(FoldersPage), new FoldersPage(this) }
            };

            Navigate(typeof(LoginPage));

            UsbEventWatcher.UsbDeviceAdded += USBManager.Plugged;
            UsbEventWatcher.UsbDeviceRemoved += USBManager.Unplugged;
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
