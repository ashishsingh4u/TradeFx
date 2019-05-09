using log4net.Config;

namespace AutoWindowsService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            XmlConfigurator.Configure();
            ServiceHelper.Execute(args, new AutoService());
        }
    }
}
