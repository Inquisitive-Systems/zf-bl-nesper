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

namespace ZF.BL.Nesper.Utils
{
    public static class BlLog
    {
        //This names shall match log4net config file names
        public const string ExceptionLog = "BL.Exceptions";
        public const string EventProcessingLog = "BL.EventProcessing";
        public const string AlertsLog = "BL.Alerts";
        //public const string ApiLog = "BL.API";
        //public const string EventsPerformanceLog = "BL.Performance.Events";
    }
}