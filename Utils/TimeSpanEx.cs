using System;

namespace ZF.BL.Nesper.Utils
{
    public static class TimeSpanEx
    {
        public static string ToPrettySeconds(this TimeSpan span)
        {
            return new DateTime(span.Ticks).ToString("s.ff") + " sec";
        }
    }
}