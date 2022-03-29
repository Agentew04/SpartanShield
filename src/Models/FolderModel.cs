using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpartanShield.Models
{
    internal class FolderModel : INotifyPropertyChanged
    {

        private string _path = string.Empty;
        public string Path
        {
            get { return _path; }
            set
            {
                if(_path != value) 
                    OnPropertyChanged(nameof(Path));
                _path = value;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
