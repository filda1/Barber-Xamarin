using Com.OneSignal;
using GentlemansV2;
using GentlemansV2.Normal;
using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Json;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GentlemansV2
{
	public partial class Entrar : ContentPage {

		public static Entrar instance;

		public Entrar(string email) {
			InitializeComponent();

			emailEntry.Text = email;

			instance = this;
		}

        private async Task TryToLogin(object sender, EventArgs e)
		{

			Navigation.PushModalAsync(new Loading());

			var keyValues = new List<KeyValuePair<string, string>> {
				new KeyValuePair<string, string>("email", emailEntry.Text),
				new KeyValuePair<string, string>("password", passwordEntry.Text)
			};

			var request = new HttpRequestMessage(HttpMethod.Post, Configs.server + "/api/user/login");

			request.Content = new FormUrlEncodedContent(keyValues);

			var client = new HttpClient();
			var response = await client.SendAsync(request);
			string resultLog = await response.Content.ReadAsStringAsync();

			if (resultLog == "incorrect") {
				await DisplayAlert("Credencias incorretas", null, "Entendi");
			} else {

                var user = JsonObject.Parse(resultLog);

                Application.Current.Properties["login_token"] = (string) user["login_token"];

                User.id = user["id"];
                User.name = (string)user["first_name"] + " " + (string)user["last_name"];
                User.email = user["email"];
                User.pic = user["image"];
				/*
                OneSignal.Current.IdsAvailable(async (playerID, pushToken) => {
                    request = new HttpRequestMessage(HttpMethod.Get, Configs.server + "/api/user/update/onesignal-id?&user_id=" + User.id + "&onesignal_id=" + playerID);

                    client = new HttpClient();
                    response = await client.SendAsync(request);
                });
                */
				Application.Current.MainPage = new NavigationPage(new ChoosePage());
				
			}

			Navigation.PopModalAsync();
			
		}

        private void Button_Clicked(object sender, EventArgs e)
        {
			TryToLogin(sender, e);
        }
    }
}
