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