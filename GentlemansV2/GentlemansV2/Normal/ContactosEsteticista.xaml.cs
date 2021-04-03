using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GentlemansV2.Normal
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ContactosEsteticista : ContentPage
    {
        public ContactosEsteticista()
        {
            InitializeComponent();
        }

        void Contacto_Tapped(object sender, System.EventArgs e)
        {
            Device.OpenUri(new Uri("tel:" + "+351913367986"));
        }

        void Email_Tapped(object sender, System.EventArgs e)
        {
            Device.OpenUri(new Uri("mailto:joo_moreira@hotmail.com"));
        }

        void Facebook_Tapped(object sender, System.EventArgs e)
        {
            Device.OpenUri(new Uri("fb://page/317026452296898"));
        }

        void Instagram_Tapped(object sender, System.EventArgs e)
        {
            Device.OpenUri(new Uri("instagram://user?username=joanamoreira_atelie"));
        }
    }
}