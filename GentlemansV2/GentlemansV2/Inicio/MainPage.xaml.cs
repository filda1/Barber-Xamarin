using System;
using System.Collections.Generic;
using Rg.Plugins.Popup.Extensions;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Com.OneSignal;
using System.Json;
using GentlemansV2.Normal;

namespace GentlemansV2
{
    public partial class MainPage : ContentPage
    {
        public static MainPage instance;
        public MainPage()
        {
            InitializeComponent();

            instance = this;
            NavigationPage.SetHasNavigationBar(this, false);

            checkIfUserHasLogin();
        }

        void checkIfUserHasLogin()
        {
            if (Application.Current.Properties.ContainsKey("login_token"))
            {
                loginUser((string)Application.Current.Properties["login_token"]);
            }
        }

        async Task loginUser(string token)
        {
            Navigation.PushModalAsync(new Loading());

            var keyValues = new List<KeyValuePair<string, string>> {
                new KeyValuePair<string, string>("token", token)
            };

            var request = new HttpRequestMessage(HttpMethod.Post, Configs.server + "/api/user/login/with-token");

            request.Content = new FormUrlEncodedContent(keyValues);

            var client = new HttpClient();
            var response = await client.SendAsync(request);
            string resultLog = await response.Content.ReadAsStringAsync();

            await Navigation.PopModalAsync();

            if (resultLog != "incorrect")
            {
                var user = JsonObject.Parse(resultLog);

                Application.Current.Properties["login_token"] = (string)user["login_token"];

                if ((int)user["isBarber"] == 0)
                {

                    User.id = user["id"];
                    User.name = (string)user["first_name"] + " " + (string)user["last_name"];
                    User.email = user["email"];
                    User.pic = user["image"];

                    OneSignal.Current.IdsAvailable(async (playerID, pushToken) => {
                        request = new HttpRequestMessage(HttpMethod.Get, Configs.server + "/api/user/update/onesignal-id?&user_id=" + User.id + "&onesignal_id=" + playerID);

                        client = new HttpClient();
                        response = await client.SendAsync(request);
                    });

                    App.Current.MainPage = new NavigationPage(new ChoosePage());
                }
                else if ((int)user["isBarber"] == 1)
                {
                    User.id = user["id"];
                    User.name = (string)user["slot"];
                    User.pic = "users/default.png";

                    App.Current.MainPage = new NavigationPage(new TodayList());
                }

            }

        }

        private void Open_RegisterOptions(object sender, EventArgs e)
        {
            Navigation.PushAsync(new Register(null, null, null, null));
        }

        private void Open_Entrar(object sender, EventArgs e)
        {
            Navigation.PushAsync(new Entrar(null));
        }

        private void FacebookTokenReturn(string token)
        {
            Navigation.PushModalAsync(new Loading(), false);
            Navigation.PopAsync();

            FacebookGetProfile(token);
        }

        public async Task FacebookGetProfile(string token)
        {

            var request = new HttpRequestMessage(HttpMethod.Get, "https://graph.facebook.com/v3.0/me?fields=id%2Cname%2Cemail%2Cbirthday%2Cgender%2Cfirst_name%2Clast_name&access_token=" + token);

            var client = new HttpClient();
            var response = await client.SendAsync(request);
            string resultLog = await response.Content.ReadAsStringAsync();

            var json = JsonObject.Parse(resultLog);

            Navigation.PopModalAsync();
            await Navigation.PushAsync(new Register(json["first_name"], json["last_name"], json["email"], "http://graph.facebook.com/" + json["id"] + "/picture?type=large"));
        }


        void Convidado_Clicked(object sender, System.EventArgs e)
        {
            App.Current.MainPage = new NavigationPage(new ChoosePage());
        }
    }
}
