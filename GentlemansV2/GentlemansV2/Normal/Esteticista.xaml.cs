using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Rg.Plugins.Popup.Extensions;
using System.Net.Http;
using System.Json;
using GentlemansV2;

namespace GentlemansV2.Normal
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Esteticista : ContentPage
    {
        public Esteticista()
        {
            InitializeComponent();

            if (User.name == null)
            {
                detailsBox.IsVisible = false;
            }
            else
            {
                GetLastAppoint();
            }
        }


        protected override void OnAppearing()
        {
            base.OnAppearing();

            pageOpen = false;
        }

        bool pageOpen = false;

    void Definitions_Clicked(object sender, System.EventArgs e)
        {
            Navigation.PushPopupAsync(new Definitions());
        }
        async Task GetLastAppoint()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, Configs.server + "/api/user/last-apointment/estetica/" + User.id);

            var client = new HttpClient();
            var response = await client.SendAsync(request);
            string resultLog = await response.Content.ReadAsStringAsync();

            if (resultLog == "none")
            {
                detailsBox.IsVisible = false;
            }
            else
            {
                var json = JsonObject.Parse(resultLog);

                BarberName.Text = json["cadeira_id"];
                details.Text = json["dia"] + " " + json["hora_inicio"];
                detailsBox.IsVisible = true;
            }
        }

        private async Task Maracacoes_Clicked(object sender, EventArgs e)
        {
            if (pageOpen)
                return;

            if (User.name == null)
            {
                var result = await DisplayActionSheet("Apenas utilizadores registados.", "Voltar", "Registar");

                if (result != "Voltar")
                {
                    App.Current.MainPage = new NavigationPage(Configs._mainPage);
                }

                return;
            }


            await Navigation.PushModalAsync(new Loading());
            StackLayout stack = (StackLayout)sender;
            Frame innerChild = (Frame)stack.Children[0];

            await innerChild.ScaleTo(0.8, 250);
            await innerChild.ScaleTo(1, 250);


            await Navigation.PushAsync(new MarcacoesEsteticista());
        }

        private async Task Galeria_Clicked(object sender, EventArgs e)
        {
            if (pageOpen)
                return;

            StackLayout stack = (StackLayout)sender;
            Frame innerChild = (Frame)stack.Children[0];

            await innerChild.ScaleTo(0.8, 150);
            await innerChild.ScaleTo(1, 150);

            await Navigation.PushAsync(new AppGaleria());
        }

        private async Task Localizacao_Clicked(object sender, EventArgs e)
        {
            if (pageOpen)
                return;

            StackLayout stack = (StackLayout)sender;
            Frame innerChild = (Frame)stack.Children[0];

            await innerChild.ScaleTo(0.8, 150);
            await innerChild.ScaleTo(1, 150);

            await Navigation.PushAsync(new LocalizacaoEsteticista());
        }

        private async Task Contactos_Clicked(object sender, EventArgs e)
        {
            if (pageOpen)
                return;

            StackLayout stack = (StackLayout)sender;
            Frame innerChild = (Frame)stack.Children[0];

            await innerChild.ScaleTo(0.8, 150);
            await innerChild.ScaleTo(1, 150);

            await Navigation.PushAsync(new ContactosEsteticista());
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            await Maracacoes_Clicked(sender, e);
        }

        private async void TapGestureRecognizer_Tapped_1(object sender, EventArgs e)
        {
            await Contactos_Clicked(sender, e);
        }

        private async void TapGestureRecognizer_Tapped_2(object sender, EventArgs e)
        {
            await Localizacao_Clicked(sender, e);
        }
    }
}