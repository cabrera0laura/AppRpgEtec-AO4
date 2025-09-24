using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppRpgEtec.Models;
using AppRpgEtec.Services.Armas;

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
         }
        // Proximos elementos da classe aqui.  PDF 5 - CONTINUAR .....

       /* public async TasK ObterPersonagem()
        {
            try
            {
                Armas = await aService.GetArmasAsync();
                //.........
            }
            catch(Exception)
            {

            }
        }*/

       

    } // Fim da classe
}
