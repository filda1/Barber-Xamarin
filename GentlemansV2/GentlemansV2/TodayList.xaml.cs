using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Json;
using System.Net.Http;
using System.Threading.Tasks;
using GentlemansV2;
using Rg.Plugins.Popup.Extensions;
using Xamarin.Forms;
using XamForms.Controls;

namespace GentlemansV2
{

    public class dayList
    {
        public string id { get; set; }
        public string hour { get; set; }
        public string DisplayName { get; set; }
        public string jobs { get; set; }
    }

    public class jobDay
    {
        public string DisplayName { get; set; }
    }

    public partial class TodayList : TabbedPage
    {

        public TodayList()
        {
            InitializeComponent();

            getDay();
        }

        ObservableCollection<dayList> dayLists = new ObservableCollection<dayList>();

        async Task getDay()
        {
            dayLists.Clear();

            var keyValues = new List<KeyValuePair<string, string>> {
                    new KeyValuePair<string, string>("cadeira_id", User.id.ToString())
                };

            var request = new HttpRequestMessage(HttpMethod.Post, Configs.server + "/api/barber/get/day");

            request.Content = new FormUrlEncodedContent(keyValues);

            var client = new HttpClient();
            var response = await client.SendAsync(request);
            string resultLog = await response.Content.ReadAsStringAsync(); 

            var jobsJson = JsonObject.Parse(resultLog);

            for (int i = 0; i < jobsJson.Count; i++)
            {

                dayLists.Add(new dayList() { id = ((int)jobsJson[i]["id"]).ToString(), DisplayName = jobsJson[i]["user_name"], hour = jobsJson[i]["hora_inicio"], jobs = jobsJson[i]["trabalhosName"] } );
            }

            todayView.ItemsSource = dayLists;
        }

        void Definitions_Clicked(object sender, System.EventArgs e)
        {
            Navigation.PushPopupAsync(new DefinitionsAdmin());
        }

        void OpenMarcacao_Tapped(object sender, System.EventArgs e)
        {
            StackLayout stack = (StackLayout)sender;

            Navigation.PushAsync(new FechoMarcacao(stack.ClassId));
        }
    }

}
