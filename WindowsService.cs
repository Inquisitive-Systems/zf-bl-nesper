using System;
using System.ServiceProcess;
using ZF.BL.Nesper.Model;

namespace ZF.BL.Nesper
{
    public class WindowsService : ServiceBase
    {
        private readonly ApplicationManager _appMgr;

        public WindowsService(ApplicationManager applicationManager)
        {
            if (applicationManager == null)
                throw new ArgumentNullException("applicationManager");

            _appMgr = applicationManager;
        }

        protected override void OnStart(string[] args)
        {
            _appMgr.Start();
        }

        protected override void OnStop()
        {
            _appMgr.Stop();
        }
    }
}