using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Json;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;
using Rg.Plugins.Popup.Extensions;
using GentlemansV2;

namespace GentlemansV2
{
    public class Job {
        public int idJob { get; set; }
        public string displayName { get; set; }
        public int duracao { get; set; }
    }
    public class Cadeira
    {
        public int idCadeira { get; set; }
        public string displayName { get; set; }
        public Color backgroundColor { get; set; }
        public Color textColor { get; set; }

    }
    public class Hora
    {
        public string hora { get; set; }
    }
    public partial class Marcar : PopupPage
    {
    
        public ObservableCollection<Job> jobs = new ObservableCollection<Job>();
        public ObservableCollection<Cadeira> cadeiras = new ObservableCollection<Cadeira>();
        public ObservableCollection<Hora> horas = new ObservableCollection<Hora>();

        public List<string> jobsSelected = new List<string>();
        public List<string> cadeirasInde = new List<string>();
        public string caderiaSelecionada = null;
        public string horaSelecionada = null;


        public int stage = 0;
        public Marcar()
        {
            InitializeComponent();

            LoadJobs();
        }

        async Task LoadJobs()
        {

            var request = new HttpRequestMessage(HttpMethod.Get, Configs.server + "/api/jobs");

            var client = new HttpClient();
            var response = await client.SendAsync(request);
            string resultLog = await response.Content.ReadAsStringAsync();

            var jobsJson = JsonObject.Parse(resultLog);
           
            for (int i = 0; i < jobsJson.Count; i++)
            {
                jobs.Add(new Job() { idJob = (int) jobsJson[i]["id"], displayName = jobsJson[i]["nome"], duracao = (int) jobsJson[i]["duracao"] });
            }

            jobsStack.FlowItemsSource = jobs;

            loading.IsVisible = false;

            jobsStackList.IsVisible = true;
        }

        async Task LoadCadeiras()
        {
            loading.IsVisible = true;
            cadeiras.Clear();
            cadeirasInde.Clear();
            DateTime date = Marcacoes.dateSelected;

            string json = "";
            foreach (var item in jobsSelected)
            {
                json += ",{\"id\": \"" + item + "\"}";
            }

            var keyValues = new List<KeyValuePair<string, string>> {
                new KeyValuePair<string, string>("jobs_id", json),
                new KeyValuePair<string, string>("date", date.ToShortDateString())
            };

            var request = new HttpRequestMessage(HttpMethod.Post, Configs.server + "/api/marcacoes/cadeiras");

            request.Content = new FormUrlEncodedContent(keyValues);

            var client = new HttpClient();
            var response = await client.SendAsync(request);
            string resultLog = await response.Content.ReadAsStringAsync();

            var cadeirasJson = JsonObject.Parse(resultLog);

            for (int i = 0; i < cadeirasJson[0].Count; i++)
            {
                cadeiras.Add(new Cadeira() { idCadeira = (int)cadeirasJson[0][i]["id"], displayName = cadeirasJson[0][i]["slot"], backgroundColor = Color.Gray, textColor = Color.Gray });
            }

            for (int i = 0; i < cadeirasJson[1].Count; i++)
            {
                cadeirasInde.Add(((int)cadeirasJson[1][i]["id"]).ToString());
                cadeiras.Add(new Cadeira() { idCadeira = (int)cadeirasJson[1][i]["id"], displayName = cadeirasJson[1][i]["slot"], backgroundColor = Color.Red, textColor = Color.Red });
            }

            cadeiraStack.FlowItemsSource = cadeiras;
            cadeiraStackList.IsVisible = true;
            loading.IsVisible = false;
        }

        Grid lastTappedJob = null;
        void Handle_Tapped(object sender, System.EventArgs e)
        {
            jobsSelected.Clear();
            Grid gr = (Grid)sender;
            StackLayout stack = (StackLayout) gr.Children[0];
            Label lb = (Label)stack.Children[0];

            if (lastTappedJob != null) {
                StackLayout stackTemp = (StackLayout)lastTappedJob.Children[0];
                Label lbTemp = (Label)stackTemp.Children[0];

                lastTappedJob.BackgroundColor = Color.Gray;
                stackTemp.BackgroundColor = Color.WhiteSmoke;
                lbTemp.TextColor = Color.Gray;
            }

            lastTappedJob = gr;

            gr.BackgroundColor = Color.Black;
            stack.BackgroundColor = Color.Black;
            lb.TextColor = Color.White;

            jobsSelected.Add(gr.ClassId);

            /* if (jobsSelected.Contains(gr.ClassId))
             {
                 gr.BackgroundColor = Color.Gray;
                 stack.BackgroundColor = Color.WhiteSmoke;
                 lb.TextColor = Color.Gray;

                 jobsSelected.Remove(gr.ClassId);
             }
             else
             {
                 gr.BackgroundColor = Color.Black;
                 stack.BackgroundColor = Color.Black;
                 lb.TextColor = Color.White;

                 jobsSelected.Add(gr.ClassId);
             }*/

            if (jobsSelected.Count <= 0)
            {
                avancarButao.IsEnabled = false;
            }
            else
            {
                avancarButao.IsEnabled = true;
            }

        }

        void Retroceder_Clicked(object sender, System.EventArgs e)
        {
            if (stage == 1)
            {
                stage1.FadeTo(0, 250);
                stage0.FadeTo(1, 250);

                stage1.IsEnabled = false;
                stage0.IsEnabled = true;

                stage1.InputTransparent = true;

                retrocederButao.IsEnabled = false;
                avancarButao.IsEnabled = true;

                stage--;
            }

            if (stage == 2)
            {
                stage2.FadeTo(0, 250);
                stage1.FadeTo(1, 250);

                avancarButao.Text = "Avançar";

                stage2.IsEnabled = false;
                stage1.IsEnabled = true;

                stage2.InputTransparent = true;

                retrocederButao.IsEnabled = true;
                avancarButao.IsEnabled = true;

                stage--;
            }

            if (stage == 3)
            {

                stage3.FadeTo(0, 250);
                stage2.FadeTo(1, 250);

                avancarButao.Text = "Avançar";

                stage3.IsEnabled = false;
                stage2.IsEnabled = true;

                stage3.InputTransparent = true;

                retrocederButao.IsEnabled = true;
                avancarButao.IsEnabled = true;

                stage--;
            }
        }

        bool isFinishingMarcacao = false;
        void Avancar_Clicked(object sender, System.EventArgs e)
        {
            if(stage == 0)
            {
            
                retrocederButao.IsEnabled = true;
                caderiaSelecionada = null;
                avancarButao.IsEnabled = false;

                if (lastSelectedCadeira != null)
                {

                    StackLayout stackTemp = (StackLayout)lastSelectedCadeira.Children[0];
                    Label lbTemp = (Label)stackTemp.Children[0];

                    lastSelectedCadeira.BackgroundColor = Color.Gray;
                    stackTemp.BackgroundColor = Color.WhiteSmoke;
                    lbTemp.TextColor = Color.Gray;

                }

                stage0.FadeTo(0, 250);
                stage1.FadeTo(1,250);
                stage1.InputTransparent = false;

                LoadCadeiras();

                stage0.IsEnabled = false;
                stage1.IsEnabled = true;


                stage++;
            }

            if (stage == 1)
            {
                if (avancarButao.IsEnabled == false)
                    return;

                retrocederButao.IsEnabled = true;
                avancarButao.IsEnabled = false;

                stage1.FadeTo(0, 250);
                stage2.FadeTo(1, 250);
                stage2.InputTransparent = false;


                checkDisponiblidade();

                stage1.IsEnabled = false;
                stage2.IsEnabled = true;

                avancarButao.IsEnabled = false;

                stage++;
            }

            if (stage == 2)
            {
                if (avancarButao.IsEnabled == false)
                    return;

                retrocederButao.IsEnabled = true;
                avancarButao.IsEnabled = false;

                stage2.FadeTo(0, 250);
                stage3.FadeTo(1, 250);
                stage3.InputTransparent = false;

                stage1.IsEnabled = false;
                stage2.IsEnabled = false;
                stage3.IsEnabled = true;

                avancarButao.Text = "Finalizar";

                barber.Text = cadeiras.Where(x => x.idCadeira.ToString() == caderiaSelecionada).First().displayName;
                dateHour.Text = Marcacoes.dateSelected.ToShortDateString() + " - " + horaSelecionada;

                var checkCount = 0;
                foreach (var item in jobsSelected)
                {   
                    if(checkCount == 0)
                        jobsToDisplay.Text = jobs.Where(x => x.idJob.ToString() == item).First().displayName;
                    else
                        jobsToDisplay.Text = jobsToDisplay.Text + ", " + jobs.Where(x => x.idJob.ToString() == item).First().displayName;

                    checkCount++;
                }

                reviewStackList.IsVisible = true;

                stage++;
                yieldForXTime();
            }

            if (stage == 3)
            {
                if (avancarButao.IsEnabled == false || isFinishingMarcacao)
                    return;

                isFinishingMarcacao = true;
                avancarButao.IsEnabled = false;

                FinalizarMarcacao();
            }
        }

        async Task yieldForXTime()
        {
            await Task.Delay(1000);
            avancarButao.IsEnabled = true;
        }

        async Task FinalizarMarcacao()
        {
            this.Content.FadeTo(0, 250);
            Navigation.PushModalAsync(new Loading());

            string json = "";
            foreach (var item in jobsSelected)
            {
                json += ",{\"id\": \"" + item + "\"}";
            }

            DateTime date = Marcacoes.dateSelected;

            var keyValues = new List<KeyValuePair<string, string>> {
                new KeyValuePair<string, string>("date", date.ToShortDateString()),
                new KeyValuePair<string, string>("cadeira", caderiaSelecionada),
                new KeyValuePair<string, string>("user_id", User.id.ToString()),
                new KeyValuePair<string, string>("jobs_id", json),
                new KeyValuePair<string, string>("hora", horaSelecionada)
            };

            var request = new HttpRequestMessage(HttpMethod.Post, Configs.server + "/api/marcacoes/new");

            request.Content = new FormUrlEncodedContent(keyValues);

            var client = new HttpClient();
            var response = await client.SendAsync(request);
            string resultLog = await response.Content.ReadAsStringAsync();

            Navigation.PopModalAsync();
            this.Content.FadeTo(1, 250);
            if (resultLog == "true")
            {
                Marcacoes.refreshCalendar();
                Navigation.PopPopupAsync();
                await DisplayAlert("Sucesso", "Marcação efetuada com sucesso", "Entendi");
            }
            else
            {
                await DisplayAlert("Erro", "Ocorreu um erro ao efetuar a marcação", "Entendi");
            }
        }

        async Task checkDisponiblidade()
        {
            loading.IsVisible = true;

            horas.Clear();

            if (lastSelectedHora != null)
            {

                StackLayout stackTemp = (StackLayout)lastSelectedHora.Children[0];
                Label lbTemp = (Label)stackTemp.Children[0];

                lastSelectedHora.BackgroundColor = Color.Gray;

                lbTemp.TextColor = Color.Gray;
                stackTemp.BackgroundColor = Color.WhiteSmoke;

                lastSelectedHora = null;
                horaSelecionada = null;
                avancarButao.IsEnabled = false;
            }

            DateTime date = Marcacoes.dateSelected;

            string json = "";
            foreach (var item in jobsSelected)
            {
                json += ",{\"id\": \"" + item + "\"}";
            }


            var keyValues = new List<KeyValuePair<string, string>> {
                new KeyValuePair<string, string>("date", date.ToShortDateString()),
                new KeyValuePair<string, string>("cadeira", caderiaSelecionada),
                new KeyValuePair<string, string>("user_id", User.id.ToString()),
                new KeyValuePair<string, string>("jobs_id", json)
            };

            var request = new HttpRequestMessage(HttpMethod.Post, Configs.server + "/api/marcacoes/verficar-disponiblidade");

            request.Content = new FormUrlEncodedContent(keyValues);

            var client = new HttpClient();
            var response = await client.SendAsync(request);
            string resultLog = await response.Content.ReadAsStringAsync();

            var horasJson = JsonObject.Parse(resultLog);

            for (int i = 0; i < horasJson.Count; i++)
            {
                horas.Add(new Hora() { hora = (string)horasJson[i] });
            }

            hoursStack.FlowItemsSource = horas;
            hoursStackList.IsVisible = true;
            loading.IsVisible = false;


        }

        Grid lastSelectedCadeira = null;
        void Cadeira_Tapped(object sender, System.EventArgs e)
        {

            Grid gr = (Grid)sender;
            StackLayout stack = (StackLayout)gr.Children[0];
            Label lb = (Label)stack.Children[0];

            if (cadeirasInde.Contains(gr.ClassId))
            {
                DisplayAlert("Indisponível", "Este barbeiro não se econtra disponível!", "Entendi");
                return;
            }

            if (lastSelectedCadeira != null)
            {

                StackLayout stackTemp = (StackLayout)lastSelectedCadeira.Children[0];
                Label lbTemp = (Label)stackTemp.Children[0];

                lastSelectedCadeira.BackgroundColor = Color.Gray;
                stackTemp.BackgroundColor = Color.WhiteSmoke;
                lbTemp.TextColor = Color.Gray;
            }

            gr.BackgroundColor = Color.Black;
            stack.BackgroundColor = Color.Black;
            lb.TextColor = Color.White;

            lastSelectedCadeira = gr;

            avancarButao.IsEnabled = true;

            caderiaSelecionada = gr.ClassId;
        }

        Grid lastSelectedHora = null;
        void OverView_Tapped(object sender, System.EventArgs e)
        {
            Grid gr = (Grid)sender;
            StackLayout stack = (StackLayout)gr.Children[0];
            Label lb = (Label)stack.Children[0];

            if (lastSelectedHora != null)
            {

                StackLayout stackTemp = (StackLayout)lastSelectedHora.Children[0];
                Label lbTemp = (Label)stackTemp.Children[0];

                lastSelectedHora.BackgroundColor = Color.Gray;
                stackTemp.BackgroundColor = Color.WhiteSmoke;
                lbTemp.TextColor = Color.Gray;
            }

            gr.BackgroundColor = Color.Black;
            stack.BackgroundColor = Color.Black;
            lb.TextColor = Color.White;

            lastSelectedHora = gr;

            avancarButao.IsEnabled = true;

            horaSelecionada = gr.ClassId;
        }
    }
}
