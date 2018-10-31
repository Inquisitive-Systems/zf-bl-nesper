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
using System.Runtime.Serialization;

namespace ZF.BL.Nesper.Wcf.Service
{
    [DataContract]
    public class ZoneFoxFault
    {
        [DataMember]
        public string StackTrace { get; set; }

        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public string InnerException { get; set; }

        [DataMember]
        public string AdditionalDetails { get; set; }

        public static ZoneFoxFault CreateFrom(Exception ex)
        {
            var fault = new ZoneFoxFault();
            if (ex.StackTrace != null) 
                fault.StackTrace = ex.StackTrace;
            if (ex.InnerException != null) 
                fault.InnerException = ex.InnerException.Message;

            fault.Message = ex.Message;
            fault.AdditionalDetails = ex.ToString();

            return fault;
        }
    }
}