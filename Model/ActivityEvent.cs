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
using System.Collections.Generic;
using System.Linq;

namespace ZF.BL.Nesper.Model
{
    public class ActivityEvent
    {
        public string Machine { get; set; }
        public string User { get; set; }
        public string Application { get; set; }
        public string Activity { get; set; }
        public string Resource { get; set; }
        public string File { get; set; }
        public string Extension { get; set; }

        public List<string> Folder { get; set; }

        public string FlattenedFolders
        {
            get
            {
                return Folder.Any() ? string.Join(",", Folder) : string.Empty;
            }
        }

        //THIS CANNOT BE UPDATED TO DATETIMEOFFSET AS NESPER HAS LITERALLY NO IDEA ABOUT IT.
        public DateTime OccurredOn { get; set; }
        public int ProcessId { get; set; }

        public NetworkEventMetaData NetworkEvent { get; set; }
    }
}