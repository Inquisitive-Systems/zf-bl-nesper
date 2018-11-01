/*
ZoneFox Business Layer event processor based on GNU NEsper
Copyright (C) 2018 ZoneFox

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 2 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License along
with this program; if not, write to the Free Software Foundation, Inc.,
51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
*/

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