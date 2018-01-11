using System;
using System.Collections.Generic;
using System.Globalization;

namespace ZF.BL.Nesper.Model
{
    public class NetworkEventMetaData
    {
        /// <summary>
        /// File Path
        /// </summary>
        public string Fp { get; set; }

        /// <summary>
        /// File size in bytes
        /// </summary>
        public long Fs { get; set; }

        /// <summary>
        /// The file extension
        /// </summary>
        public string Ext { get; set; }

        /// <summary>
        /// Transfer size in bytes
        /// </summary>
        public long Ts { get; set; }

        /// <summary>
        /// Source IP
        /// </summary>
        public string Sip { get; set; }

        /// <summary>
        /// Source Port
        /// </summary>
        public int Sp { get; set; }

        /// <summary>
        /// Destination IP
        /// </summary>
        public string Dip { get; set; }

        /// <summary>
        /// Destination Port
        /// </summary>
        public int Dp { get; set; }

        /// <summary>
        /// Protocol
        /// </summary>
        public string P { get; set; }

        /// <summary>
        /// Destination Host
        /// </summary>
        public string Dh { get; set; }

        /// <summary>
        /// The resolved location of the source or destination depending on the type of network event
        /// </summary>
        public Region Loc { get; set; }
    }

    public class EventDto
    {
        public string D { get; set; }
        public string M { get; set; }
        public string U { get; set; }
        public string Ap { get; set; }
        public string Ac { get; set; }
        public string R { get; set; }
        public string File { get; set; }
        public string Ext { get; set; }
        public List<string> Folder { get; set; }

        /// <summary>
        /// Metadata Network
        /// </summary>
        public NetworkEventMetaData Mn { get; set; }

        public ActivityEvent ToActivityEvent()
        {
            return new ActivityEvent
            {
                OccurredOn = DateTime.Parse(D, null, DateTimeStyles.AssumeUniversal),
                Machine = M,
                User = U,
                Activity = Ac,
                Application = Ap,
                Resource = R,
                NetworkEvent = Mn,
                File = File,
                Ext = Ext,
                Folder = Folder
            };
        }

        
    }
}
