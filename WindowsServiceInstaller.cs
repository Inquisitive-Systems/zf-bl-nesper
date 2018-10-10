using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace ZF.BL.Nesper
{
    [RunInstaller(true)]
    public class WindowsServiceInstaller : Installer
    {
        private readonly ServiceProcessInstaller _process;
        private readonly ServiceInstaller _service;

        public WindowsServiceInstaller()
        {
            _process = new ServiceProcessInstaller
                {
                    Account = ServiceAccount.LocalSystem
                };
            _service = new ServiceInstaller
                {
                    ServiceName = "ZoneFox.Bl.Nesper",
                    StartType = ServiceStartMode.Automatic,
                    Description = "ZoneFox wrapper service around NEsper"
                };
            Installers.Add(_process);
            Installers.Add(_service);
        }
    }
}