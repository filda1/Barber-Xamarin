using System;
using Com.OneSignal;
using DLToolkit.Forms.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace GentlemansV2
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            FlowListView.Init();

            Configs._mainPage = new MainPage();
            MainPage = new NavigationPage(Configs._mainPage);

            MainPage.SetValue(NavigationPage.BarBackgroundColorProperty, Color.Black);

            OneSignal.Current.StartInit("3f305ede-5599-46d6-a753-f036c4149b45").EndInit();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
