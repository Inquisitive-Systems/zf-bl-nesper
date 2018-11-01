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
using System.Linq;

namespace ZF.BL.Nesper.Utils
{
    public class EplParser
    {
        public const string StartMarker = "[DO NOT FIRE ALERT BLOCK START]";
        public const string StopMarker = "[DO NOT FIRE ALERT BLOCK STOP]";
        public static char[] TrimChars = new[] {'\n', '\r', '\t', ' '};

        public bool HasSubscriptionMarkers(string input)
        {
            return input.Contains(StartMarker) && input.Contains(StopMarker);
        }

        public EplParsedTuple Parse(string input)
        {
            var output = new EplParsedTuple();

            // Get part of a string with no alert subscription
            int idxStart = input.IndexOf(StartMarker, StringComparison.Ordinal) + StartMarker.Length + 1;
            for (int i = idxStart; i < input.Length; i++)
            {
                if (TrimChars.Contains(input[i]))
                {
                    idxStart++;
                }
                else
                {
                    break;
                }
            }

            int idxEnd = input.IndexOf(StopMarker, StringComparison.Ordinal) - 1;
            for (int i = idxEnd; i > 0; i--)
            {
                if (TrimChars.Contains(input[i]))
                {
                    idxEnd--;
                }
                else
                {
                    break;
                }
            }

            int len = idxEnd - idxStart + 1;

            output.EplScript = input.Substring(idxStart, len);

            //**********
            // Get part of a string with alert subscription
            //**********
            idxStart = input.IndexOf(StopMarker, StringComparison.Ordinal) + StopMarker.Length + 1;

            for (int i = idxStart; i < input.Length; i++)
            {
                if (TrimChars.Contains(input[i]))
                {
                    idxStart++;
                }
                else
                {
                    break;
                }
            }

            idxEnd = input.Length - 1;

            for (int i = idxEnd; i > 0; i--)
            {
                if (TrimChars.Contains(input[i]))
                {
                    idxEnd--;
                }
                else
                {
                    break;
                }
            }

            len = idxEnd - idxStart + 1;
            if(len > 0) //we have a statement to subscribe to
                output.StatementToFireAlert = input.Substring(idxStart, len);

            return output;
        }
    }
}