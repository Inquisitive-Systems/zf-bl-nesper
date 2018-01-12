﻿using System;
using System.Collections.Generic;

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

        //THIS CANNOT BE UPDATED TO DATETIMEOFFSET AS NESPER HAS LITERALLY NO IDEA ABOUT IT.
        public DateTime OccurredOn { get; set; }
        public int ProcessId { get; set; }

        public NetworkEventMetaData NetworkEvent { get; set; }
    }
}