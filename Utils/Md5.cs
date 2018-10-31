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

using System.Security.Cryptography;
using System.Text;

namespace ZF.BL.Nesper.Utils
{
    public class Md5
    {
        public string GetHashAsHex(string s)
        {
            using (MD5 hasher = MD5.Create())
            {
                var inputBytes = Encoding.UTF8.GetBytes(s);
                byte[] hash = hasher.ComputeHash(inputBytes);

                var sb = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                    sb.Append(hash[i].ToString("x2"));

                // Return the hexadecimal string. 
                return sb.ToString();
            }
        } 
    }
}