using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AppRpgEtec.Models;
using AppRpgEtec.Models.Enuns;
using AppRpgEtec.Services.Personagens;

namespace AppRpgEtec.ViewModels.Personagens
{
    [QueryProperty("PersonagemSelecionadoId", "pId")]
    public class CadastroPersonagemViewModel : BaseViewModel
    {
        private PersonagemService pService;
        public ICommand SalvarCommand { get;}
        public ICommand CancelarCommand { get; }

        
        

        public CadastroPersonagemViewModel()
        {
            string token = Preferences.Get("UsuarioToken", string.Empty);
            pService = new PersonagemService(token);
            _ = ObterClasses();

            SalvarCommand = new Command(async () => { await SalvarPersonagem(); });
            CancelarCommand = new Command(async => CancelarCadastro());
        }

        private ObservableCollection<TipoClasse> listaTiposClasse;
        public ObservableCollection<TipoClasse> ListaTiposClasse
        {
            get { return listaTiposClasse; }
            set
            {
                listaTiposClasse = value;
                OnPropertyChanged();
            }

        }
        private TipoClasse tipoClasseSelecionado;
        public TipoClasse TipoClasseSelecionado
        {
            get { return tipoClasseSelecionado; }
            set
            {
                if(value != null)
                {
                    tipoClasseSelecionado = value;
                    OnPropertyChanged();
                }
            } 
        }

        private async void CancelarCadastro()
        {
            await Shell.Current.GoToAsync("..");
        }

        public async void CarregarPersonagem()
        {
           try
            {
                Personagem p = await pService.GetPersonagemAsync(int.Parse(PersonagemSelecionadoId1));

                this.Nome = p.Nome;
                this.PontosVida = p.PontosVida;
                this.Defesa = p.Defesa;
                this.Derrotas = p.Derrotas;
                this.Disputas = p.Disputas;
                this.Forca = p.Forca;
                this.Inteligencia = p.Inteligencia;
                this.Vitorias = p.Vitorias;
                this.id = p.Id;

                TipoClasseSelecionado = this.ListaTiposClasse
                    .FirstOrDefault(tClasse => tClasse.Id == (int)p.Classe);
            }
            catch(Exception ex)
            {
                await Application.Current.MainPage
                    .DisplayAlert("Ops", ex.Message + "Detalhes: " + ex.InnerException, "Ok");
            }
        }

        private string personagemSelecionadoId; 
        public string PersonagemSelecionadoId
        {
            set
            {
                if (value != null)
                {
                    personagemSelecionadoId = Uri.UnescapeDataString(value);
                    CarregarPersonagem();
                }
            }
        }
       

        #region GetSet
        private int id;
        private string nome;
        private int pontosVida;
        private int forca;
        private int defesa;
        private int inteligencia;
        private int disputas;
        private int vitorias;
        private int derrotas;

        public int Id 
        {
            get => id; 
            set
            {
                id = value;
                OnPropertyChanged();
            }
                
        }
        public string Nome 
        { 
            get => nome; 
            set 
            { 
                nome = value;
                OnPropertyChanged();
            }
        }
        public int PontosVida 
        { 
            get => pontosVida; 
            set 
            { 
                pontosVida = value;
                OnPropertyChanged();
            }
        }
        public int Forca 
        { 
            get => forca; 
            set 
            { 
                forca = value; 
                OnPropertyChanged();
            }
        }
        public int Defesa 
        { 
            get => defesa; set 
            { 
                defesa = value; 
                OnPropertyChanged();
            }
        }
        public int Inteligencia 
        { 
            get => inteligencia; 
            set 
            { 
                inteligencia = value; 
                OnPropertyChanged();
            } 
        }
        public int Disputas 
        { 
            get => disputas; 
            set 
            { 
                disputas = value; 
                OnPropertyChanged();
            }
        }
        public int Vitorias 
        { 
            get => vitorias; 
            set 
            { 
                vitorias = value; 
                OnPropertyChanged();
            } 
        }
        public int Derrotas 
        { 
            get => derrotas; set 
            { 
                derrotas = value; 
                OnPropertyChanged();
            } 
        }

        //public string PersonagemSelecionado { get => PersonagemSelecionado1; set => PersonagemSelecionado1 = value; }
       // public string PersonagemSelecionado1 { get => personagemSelecionado; set => personagemSelecionado = value; }
        public string PersonagemSelecionadoId1 { get => personagemSelecionadoId; set => personagemSelecionadoId = value; }
        #endregion

        public async Task ObterClasses()
        {
            try
            {
                ListaTiposClasse = new ObservableCollection<TipoClasse>();
                ListaTiposClasse.Add( new TipoClasse() { Id =1, Descricao = "Cavaleiro" });
                ListaTiposClasse.Add(new TipoClasse() { Id = 2, Descricao = "Mago" });
                ListaTiposClasse.Add(new TipoClasse() { Id = 3, Descricao = "Clerigo" });
                OnPropertyChanged(nameof(ListaTiposClasse));
            }
            catch (Exception ex) 
            {
                await Application.Current.MainPage
                    .DisplayAlert("Ops", ex.Message + "Detalhes: " + ex.InnerException,"Ok");
            }
        }

        public async Task SalvarPersonagem()
        {
            try
            {
                Personagem model = new Personagem()
                {
                    Nome = this.nome,
                    PontosVida = this.pontosVida,
                    Defesa = this.defesa,
                    Derrotas = this.derrotas,
                    Disputas = this.disputas,
                    Forca = this.forca,
                    Inteligencia = this.inteligencia,
                    Vitorias = this.vitorias,
                    Id = this.id,
                    Classe = (ClasseEnum)tipoClasseSelecionado.Id
                };
                if (model.Id == 0) 
                    await pService.PostPersonagemAsync(model);
                else
                    await pService.PutPersonagemAsync (model);

                await Application.Current.MainPage.DisplayAlert("Mensagem","Dados salvos com sucesso!","Ok");
                await Shell.Current.GoToAsync(".."); // Remove a pagina atual da pilha de paginas
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage
                    .DisplayAlert("Ops", ex.Message + "Detalhes" + ex.InnerException, "Ok");
            }
        }

    }
}
