using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AppRpgEtec.Models;
using AppRpgEtec.Services.Armas;
using AppRpgEtec.ViewModels;

namespace AppRpgEtec.ViewModels.Armas
{
    internal class ListagemArmaViewModel : BaseViewModel
    {
       
         private ArmaService aService;
         public ObservableCollection<Arma> Armas { get; set; }
         public ListagemArmaViewModel()
         {
            string token = Preferences.Get("UsuarioToken", string.Empty);
            aService = new ArmaService(token);
            Armas = new ObservableCollection<Arma>();

            _ = ObterArmas();
            NovaArmaCommand = new Command(async () => { await ExibirCadastroArma(); });
         }
        // Proximos elementos da classe aqui.  PDF 5 - CONTINUAR .....
        public ICommand NovaArmaCommand { get; set; }

        public async Task ObterArmas()
        {
            try
            {
                Armas = await aService.GetArmasAsync();
                OnPropertyChanged(nameof(Armas));
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage
                    .DisplayAlert("Ops", ex.Message + " Detalhes" + ex.InnerException, "Ok");
            }
        }

        public async Task ExibirCadastroArma()
        {
            try
            {
                await Shell.Current.GoToAsync("cadArmasView");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage
                    .DisplayAlert("Ops", ex.Message + "Detalhes: " + ex.InnerException, "Ok");
            }
        }



       

    } // Fim da classe
}
