using System.ComponentModel;
using System.ServiceProcess;

namespace AutoWindowsService
{
    partial class AutoInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.autoServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.autoServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // autoServiceProcessInstaller
            // 
            this.autoServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.autoServiceProcessInstaller.Password = null;
            this.autoServiceProcessInstaller.Username = null;
            // 
            // autoServiceInstaller
            // 
            this.autoServiceInstaller.ServiceName = "MyAutoService";
            // 
            // AutoInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.autoServiceProcessInstaller,
            this.autoServiceInstaller});

        }

        #endregion

        private ServiceProcessInstaller autoServiceProcessInstaller;
        private ServiceInstaller autoServiceInstaller;
    }
}