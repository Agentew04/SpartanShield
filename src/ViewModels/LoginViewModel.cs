﻿using MaterialDesignThemes.Wpf;
using Microsoft.Toolkit.Mvvm.Input;
using SpartanShield.Managers;
using SpartanShield.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SpartanShield.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        public LoginViewModel(LoginView loginView)
        {
            LoginView = loginView;
        }

        private LoginView LoginView { get; set; }
        private string _username = string.Empty;
        public string Username
        {
            get { return _username; }
            set
            {
                _username = value; 
                OnPropertyChanged();
            }
        }

        private bool _isLoading = false;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        private SnackbarMessageQueue _snackbarMessageQueue = new();
        public SnackbarMessageQueue SnackbarMessageQueue
        {
            get { return _snackbarMessageQueue; }
            set
            {
                _snackbarMessageQueue = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoginCommand
        {
            get
            {
                return new RelayCommand<object>(Login);
            }
        }

        private async void Login(object? passwordbox)
        {
            bool isError = false;
            IsLoading = true;
            var password = (passwordbox as PasswordBox)?.Password;

            if (password == null) return;
            var result = await Task.Run(() =>
               UserManager.Login(Username, password));

            // handle errors
            switch (result)
            {
                case UserManager.AuthResult.UserNotExist:
                    SnackbarMessageQueue.Enqueue("User does not exist!");
                    isError = true;
                    break;
                case UserManager.AuthResult.MissingInfo:
                    SnackbarMessageQueue.Enqueue("Fill all fields!");
                    isError = true;
                    break;
                case UserManager.AuthResult.WrongPassword:
                    SnackbarMessageQueue.Enqueue("Wrong password!");
                    isError = true;
                    break;
                case UserManager.AuthResult.UnknownError:
                    SnackbarMessageQueue.Enqueue("Unknown error!");
                    isError = true;
                    break;
            }

            if (!isError)
            {
                // initialize file manager
                FileManager.Instance.Initialize(Username, password);

                LoginView.MainWindow.Navigate(typeof(MenuView));
            }
            

            IsLoading = false;
        }


    }
}
