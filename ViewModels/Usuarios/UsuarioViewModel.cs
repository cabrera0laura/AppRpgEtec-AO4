using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AppRpgEtec.Models;
using AppRpgEtec.Services.Usuarios;

namespace AppRpgEtec.ViewModels.Usuarios
{
    public class UsuarioViewModel : BaseViewModel
    {
        public UsuarioViewModel()
        {
            uServices = new UsuarioService();
            //chama os metodo de baxo(Horganização).
            InicializarCommands();
        }
        public void InicializarCommands()
        {
            AutenticarCommand = new Command(async () => await AutenticarUsuario());
        }
        private UsuarioService uServices;
        public ICommand AutenticarCommand { get; set; }
       

        //region compacta o codigo visualmente.
        #region AtributosPropriedades
        private string login = string.Empty;
        private string senha = string.Empty;

        //gerar GET/SET Ctrl + r + e
        public string Login 
        { 
            get => login;
            set 
            { 
                login = value;
                OnPropertyChanged();
            }
        }
        public string Senha 
        { 
            get => senha;
            set
            { 
                senha = value; 
                OnPropertyChanged();
            }
        }


        #endregion

        #region Metodos
        public async Task AutenticarUsuario()
        {
            try
            {
                // metodo de chamada para API
                Usuario u = new Usuario();
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

                    await Application.Current.MainPage.DisplayAlert("Informação", mensagem, "Ok");
                    Application.Current.MainPage = new MainPage();
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
        #endregion
    }
}
