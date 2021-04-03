using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GentlemansV2
{
    public partial class ImageFinishEstetica : ContentPage
    {
        Marc tmp;
        string id;

        public ImageFinishEstetica(string id)
        {
            InitializeComponent();
            this.id = id;
            tmp = Marcacoes.instance.olds.Where(x => x.idJob.ToString() == id).First();

            if (tmp.imageURL != "na")
            {
                ImageToPreview.Source = Configs.server + "/storage/" + tmp.imageURL;
                var marcar = new ToolbarItem
                {
                    Text = "Partilhar"
                };

                if (Device.RuntimePlatform == Device.Android)
                {
                    //marcar.Icon = "plus.png";
                }


                marcar.Clicked += ShareButton;

                this.ToolbarItems.Add(marcar);
            }
            else
            {
                loading.IsVisible = false;
                ChoosePick();
            }

        }

        private void ShareButton(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        string imagePath = null;
        private async Task ChoosePick()
        {
            await Task.Delay(250);
            //new ImageCropper()
            //{
            //    SelectSourceTitle = "Selecione o método",
            //    TakePhotoTitle = "Capturar Fotografia",
            //    PhotoLibraryTitle = "Galeria de Fotos",
            //    AspectRatioX = 1,
            //    AspectRatioY = 1,
            //    Success = (imageFile) => {

            //        Device.BeginInvokeOnMainThread(() => {
            //            imagePath = imageFile;

            //            ImageToPreview.Source = ImageSource.FromFile(imageFile);
            //            uploadImage();
            //        });
            //    }
            //}.Show(this);

        }

        async Task uploadImage()
        {
            await Navigation.PushModalAsync(new Loading());

            var keyValues = new List<KeyValuePair<string, string>> {
                new KeyValuePair<string, string>("marcacao_id", tmp.idJob.ToString())
            };

            var b = File.ReadAllBytes(imagePath);
            var stream = new MemoryStream(b);

            var bytes = new byte[stream.Length];
            await stream.ReadAsync(bytes, 0, (int)stream.Length);
            string base64 = System.Convert.ToBase64String(bytes);

            HttpContent DictionaryItems = new FormUrlEncodedContent(keyValues);
            StringContent fileStreamContent = new StringContent(base64);


            var form = new MultipartFormDataContent();

            form.Add(DictionaryItems, "info");
            form.Add(fileStreamContent, "image");

            var client = new HttpClient();
            var response = await client.PostAsync(Configs.server + "/api/marcacao/upload-image", form);

            string resultLog = await response.Content.ReadAsStringAsync();

            Marcacoes.instance.olds.Where(x => x.idJob.ToString() == id).First().imageURL = resultLog;

            await Navigation.PopModalAsync();
        }

    }
}
