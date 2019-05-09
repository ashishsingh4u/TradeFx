using System.Windows;

using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.UnityExtensions;
using Microsoft.Practices.ServiceLocation;

namespace TradeFx.Shell
{
    internal class Bootstrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return ServiceLocator.Current.GetInstance<TradeFxAppShell>();
        }

        protected override void InitializeModules()
        {
            base.InitializeModules();
            Application.Current.MainWindow = (TradeFxAppShell)Shell;
            Application.Current.MainWindow.Show();
        }

        protected override IModuleCatalog CreateModuleCatalog()
        {
            return new DirectoryModuleCatalog { ModulePath = @".\Modules" };
        }
    }
}
