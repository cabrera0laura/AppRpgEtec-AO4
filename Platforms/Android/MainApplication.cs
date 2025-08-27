using Android.App;
using Android.Runtime;

namespace AppRpgEtec
{
    [Application(UsesCleartextTraffic=true)] //  habilite o emulador trafegar dados JSON conforme a instrução sinalizada abaixo
    public class MainApplication : MauiApplication
    {
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}
