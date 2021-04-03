using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Rg.Plugins.Popup.Extensions;
using System.Diagnostics;
using System.Json;
using Plugin.GoogleClient;
using Plugin.GoogleClient.Shared;
using GentlemansV2;

namespace GentlemansV2
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class RegisterOptions : Rg.Plugins.Popup.Pages.PopupPage {
		public RegisterOptions ()
		{
			InitializeComponent ();


        }

		public void Open_RegisterPage() {
			Navigation.PopPopupAsync();
			Navigation.PushAsync(new Register(null, null, null, null));
		}

		private void FacebookLogin_Click(object sender, EventArgs e) {
			Navigation.PopPopupAsync();
        }

        void Google_Click(object sender, System.EventArgs e)
        {
            Debug.Write("foda se 1");

            LoginAsync();
        }


        public async Task LoginAsync()
        {

            try
            {
                CrossGoogleClient.Current.OnLogin += OnLoginCompleted;
                CrossGoogleClient.Current.OnError += Current_OnError;

                await CrossGoogleClient.Current.LoginAsync();
            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message);
                //App.Current.MainPage.DisplayAlert("Erro", "Ocorreu um erro por favor tente novamente mais tarde.", "Entendi");
            }

        }

        void Current_OnError(object sender, GoogleClientErrorEventArgs e)
        {
            App.Current.MainPage.DisplayAlert("Erro", "Ocorreu um erro por favor tente novamente mais tarde.", "Entendi");
        }


        private void OnLoginCompleted(object sender, GoogleClientResultEventArgs<GoogleUser> loginEventArgs)
        {
            Debug.Write("foda se4");
            if (loginEventArgs.Data != null)
            {
                GoogleUser googleUser = loginEventArgs.Data;
                Navigation.PopPopupAsync();
                Navigation.PushAsync(new Register(googleUser.GivenName, googleUser.FamilyName, googleUser.Email, googleUser.Picture.AbsoluteUri));
            }
            else
            {
                App.Current.MainPage.DisplayAlert("Erro", loginEventArgs.Message, "Entendi");
            }

        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            Open_RegisterPage();
        }
    }
}