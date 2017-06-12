using System.Diagnostics;
using System.Reflection;

namespace ZF.BL.Nesper.Utils
{
    public static class Version
    {
        public static string GetAssemblyVersionFor<T>()
        {
            Assembly assembly = Assembly.GetAssembly(typeof (T));
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            return fvi.FileVersion;
        }
    }
}