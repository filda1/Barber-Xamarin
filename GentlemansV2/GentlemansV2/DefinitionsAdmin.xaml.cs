using System;
using System.Collections.Generic;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;
using FFImageLoading.Helpers;
using Rg.Plugins.Popup.Extensions;
using GentlemansV2;

namespace GentlemansV2
{
    public partial class DefinitionsAdmin : PopupPage
    {
        public DefinitionsAdmin()
        {
            InitializeComponent();

            ProfileName.Text = User.name;
        }

        void ChangePassword_Clicked(object sender, System.EventArgs e)
        {

        }

        void Logout_Clicked(object sender, System.EventArgs e)
        {
            Application.Current.Properties["login_token"] = null;
            App.Current.MainPage = new NavigationPage(Configs._mainPage);
            Navigation.PopPopupAsync();
        }
    }
}
