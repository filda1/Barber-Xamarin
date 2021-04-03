using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace GentlemansV2
{
    public partial class Localizacao : ContentPage
    {


        public Localizacao()
        {
            InitializeComponent();

            if (Device.RuntimePlatform == Device.Android)
            {
                CheckPermission();
            }
            else
            {
                carregarMapa();
            }

        }

        async Task CheckPermission()
        {
            try
            {
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
                if (status != PermissionStatus.Granted)
                {
                    var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Location);

                    if (results.ContainsKey(Permission.Location))
                        status = results[Permission.Location];
                }

                if (status == PermissionStatus.Granted)
                {
                    carregarMapa();
                }
                else if (status != PermissionStatus.Unknown)
                {
                    await DisplayAlert("Localização Necessária", "É necessária a sua localização para que possa prosseguir tente novamente.", "Entendi");
                    Navigation.PopAsync();
                }
            }
            catch (Exception ex)
            {
                Debug.Write("aqui: "+ex.Message);

            }
        }

        void carregarMapa()
        {
            var map = new Map(
            MapSpan.FromCenterAndRadius(
                    new Position(41.2776419, -8.2775404), Distance.FromMiles(0.2)))
            {
                IsShowingUser = true,
                HeightRequest = 100,
                WidthRequest = 960,
                VerticalOptions = LayoutOptions.FillAndExpand,
                MapType = MapType.Satellite
            };

            var position = new Position(41.2776419, -8.2775404);
            var pin = new Pin
            {
                Type = PinType.Place,
                Position = position,
                Label = "Gentleman's Barbershop Lousada",
                Address = "RUA GENERAL HUMBERTO DELGADO, Nº40"
            };
            map.Pins.Add(pin);

            var stack = new StackLayout { Spacing = 0 };
            stack.Children.Add(map);
            Content = stack;
        }
    }
}