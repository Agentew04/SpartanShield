﻿using SpartanShield.ViewModels;
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

namespace SpartanShield.Views
{
    /// <summary>
    /// Interação lógica para MenuView.xam
    /// </summary>
    public partial class MenuView : Page
    {
        public MenuView(MainWindow mainWindow)
        {
            InitializeComponent();
            MainWindow = mainWindow;
            ViewModel = new(this);
            DataContext = ViewModel;
        }

        private MainWindow MainWindow;
        public MenuViewModel ViewModel { get; set; }
    }
}