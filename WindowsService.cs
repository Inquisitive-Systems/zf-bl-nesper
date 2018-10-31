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