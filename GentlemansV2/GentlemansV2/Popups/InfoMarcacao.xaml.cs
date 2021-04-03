using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Net.Http;
using System.Diagnostics;
using System.Json;
using Rg.Plugins.Popup.Extensions;
using GentlemansV2;

namespace GentlemansV2
{
    public partial class InfoMarcacao : Rg.Plugins.Popup.Pages.PopupPage
    {
        string id;
        public InfoMarcacao(string id)
        {
            InitializeComponent();
            this.id = id;
            GetInfoAppoint();
        }

        async Task GetInfoAppoint()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, Configs.server + "/api/user/info-apointment/" + id);

            var client = new HttpClient();
            var response = await client.SendAsync(request);
            string resultLog = await response.Content.ReadAsStringAsync();

            var json = JsonObject.Parse(resultLog);

            barber.Text = json["cadeira_id"];
            dateHour.Text = json["dia"] + " " + json["hora_inicio"] + " - " + json["hora_fim"];
            jobs.Text = json["trabalho"];

            if (json["dia_fechado"] != null)
            {
                desmarcar_button.IsVisible = false;
            }
        }

        void Desmarcar_Clicked(object sender, System.EventArgs e)
        {


            SendDelete();
        }

        async Task SendDelete()
        {
            var result = await DisplayActionSheet("Tem a certeza que pretende proceder com a desmarcação?", "Não", "Sim");

            if (result == "Não")
            {
                return;
            }

            var request = new HttpRequestMessage(HttpMethod.Get, Configs.server + "/api/user/remove-apointment/" + id + "/" + User.id);

            var client = new HttpClient();
            var response = await client.SendAsync(request);
            string resultLog = await response.Content.ReadAsStringAsync();

            if (resultLog == "ok")
            {
                Marcacoes.refreshCalendar();
                await DisplayAlert("Sucesso", "Desmarcação efetuada com sucesso.", "Entendi");
                await Navigation.PopPopupAsync();
            }
            else if (resultLog == "none")
            {
                await DisplayAlert("Erro", "Ocorreu um erro, por favor tente novamente mais tarde.", "Entendi");
            }
            else
            {
                await DisplayAlert("Erro", "Ocorreu um erro, por favor tente novamente mais tarde.", "Entendi");
            }
        }
    }
}
