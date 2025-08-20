namespace AppRpgEtec
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            //primeira pag. á a parecer:<codigo>
            MainPage = new NavigationPage(new Views.Usuarios.LoginView());
        }

        //protected override Window CreateWindow(IActivationState? activationState)
        //{
        //  return new Window(new AppShell());
        //}
    }
}