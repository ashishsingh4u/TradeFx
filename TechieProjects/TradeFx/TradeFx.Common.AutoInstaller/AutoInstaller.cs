using System.ComponentModel;
using System.Configuration;
using System.Configuration.Install;

namespace TradeFx.Common.AutoInstaller
{
    [RunInstaller(true)]
    public partial class AutoInstaller :Installer
    {
        public AutoInstaller()
        {
            InitializeComponent();

            autoServiceInstaller.ServiceName = ConfigurationManager.AppSettings["ServiceName"];
        }
    }
}
