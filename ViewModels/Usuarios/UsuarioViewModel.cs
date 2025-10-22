using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AppRpgEtec.Models;
using AppRpgEtec.Services.Usuarios;
using AppRpgEtec.Views.Personagens;
using AppRpgEtec.Views.Usuarios;

namespace AppRpgEtec.ViewModels.Usuarios
{
    public class UsuarioViewModel : BaseViewModel
    {
        public UsuarioViewModel()
        {
            //
            uServices = new UsuarioService();
            //chama os metodo de baixo(Horganização).
            InicializarCommands();
        }
        public void InicializarCommands()
        {
            AutenticarCommand = new Command(async () => await AutenticarUsuario());
            RegistrarCommand = new Command(async () => await RegistrarUsuario());
            DirecionarCadastroCommand = new Command(async () => await DirecionarParaCadastro());
        }
        private UsuarioService uServices;
        public ICommand AutenticarCommand { get; set; }
        public ICommand RegistrarCommand { get; set; }
        public ICommand DirecionarCadastroCommand { get; set; }
       

        //region compacta o codigo visualmente.
        #region AtributosPropriedades
        private string login = string.Empty;
        private string senha = string.Empty;

        //gerar GET/SET Ctrl + r + e
        public string Login 
        {
            get {return  login; }
            set 
            { 
                login = value;
                OnPropertyChanged();
            }
        }
        public string Senha 
        {
            get { return senha; }
            set
            { 
                senha = value; 
                OnPropertyChanged();
            }
        }


        #endregion

        #region Metodos

        // atributos para controlar a operação de coleta de geolocalização
        private CancellationTokenSource _cancelTokenSource;
        private bool _isCheckingLocation;

        public async Task AutenticarUsuario()
        {
            try
            {
                // metodo de chamada para API
                Usuario u = new Usuario();
                u.Username = login;
                u.PasswordString = senha;

                //Chamada a API
                Usuario uAutenticado = await uServices.PostAutenticarUsuarioAsync(u);

                //Se for diferente de vazio, Se não...
                if (!string.IsNullOrEmpty(uAutenticado.Token))
                {
                    string mensagem = $"Bem-vindo(a) {uAutenticado.Username}";

                    //Guarda dados para uso futuro
                    Preferences.Set("UsuarioId", uAutenticado.Id);
                    Preferences.Set("UsuarioUsername", uAutenticado.Username);
                    Preferences.Set("UsuarioPerfil", uAutenticado.Perfil);
                    Preferences.Set("UsuarioToken", uAutenticado.Token);

                    // Início da coleta de Geolocalização atual para Atualizar na API
                    _isCheckingLocation = true;
                    _cancelTokenSource = new CancellationTokenSource();
                    GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));

                    Location location = await Geolocation.Default.GetLocationAsync(request, _cancelTokenSource.Token);

                    Usuario uLoc = new Usuario();
                    uLoc.Id = uAutenticado.Id;
                    uLoc.Latitude = location.Latitude;
                    uLoc.Longitude = location.Longitude;

                    UsuarioService uServiceLoc = new UsuarioService(uAutenticado.Token);
                    await uServiceLoc.PutAtualizarLocalizacaoAsync(uLoc);
                    // Fim da coleta de Geolocalização atual para Atualização na API

                    await Application.Current.MainPage.DisplayAlert("Informação", mensagem, "Ok");

                    Application.Current.MainPage = new AppShell();
                    // Alteração para que view inicial possa ser a de listagem.
                }
                else
                {
                    await Application.Current.MainPage
                        .DisplayAlert("Informação", "Dados incorretos! 🤨 ", "Ok");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage
                    .DisplayAlert("Informações", ex.Message + ex.InnerException, "Ok");
            }
        }

        public async Task RegistrarUsuario() //Metodo para registrar um usuario
        {
            try
            {
                //Proxima codificacao
                Usuario u = new Usuario();
                u.Username = Login;
                u.PasswordString = senha;

                Usuario uRegistrado = await uServices.PostRegistrarUsuarioAsync(u);

                if (uRegistrado.Id != 0)
                {
                    string mensagem = $"Usuário Id {uRegistrado.Id} registrado com sucesso.";
                    await Application.Current.MainPage.DisplayAlert("Informação", mensagem, "Ok");

                    await Application.Current.MainPage.Navigation.PopAsync(); // Remove  a pagina da pilha de visualização.
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage
                    .DisplayAlert("Informação", ex.Message + "Detalhes:" + ex.InnerException, "Ok");
            }
        }

        public async Task DirecionarParaCadastro() //Método para exibição da view de Cadastro
        {
            try
            {
                await Application.Current.MainPage
                    .Navigation.PushAsync(new CadastroView());
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage
                    .DisplayAlert("Informação", ex.Message, "Detalhes" + ex.InnerException, "Ok");
            }
        }

        #endregion
    }
}
