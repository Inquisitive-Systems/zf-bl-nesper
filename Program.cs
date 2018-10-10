using System;
using System.Linq;
using System.ServiceProcess;
using ZF.BL.Nesper.Model;

namespace ZF.BL.Nesper
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var appMgr = new ApplicationManager();

            if (args.ToList().Contains("--console"))
            {
                appMgr.Start();
                Console.Read();
                appMgr.Stop();
            }
            else
            {
                var winService = new WindowsService(appMgr);
                ServiceBase.Run(winService);
            }
        }
    }
}