using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppRpgEtec.Models;
using AppRpgEtec.Services.Personagens;

namespace AppRpgEtec.ViewModels.Personagens
{
    public class ListagemPersonagemViewModel : BaseViewModel
    {
        private PersonagemService pService;
        public ObservableCollection<Personagem> Personagens { get; set; }
        public ListagemPersonagemViewModel()
        {
            string token = Preferences.Get("UsuarioToken", string.Empty);
            pService = new PersonagemService(token);
            Personagens = new ObservableCollection<Personagem>();
        }

        // Proximos elementos da classe aqui

        public async Task ObterPersonagens()
        {
            try // Junto com o Cacth evitará que erros fechem o aplicativo
            {
                Personagens = await pService.GetPersonagensAsync();
                OnPropertyChanged(nameof(Personagens));  // Informara a view que houve carregamento
            }
            catch (Exception ex) 
            {
                // Captara o erro para exibir em tela 
                await Application.Current.MainPage
                    .DisplayAlert("Ops", ex.Message, "Detalhes" + ex.InnerException, "Ok");
            }
        }

        public ListagemPersonagemViewModel()
        {
            string token = Preferences.Get("UsuarioToken",string.Empty);
            pService = new PersonagemService(token);
            Personagens = new ObservableCollection<Personagem>();

            _=ObterPersonagens();
        }
    }
}
