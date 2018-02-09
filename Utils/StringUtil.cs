using System;
using System.Globalization;
using System.IO;

namespace ZF.BL.Nesper.Utils
{
    public class StringUtil
    {
        /// <summary>
        /// Represents the inclusivity/exclusivity state of a search query
        /// </summary>
        public enum RangeOpenness
        {
            /// <summary>
            /// Query which includes its boundary elements
            /// </summary>
            Inclusive,
            /// <summary>
            /// Query which excludes its boundary elements
            /// </summary>
            Exclusive
        }

        public static string TrimEnd(string input, string trimSting)
        {
            return input.TrimEnd(trimSting.ToCharArray());
        }

        public static bool FirstStringStartsWithSecond(string first, string second)
        {
            return first.StartsWith(second);
        }

        public static bool IsSameFile(string first, string second)
        {
            try
            {
                var firstFName = Path.GetFileName(first);
                var secondFName = Path.GetFileName(second);

                return  firstFName != null &&
                        secondFName != null &&
                        firstFName.TrimEnd('\\') == secondFName.TrimEnd('\\');
            }
            catch
            {
                return false;
            }
        }

        public static bool InRange(long val, string from, string to, string openness)
        {
            var fromL = long.Parse(from, CultureInfo.InvariantCulture);
            var toL = long.Parse(to, CultureInfo.InvariantCulture);
            var open = (RangeOpenness)Enum.Parse(typeof(RangeOpenness), openness, true);

            if (open == RangeOpenness.Inclusive)
            {
                return fromL <= val && val <= toL;
            }

            //Exclusive
            return fromL < val && val < toL;
        }

        public static bool InRange(DateTime val, string from, string to, string openness)
        {
            var fromDate = DateTime.ParseExact(from, "o", CultureInfo.InvariantCulture);
            var toDate = DateTime.ParseExact(to, "o", CultureInfo.InvariantCulture);
            var open = (RangeOpenness) Enum.Parse(typeof(RangeOpenness), openness, true);

            if (open == RangeOpenness.Inclusive)
            {
                return fromDate <= val && val <= toDate;
            }

            //Exclusive
            return fromDate < val && val  < toDate;
        }
    }
}