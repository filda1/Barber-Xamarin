using GentlemansV2;
using System.Collections.ObjectModel;
using System.Json;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GentlemansV2
{
    public class Imagem
    {
        public string ImageUrl { get; set; }
        public string loadingURL { get; set; }
    }

    public partial class AppGaleria : ContentPage
    {
        public ObservableCollection<Imagem> imagensHair = new ObservableCollection<Imagem>();
        public ObservableCollection<Imagem> imagensOld = new ObservableCollection<Imagem>();
        public ObservableCollection<Imagem> imagensNew = new ObservableCollection<Imagem>();
        public ObservableCollection<Imagem> imagensKids = new ObservableCollection<Imagem>();


        public AppGaleria()
        { 
            InitializeComponent();
            LoadImages();
        }

        private void OpenImage(object sender) {
            Grid grid = (Grid)sender;

            Navigation.PushAsync(new ImagePreview(grid.ClassId));
        }

        async Task LoadImages()
        {
            Navigation.PushModalAsync(new Loading());

            var request = new HttpRequestMessage(HttpMethod.Get, Configs.server + "/api/galeria");

            var client = new HttpClient();
            var response = await client.SendAsync(request);
            string resultLog = await response.Content.ReadAsStringAsync();

            var images = JsonObject.Parse(resultLog);

            var countOld = 1;
            var countNew = 1;
            var countKids = 1;
            var countHair = 1;

            var countOldTmp = 0;
            var countNewTmp = 0;
            var countKidsTmp = 0;
            var countHairTmp = 0;
            for (int i = 0; i < images.Count; i++)
            {
                if (images[i]["type"] == "Old School")
                {
                    imagensOld.Add(new Imagem() { ImageUrl = Configs.server + "/storage/" + images[i]["file"] });
                    countOldTmp++;

                    if (countOldTmp >= 4)
                    {
                        countOld++;
                        countOldTmp = 0;
                    }

                }
                else if (images[i]["type"] == "New School")
                {
                    imagensNew.Add(new Imagem() { ImageUrl = Configs.server + "/storage/" + images[i]["file"] });
                    countNewTmp++;

                    if (countNewTmp >= 4)
                    {
                        countNew++;
                        countNewTmp = 0;
                    }
                }
                else if (images[i]["type"] == "Kids")
                {
                    imagensKids.Add(new Imagem() { ImageUrl = Configs.server + "/storage/" + images[i]["file"] });
                    countKidsTmp++;

                    if (countKidsTmp >= 4)
                    {
                        countKids++;
                        countKidsTmp = 0;
                    }
                }
                else if (images[i]["type"] == "Hair Design")
                {
                    imagensHair.Add(new Imagem() { ImageUrl = Configs.server + "/storage/" + images[i]["file"] });
                    countHairTmp++;

                    if (countHairTmp >= 4)
                    {
                        countHair++;
                        countHairTmp = 0;
                    }
                }

            }

            galeriaStackOld.FlowItemsSource = imagensOld;
            galeriaStackNew.FlowItemsSource = imagensNew;
            galeriaStackKids.FlowItemsSource = imagensKids;
            galeriaStackHair.FlowItemsSource = imagensHair;


            galeriaStackOld.HeightRequest = 110 * countOld;
            galeriaStackNew.HeightRequest = 110 * countNew;
            galeriaStackKids.HeightRequest = 110 * countKids;
            galeriaStackHair.HeightRequest = 110 * countHair;



            Navigation.PopModalAsync();
        }

        private void TapGestureRecognizer_Tapped(object sender, System.EventArgs e)
        {
            OpenImage(sender);
        }

    }
}

