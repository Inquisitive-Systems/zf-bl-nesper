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
using System.ServiceModel;

namespace ZF.BL.Nesper.Wcf.Service
{
    public class WcfHost
    {
        private readonly ServiceHost _serviceHost;

        public WcfHost(object service, string name)
        {
            if (service == null) throw new ArgumentNullException("service");
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");

            _serviceHost = new ServiceHost(service);
            Name = name;
        }

        public string Name { get; private set; }

        public void Start()
        {
            if (_serviceHost != null)
                _serviceHost.Open();
        }

        public void Stop()
        {
            if (_serviceHost != null)
                _serviceHost.Close();
        }
    }
}