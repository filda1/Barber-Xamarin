using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xamarin.Forms;

namespace GentlemansV2
{
    public partial class ImagePreview : ContentPage
    {
        public ImagePreview(string url)
        {
            InitializeComponent();

            Debug.WriteLine("link->"+url);

            ImageToPreview.Source = url;
        }
    }
}
