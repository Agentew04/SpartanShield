using MaterialDesignThemes.Wpf;
using Microsoft.Toolkit.Mvvm.Input;
using SpartanShield.Managers;
using SpartanShield.Models;
using SpartanShield.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SpartanShield.ViewModels
{
    public class MenuViewModel : BaseViewModel
    {
        public MenuViewModel(MenuView menuView)
        {
            MenuView = menuView;
        }

        private MenuView MenuView { get; set; }


        private string _statusMessage = "LOCKED";
        public string StatusMessage
        {
            get { return _statusMessage; }
            set
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }

        private bool _isWorking = false;
        public bool IsWorking
        {
            get { return _isWorking; }
            set
            {
                _isWorking = value;
                OnPropertyChanged();
            }
        }

        private EncryptionStatus _encryptionStatus = EncryptionStatus.Encrypted;
        public EncryptionStatus EncryptionStatus
        {
            get { return _encryptionStatus; }
            set
            {
                _encryptionStatus = value;
                OnPropertyChanged();
            }
        }

        public ICommand ToggleLockCommand
        {
            get { return new RelayCommand(ToggleLock); }
        }

        public ICommand RequestFoldersViewCommand
        {
            get { return new RelayCommand(RequestFoldersView); }
        }



        private async void ToggleLock()
        {
            IsWorking = true;

            if(EncryptionStatus == EncryptionStatus.Encrypted)
            {
                // decrypt
                await Task.Run(
                    () => FileManager.Instance.Decrypt());
                StatusMessage = "UNLOCKED";
                EncryptionStatus = EncryptionStatus.Decrypted;
            }
            else
            {
                // encrypt
                await Task.Run(
                    () => FileManager.Instance.Encrypt());
                StatusMessage = "LOCKED";
                EncryptionStatus= EncryptionStatus.Encrypted;
            }

            IsWorking = false;
        }

        private void RequestFoldersView()
        {
            MenuView.MainWindow.Navigate(typeof(LoginView));
        }
    }
}
