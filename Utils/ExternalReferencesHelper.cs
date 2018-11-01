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

using Castle.Core;
using Common.Logging;
using Nito.KitchenSink.CRC;

namespace ZF.BL.Nesper.Utils
{
    /// <summary>
    ///     Dummy file to help resolve build problem. Do not use anywhere in code.
    /// </summary>
    internal class ExternalReferencesHelper
    {
        private ExternalReferencesHelper(){}
        //--------------------------
        // Important! Do not remove the following private members
        // This declarations make sure that the libraries are copied to the output folder
        // Otherwise they may not be deployed causing runtime error

#pragma warning disable 0169 // disables compiler warning Field ABC is never used
        // ReSharper disable UnusedMember.Local
        private IServiceProviderEx _ex;
        private IConfigurationReader _r;
        private CRC16 _crc16;

        // ReSharper restore UnusedMember.Local
#pragma warning restore 0169
        //-------------------------- 
    }
}