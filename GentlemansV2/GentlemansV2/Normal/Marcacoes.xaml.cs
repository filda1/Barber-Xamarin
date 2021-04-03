using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Json;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using GentlemansV2;
using Rg.Plugins.Popup.Extensions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XamForms.Controls;

namespace GentlemansV2
{
    public class Marc
    {
        public int idJob { get; set; }
        public string cadeira { get; set; }
        public string dayMonth { get; set; }
        public string date { get; set; }
        public string year { get; set; }
        public string hour { get; set; }
        public string imageURL { get; set; }
        public DateTime _DATE { get; set; }
    }
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Marcacoes : TabbedPage
    {

        public static int cadeiraID;
        public static int jobID;

        public static DateTime dateSelected;

        public ObservableCollection<Marc> olds = new ObservableCollection<Marc>();
        public ObservableCollection<Marc> gets = new ObservableCollection<Marc>();
        public ObservableCollection<Marc> dayInfo = new ObservableCollection<Marc>();

        public static Marcacoes instance;

        public Marcacoes ()
        {
            InitializeComponent();

            instance = this;

            GetOld();
            Get();
        }

        public static void refreshCalendar()
        {
            instance.Get();
            instance.GetDayMarcacoes((DateTime) instance.calendarView.SelectedDate);
        }

        bool hasMarcarToolBar = false;

        private void DataSelecionada(object sender, XamForms.Controls.DateTimeEventArgs e)
        {
            XamForms.Controls.Calendar cal = (XamForms.Controls.Calendar)sender;
            DateTime date = (DateTime) cal.SelectedDate;
            dateSelected = (DateTime)cal.SelectedDate;

            DateTime myDate = DateTime.ParseExact("2019-03-01", "yyyy-MM-dd",
                                       System.Globalization.CultureInfo.InvariantCulture);

            DateTime maxDateRealese = DateTime.ParseExact("2019-04-01", "yyyy-MM-dd",
                                       System.Globalization.CultureInfo.InvariantCulture);

            if ( myDate > date)
            {
                DisplayAlert("", "Não é possivel realizar marcações para este dia", "Entendi");
                cal.SelectedDate = null;
                hasMarcarToolBar = false;
                this.ToolbarItems.Clear();
                return;
            }

            if (( date - DateTime.Now ).TotalDays >= 15)
            {   
                if(date >= maxDateRealese)
                {
                    DisplayAlert("Indisponível", "Devido à situação do COVID só aceitamos marcações em prazo de 15 dias não comprometendo as mesmas se algo acontecer. Obrigado!", "Entendi");
                    cal.SelectedDate = null;
                    hasMarcarToolBar = false;
                    this.ToolbarItems.Clear();
                    return;
                }
                    
            }

            if (date <= DateTime.Now.AddDays(-1))
            {
                DisplayAlert("Indisponível", "Não pode realizar marcações em datas passadas", "Entendi");
                cal.SelectedDate = null;
                hasMarcarToolBar = false;
                this.ToolbarItems.Clear();
                return;
            }

            if (loadingIndi.Opacity > 0)
            {
                return;
            }

            if (gets.Any(p => p._DATE == date))
            {
                infoNon.FadeTo(0, 250);
                loadingIndi.FadeTo(1, 250);
                stackListDay.FadeTo(0, 250);
                dayInfo.Clear();
                GetDayMarcacoes(date);
            }
            else
            {
                loadingIndi.FadeTo(0, 250);
                infoNon.FadeTo(1, 250);
                stackListDay.FadeTo(0, 250);
                dayInfo.Clear();
                getStack.FlowItemsSource = dayInfo;
            }

            if (hasMarcarToolBar)
               return;

            hasMarcarToolBar = true;

            var marcar = new ToolbarItem
            {
                Text = "Marcar"
            };

            if  (Device.RuntimePlatform == Device.Android) {
                marcar.Icon = "plus.png";
            }
                 
     
            marcar.Clicked += ContinueWithMarcacao;

            this.ToolbarItems.Add(marcar);

        }

        async Task GetDayMarcacoes(DateTime _date)
        {
            dayInfo.Clear();

            var data = _date.Date;

            var keyValues = new List<KeyValuePair<string, string>> {
                new KeyValuePair<string, string>("user_id", User.id.ToString()),
                new KeyValuePair<string, string>("date", data.ToShortDateString())
            };

            var request = new HttpRequestMessage(HttpMethod.Post, Configs.server + "/api/marcacoes/get/day" );

            request.Content = new FormUrlEncodedContent(keyValues);

            var client = new HttpClient();
            var response = await client.SendAsync(request);
            string resultLog = await response.Content.ReadAsStringAsync();

            var jobsJson = JsonObject.Parse(resultLog);


            for (int i = 0; i < jobsJson.Count; i++)
            {

                var date = DateTime.ParseExact(jobsJson[i]["dia"], "yyyy-MM-dd", CultureInfo.InvariantCulture);

                var day = date.ToString("dd");
                var month = date.ToString("MMM");
                var year = date.ToString("yyyy");
                string[] days = DateTimeFormatInfo.CurrentInfo.AbbreviatedDayNames;
                var dayOfWeek = days[(int)date.DayOfWeek] + ",";

                dayInfo.Add(new Marc() { idJob = (int)jobsJson[i]["id"], cadeira = jobsJson[i]["cadeira_id"], date = date.ToShortDateString() + " " + jobsJson[i]["hora_inicio"], dayMonth = day + " " + month, year = year, hour = jobsJson[i]["hora_inicio"] });
            }

            getStack.FlowItemsSource = dayInfo;
            loadingIndi.FadeTo(0, 250);
            stackListDay.FadeTo(1, 250);
        }

        void ContinueWithMarcacao(object sender, EventArgs e)
        {
            Navigation.PushPopupAsync(new Marcar());
        }

        async Task Get()
        {
            calendarView.SpecialDates.Clear();
            var request = new HttpRequestMessage(HttpMethod.Get, Configs.server + "/api/marcacoes/get/" + User.id);

            calendarView.SpecialDates.Add(new SpecialDate(DateTime.Now)
            {
                Selectable = true,
                BackgroundColor = Color.FromHex("#4d4d4d"),
                BorderColor = Color.Transparent,
                TextColor = Color.White,
                BorderWidth = 0
            });

            var client = new HttpClient();
            var response = await client.SendAsync(request);
            string resultLog = await response.Content.ReadAsStringAsync();

            var jobsJson = JsonObject.Parse(resultLog);

            for (int i = 0; i < jobsJson.Count; i++)
            {
                var date = DateTime.ParseExact(jobsJson[i]["dia"], "yyyy-MM-dd", CultureInfo.InvariantCulture);

                calendarView.SpecialDates.Add(new SpecialDate(date) {
                    Selectable = true,
                    BackgroundColor = Color.White,
                    BorderColor = Color.FromHex("#ffcf40"),
                    BorderWidth = 3
                });

                calendarView.RaiseSpecialDatesChanged();

                gets.Add(new Marc() { idJob = (int)jobsJson[i]["id"], _DATE = date, cadeira = jobsJson[i]["cadeira_id"], date = date.ToShortDateString(), hour = jobsJson[i]["hora_inicio"] });
            }

            Navigation.PopModalAsync();

        }

        async Task GetOld()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, Configs.server + "/api/marcacoes/old/" + User.id);

            var client = new HttpClient();
            var response = await client.SendAsync(request);
            string resultLog = await response.Content.ReadAsStringAsync();

            var jobsJson = JsonObject.Parse(resultLog);

            for (int i = 0; i < jobsJson.Count; i++)
            {
                infoNA.IsVisible = false; 
                var date = DateTime.ParseExact(jobsJson[i]["dia"], "yyyy-MM-dd", CultureInfo.InvariantCulture);

                var day = date.ToString("dd");
                var month = date.ToString("MMM");
                var year = date.ToString("yyyy");
                string[] days = DateTimeFormatInfo.CurrentInfo.AbbreviatedDayNames;
                var dayOfWeek = days[(int)date.DayOfWeek] + ",";

                olds.Add(new Marc() { idJob = (int)jobsJson[i]["id"], imageURL = jobsJson[i]["image"], cadeira = jobsJson[i]["cadeira_id"], date = date.ToShortDateString() + " " + jobsJson[i]["hora_inicio"], dayMonth = day + " " + month, year = year, hour = jobsJson[i]["hora_inicio"] });
            }

            historicoStack.FlowItemsSource = olds;
        }

        void Photo_Click(object sender, System.EventArgs e)
        {
            Grid grid = (Grid)sender;
            
            Navigation.PushAsync(new ImageFinish(grid.ClassId));
        }

        void Marcacao_Tapped(object sender, System.EventArgs e)
        {
            Grid _grid = (Grid)sender;
            Navigation.PushPopupAsync(new InfoMarcacao( _grid.ClassId ));
        }
    }
}