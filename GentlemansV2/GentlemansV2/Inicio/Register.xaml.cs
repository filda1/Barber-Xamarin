//using Stormlion.ImageCropper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Rg.Plugins.Popup.Extensions;
using GentlemansV2;

namespace GentlemansV2
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Register : ContentPage
	{
		string imageURL = null;

        public Register(string firstName, string lastName, string email, string picture)
		{
			InitializeComponent ();

            if(picture != null)
			    profileImage.Source = picture;

			imageURL = picture;
			firstNameEntry.Text = firstName;
			lastNameEntry.Text = lastName;
			emailEntry.Text = email;

            dateEntry.Unfocus();

        }

		public async Task SendRegisterInfo() {

            if (imageURL == null)
            {
                imageURL = "useDefault";
            }

            if (firstNameEntry.Text == null)
            {
                DisplayAlert("Erro", "O campo Nome tem de ser preenchido.", "Entendi");
                return;
            }

            if (lastNameEntry.Text == null)
            {
                DisplayAlert("Erro", "O campo Apelido tem de ser preenchido.", "Entendi");
                return;
            }

            if (emailEntry.Text == null)
            {
                DisplayAlert("Erro", "O campo Email tem de ser preenchido.", "Entendi");
                return;
            }

            if (passwordEntry.Text != confirmPasswordEntry.Text || firstNameEntry.Text == null)
            {
                await DisplayAlert("Erro", "Password's não coincide!", "Ok");
                await Navigation.PopModalAsync();
                return;
            }

            Navigation.PushModalAsync(new Loading());

            var date = dateEntry.Date;

            var keyValues = new List<KeyValuePair<string, string>> {
				new KeyValuePair<string, string>("first_name", firstNameEntry.Text),
				new KeyValuePair<string, string>("last_name", lastNameEntry.Text),
				new KeyValuePair<string, string>("email", emailEntry.Text),
				new KeyValuePair<string, string>("password", passwordEntry.Text),
                new KeyValuePair<string, string>("imageURL", imageURL),
                new KeyValuePair<string, string>("number", numeroEntry.Text),
                new KeyValuePair<string, string>("birthday", date.ToShortDateString()),
                new KeyValuePair<string, string>("dateSelected", dateSelected),
            };

            var form = new MultipartFormDataContent();

            if (imagePath != null)
            {
                var b = File.ReadAllBytes(imagePath);
                var stream = new MemoryStream(b);

                var bytes = new byte[stream.Length];
                await stream.ReadAsync(bytes, 0, (int)stream.Length);
                string base64 = System.Convert.ToBase64String(bytes);

                StringContent fileStreamContent = new StringContent(base64);

                form.Add(fileStreamContent, "image");

            }

            HttpContent DictionaryItems = new FormUrlEncodedContent(keyValues);


           

            form.Add(DictionaryItems, "info");

            var client = new HttpClient();
            var response = await client.PostAsync(Configs.server + "/api/user/new", form);

            string resultLog = await response.Content.ReadAsStringAsync();

			await Navigation.PopModalAsync();

			if (resultLog == "success") {
				await Navigation.PushAsync(new Entrar(emailEntry.Text));
				Navigation.RemovePage(this);
            }
            else if(resultLog == "email")
            {
                await DisplayAlert("Erro", "Já se encontra registada uma conta com o mesmo email.", "Entendi");
            }else {
				await DisplayAlert("Erro", "Ocorreu um erro a efetuar o registo, por favor tente novamente mais tarde.", "Entendi");
			}
		}
        string imagePath = null;
        private void ChoosePick(object sender, EventArgs e)
        {
            /*new ImageCropper()
            {
                SelectSourceTitle = "Selecione o método",
                TakePhotoTitle = "Capturar Fotografia",
                PhotoLibraryTitle = "Galeria de Fotos",
                AspectRatioX = 1,
                AspectRatioY = 1,
                CropShape = ImageCropper.CropShapeType.Oval,
                Success = (imageFile) => {

                    Device.BeginInvokeOnMainThread(() => {
                        imagePath = imageFile;

                        changeImage.IsVisible = false;
						profileImage.Source = ImageSource.FromFile(imageFile);
					});
                }
            }.Show(this);*/
        }
        string dateSelected = "na";
        void Handle_DateSelected(object sender, Xamarin.Forms.DateChangedEventArgs e)
        {
            dateSelected = "s";
            OpenDataPicker.Text = dateEntry.Date.ToShortDateString();
        }
        
        void Numero_Focused(object sender, Xamarin.Forms.FocusEventArgs e)
        {
            Navigation.PushPopupAsync(new TooltipNumero());
        }

        void Numero_Unfocused(object sender, Xamarin.Forms.FocusEventArgs e)
        {
            Navigation.PopPopupAsync();
        }
        private void OpenDataPicker_Clicked(object sender, EventArgs e)
        {
            dateEntry.Focus();
        }
        private void Button_Clicked(object sender, EventArgs e)
        {
            SendRegisterInfo();
        }
    }
}