using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Rg.Plugins.Popup.Extensions;

namespace GentlemansV2.Normal
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChoosePage : ContentPage
    {
        public ChoosePage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            pageOpen = false;
        }
        bool pageOpen = false;

        private async Task Barbeiro_Clicked(object sender, EventArgs e)
        {
            if (pageOpen)
                return;

            StackLayout stack = (StackLayout)sender;
            Image innerChild = (Image)stack.Children[0];

            await innerChild.ScaleTo(0.8, 150);
            await innerChild.ScaleTo(1, 150);

            await Navigation.PushAsync(new MenuPrincipal());
        }

        private async Task Esteticista_Clicked(object sender, EventArgs e)
        {
            if (pageOpen)
                return;

            StackLayout stack = (StackLayout)sender;
            Image innerChild = (Image)stack.Children[0];

            await innerChild.ScaleTo(0.8, 150);
            await innerChild.ScaleTo(1, 150);

            await Navigation.PushAsync(new Esteticista());
        }

        void Definitions_Clicked(object sender, System.EventArgs e)
        {
            Navigation.PushPopupAsync(new Definitions());
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            await Barbeiro_Clicked(sender, e);
        }

        private async void TapGestureRecognizer_Tapped_1(object sender, EventArgs e)
        {
            await Esteticista_Clicked(sender, e);
        }
    }
}