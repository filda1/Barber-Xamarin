using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Net.Http;
using System.Diagnostics;
using Rg.Plugins.Popup.Extensions;
using GentlemansV2;

namespace GentlemansV2
{
    public partial class ChangePassword : Rg.Plugins.Popup.Pages.PopupPage
    {
        public ChangePassword()
        {
            InitializeComponent();
        }


        void Change_Password(object sender, System.EventArgs e)
        {
            SendInfo();
        }

        async Task SendInfo()
        {

            if (newPass.Text != newConfiPass.Text)
            {
                DisplayAlert("Passwords não coicidem", null, "Entendi");
                return;
            }

            var keyValues = new List<KeyValuePair<string, string>> {
                new KeyValuePair<string, string>("oldPassword", oldPass.Text),
                new KeyValuePair<string, string>("newPassword", newPass.Text),
                new KeyValuePair<string, string>("email", User.email)
           };

            var request = new HttpRequestMessage(HttpMethod.Post, Configs.server + "/api/user/change-password");

            request.Content = new FormUrlEncodedContent(keyValues);

            var client = new HttpClient();
            var response = await client.SendAsync(request);
            string resultLog = await response.Content.ReadAsStringAsync();

            Debug.Write(resultLog);

            if (resultLog == "success")
            {
                await DisplayAlert("Sucesso", "Password alterada com sucesso", "Entendi");
                await Navigation.PopPopupAsync();
            }
            else if (resultLog == "passwordError")
            {
                DisplayAlert("Password antiga incorreta", null, "Entendi");
            }
            else
            {
                DisplayAlert("Ocorreu um erro inesperado tente novamente mais tarde", null, "Entendi");
            }
        }
    }
}
