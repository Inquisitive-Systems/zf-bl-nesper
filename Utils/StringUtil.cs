using System.IO;
using System.Linq;

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

        public static bool ContainsFolders(string[] policyFolders, string[] eventFolders)
        {
            if (eventFolders == null|| policyFolders == null)
                return false;

            if (!eventFolders.Any() || !policyFolders.Any())
                return false;

            return eventFolders.Any(x => policyFolders.Any(y => y == x));
        }
    }
}