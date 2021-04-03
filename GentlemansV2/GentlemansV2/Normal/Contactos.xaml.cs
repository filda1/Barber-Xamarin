using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace GentlemansV2
{
    public partial class Contactos : ContentPage
    {
        public Contactos()
        {
            InitializeComponent();
        }

        void Contacto_Tapped(object sender, System.EventArgs e)
        {
            Device.OpenUri(new Uri("tel:" + "+351913762183"));
        }

        void Email_Tapped(object sender, System.EventArgs e)
        {
            Device.OpenUri(new Uri("mailto:geral@gentlemansbarbershop.pt"));
        }

        void Facebook_Tapped(object sender, System.EventArgs e)
        {
            Device.OpenUri(new Uri("fb://page/1863169583967819"));
        }

        void Instagram_Tapped(object sender, System.EventArgs e)
        {
            Device.OpenUri(new Uri("instagram://user?username=gentlemans_barbershop_lousada"));
        }
    }
}
