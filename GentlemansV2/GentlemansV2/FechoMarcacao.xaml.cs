using System;
using System.Collections.Generic;
using SystemJsonAlias = System.Json;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;
using GentlemansV2;

namespace GentlemansV2
{
    public partial class FechoMarcacao : ContentPage
    {
        string id { get; set; }

        public FechoMarcacao(string id)
        {
            InitializeComponent();

            this.id = id;
            Get();
        }

        async Task Get()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, Configs.server + "/api/marcacao/get/" + id);

            var client = new HttpClient();
            var response = await client.SendAsync(request);
            string resultLog = await response.Content.ReadAsStringAsync();

            var marcacaoData = SystemJsonAlias.JsonObject.Parse(resultLog);

            cliente_Name.Text = marcacaoData["user_name"];
            hora_inicio.Text = marcacaoData["hora_inicio"];
            hora_fim.Text = marcacaoData["hora_fim"];
            hora_inicio.Text = marcacaoData["hora_inicio"];
            trabalhos.Text = marcacaoData["trabalhosName"];
            valor.Text = ((int)marcacaoData["valor_final"]).ToString();

            if (marcacaoData["dia_fechado"] != null)
            {
                data_finalizacao.Text = marcacaoData["dia_fechado"];
                data_finalizacao.IsVisible = true;
                data_finalizacaoText.IsVisible = true;
            }
        }



        async Task FinalizarMarcacao()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, Configs.server + "/api/marcacao/finaliar/" + id + "/" + valor.Text);

            var client = new HttpClient();
            var response = await client.SendAsync(request);
            string resultLog = await response.Content.ReadAsStringAsync();

            if (resultLog == "success")
            {
                Navigation.PopAsync();
                await DisplayAlert("Marcação Finalizada", null, "Entendi");
            }
            else
            {
                await DisplayAlert("Ocorreu um erro, por favor tente novamente mais tarde", null, "Entendi");
            }
        }

        private void Finalizar_Clicked(object sender, EventArgs e)
        {
            FinalizarMarcacao();
        }
    }
    }
