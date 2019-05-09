using System;
using System.Configuration.Install;
using System.Reflection;
using System.ServiceProcess;

namespace TradeFx.Common.AutoInstaller
{
    public class ServiceHelper
    {
        private static readonly string ExePath =
           Assembly.GetExecutingAssembly().Location;
        public static bool InstallMe()
        {
            try
            {
                ManagedInstallerClass.InstallHelper(
                    new[] { ExePath });
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static bool UninstallMe()
        {
            try
            {
                ManagedInstallerClass.InstallHelper(
                    new[] { "/u", ExePath });
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static bool Launch(ServiceBase[] services)
        {
            try
            {
                if (Environment.UserInteractive)
                {
                    Type type = typeof(ServiceBase);
                    const BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic;
                    var method = type.GetMethod("OnStart", Flags);

                    foreach (var service in services)
                    {
                        method.Invoke(service, new object[] {null});
                    }

                    Console.WriteLine("Press any key to exit");
                    Console.Read();


                    foreach (var service in services)
                    {
                        service.Stop();
                    }
                }
                else
                {
                    ServiceBase.Run(services);
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static void Execute(string[] args, params ServiceBase[] servicesToRun)
        {
            if (args != null && args.Length == 1 && args[0].Length > 1 && (args[0][0] == '-' || args[0][0] == '/'))
            {
                switch (args[0].Substring(1).ToLower())
                {
                    case "i":
                    case "install":
                        InstallMe();
                        break;
                    case "u":
                    case "uninstall":
                        UninstallMe();
                        break;
                }
            }
            else
            {
                Launch(servicesToRun);
            }
        }
    }
}
