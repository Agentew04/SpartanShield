using SpartanShield.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpartanShield.ViewModels
{
    public class MenuViewModel : BaseViewModel
    {
        public MenuViewModel(MenuView menuView)
        {
            MenuView = menuView;
        }

        private MenuView MenuView { get; set; }
    }
}
