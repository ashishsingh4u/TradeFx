using System.Windows;

namespace TradeFx.Shell
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // The boostrapper will create the Shell instance, so the App.xaml does not have a StartupUri.
            Bootstrapper bootstrapper = new Bootstrapper();
            bootstrapper.Run();
        }

    }
}
