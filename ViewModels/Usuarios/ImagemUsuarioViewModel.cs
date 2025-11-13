using AppRpgEtec.Models;
using AppRpgEtec.Services.Usuarios;
using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;


namespace AppRpgEtec.ViewModels.Usuarios
{
    public class ImagemUsuarioViewModel : BaseViewModel
    {
        private UsuarioService uService;
        private static string conexaoAzureStorage = "ConexaoAzure";
        private static string container = "arquivos"; // nome do conteiner criado

        public ImagemUsuarioViewModel()
        {
            string token = Preferences.Get("UsuarioToken", string.Empty);
            uService = new UsuarioService(token);

            FotografarCommand = new Command(Fotografar);
            SalvarImagemAzureCommand = new Command(SalvarImagemAzure);
            AbrirGaleriaCommand = new Command(AbrirGaleria);
            CarregarUsuarioAzure();
        }

        public ICommand FotografarCommand { get; }
        public ICommand SalvarImagemAzureCommand { get; }

        public ICommand AbrirGaleriaCommand { get; }

        // Proximas etapas aqui

        private ImageSource fonteImagem;

        public ImageSource FonteImagem 
        {
            get {return fonteImagem; }
            set
            {
                fonteImagem = value;
                OnPropertyChanged();
            }
        }

        private byte[] foto;

        public byte[] Foto
        {
            get => foto;
            set
            { 
                foto = value;
                OnPropertyChanged();
            }
        }

        public async void Fotografar()
        {
            try
            {
                // Codificacção aqui


                //Verificar se o dispositivo suporta midia como camera e galeria
                if(MediaPicker.Default.IsCaptureSupported)
                {
                    // Chamada para a camera do dispositivo. Fica aguardando usuario tirar foto
                    FileResult photo = await MediaPicker.Default.CapturePhotoAsync();

                    if(photo != null)
                    {
                        using (Stream sourceStream = await photo.OpenReadAsync()) // Leitura dos bytes da foto para..
                        {
                            using (MemoryStream ms = new MemoryStream())
                            {
                                await sourceStream.CopyToAsync(ms);

                                Foto = ms.ToArray();

                                FonteImagem = ImageSource.FromStream(() => new MemoryStream(ms.ToArray()));
                            }
                        }
                    }
                }

            }
            catch(Exception ex)
            {
                await Application.Current.MainPage
                    .DisplayAlert("Ops", ex.Message + "Detalhes: " + ex.InnerException, "Ok");
            }
        }

        public async void SalvarImagemAzure()
        {
            try
            {
                Usuario u = new Usuario();
                u.Foto = foto;
                u.Id = Preferences.Get("UsuarioId", 0);

                string fileName = $"{u.Id}.jpg";

                // Define o BLOB no qual a imagem será armazenada
                var blobClient = new BlobClient(conexaoAzureStorage, container, fileName);

                if (blobClient.Exists())
                    blobClient.Delete();

                using (var stream = new MemoryStream(u.Foto))
                {
                    blobClient.Upload(stream);
                }

                await Application.Current.MainPage.DisplayAlert("Menssagem", "Dados salvos com sucesso!", "Ok");
                await App.Current.MainPage.Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage
                    .DisplayAlert("Ops", ex.Message + "Detalhes: " + ex.InnerException, "Ok");
            }
        }

        public async void AbrirGaleria()
        {
            try
            {
                // Codificacção aqui


                //Verificar se o dispositivo suporta midia como camera e galeria
                if (MediaPicker.Default.IsCaptureSupported)
                {
                    // Chamada para a camera do dispositivo. Fica aguardando usuario tirar foto
                    FileResult photo = await MediaPicker.Default.PickPhotoAsync();

                    if (photo != null)
                    {
                        using (Stream sourceStream = await photo.OpenReadAsync()) // Leitura dos bytes da foto para..
                        {
                            using (MemoryStream ms = new MemoryStream())
                            {
                                await sourceStream.CopyToAsync(ms);

                                Foto = ms.ToArray();

                                FonteImagem = ImageSource.FromStream(() => new MemoryStream(ms.ToArray()));
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                await Application.Current.MainPage
                    .DisplayAlert("Ops", ex.Message + "Detalhes: " + ex.InnerException, "Ok");
            }
        }

        public async void CarregarUsuarioAzure()
        {
            try
            {
                int usuarioId = Preferences.Get("UsuarioId", 0);
                string filename = $"{usuarioId}.jpg";
                var blobCliente = new BlobClient(conexaoAzureStorage, container, filename);

                if (blobCliente.Exists())
                {
                    Byte[] filebytes;

                    using (MemoryStream ms  = new MemoryStream())
                    {
                        blobCliente.OpenRead().CopyTo(ms);
                        filebytes = ms.ToArray();
                    }
                    Foto = filebytes;
                }

            }
            catch (Exception ex)
            {
                await Application.Current.MainPage
                    .DisplayAlert("Ops", ex.Message + "Detalhes: " + ex.InnerException, "Ok");
            }
        }
    }
}
