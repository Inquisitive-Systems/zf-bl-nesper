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

        public static string TrimEnd(string input, string trimString)
        {
            return input.TrimEnd(trimString.ToCharArray());
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