using System;
using System.Collections.Generic;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;
using FFImageLoading.Helpers;
using Rg.Plugins.Popup.Extensions;
using System.Net.Mail;
using System.Net.Http;
using GentlemansV2;

namespace GentlemansV2
{
    public partial class Definitions : PopupPage
    {
        private static readonly HttpClient client = new HttpClient();

        public Definitions()
        {
            InitializeComponent();

            if(User.name == null)
            {
                ChangePasswordStack.IsVisible = false;
                ProfileName.Text = "Convidado";
                profileImage.Source = "user.png";
            }
            else
            {
                ProfileName.Text = User.name;
                profileImage.Source = Configs.server + "/storage/" + User.pic;
            }
        }

        void ChangePassword_Clicked(object sender, System.EventArgs e)
        {
            Navigation.PushPopupAsync(new ChangePassword());
        }

        async void NewPassword_Clicked()
        {
            /*
            await DisplayAlert("Alert", User.email, "OK");
            */
            var keyValues = new List<KeyValuePair<string, string>> {
                new KeyValuePair<string, string>("email", User.email)
            };

            var request = new HttpRequestMessage(HttpMethod.Post, Configs.server + "/api/user/new-password");

            request.Content = new FormUrlEncodedContent(keyValues);

            var client = new HttpClient();
            var response = await client.SendAsync(request);
            string resultLog = await response.Content.ReadAsStringAsync();

            await DisplayAlert("Alerta", "Recebeu um email com um link para mudar a sua Palavra passe", "OK");

            User.id = 0;
            User.email = "";
            User.name = "";
            User.pic = "";
            Application.Current.Properties["login_token"] = null;
            App.Current.MainPage = new NavigationPage(Configs._mainPage);
            Navigation.PopPopupAsync();

        }

        void Logout_Clicked(object sender, System.EventArgs e)
        {
            User.id = 0;
            User.email = "";
            User.name = "";
            User.pic = "";
            Application.Current.Properties["login_token"] = null;
            App.Current.MainPage = new NavigationPage(Configs._mainPage);
            Navigation.PopPopupAsync();
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            NewPassword_Clicked();
        }
    }
}
