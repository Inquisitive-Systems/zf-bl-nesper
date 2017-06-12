using System.IO;

namespace ZF.BL.Nesper.Utils
{
    public class StringUtil
    {
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
    }
}