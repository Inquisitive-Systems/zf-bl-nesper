using System;
using System.IO;
using System.Reflection;

namespace ZF.BL.Nesper.Utils
{
    public static class PathLocator
    {
        public static void SetCurrentDirectoryPathFromExecutingAssembly()
        {
            string exeLocation = Assembly.GetExecutingAssembly().Location;
            string root = Directory.GetParent(exeLocation).FullName;
            Environment.CurrentDirectory = root;
        }

        public static string GetCurrentDirectoryPathFromExecutingAssembly()
        {
            string exeLocation = Assembly.GetExecutingAssembly().Location;
            return Directory.GetParent(exeLocation).FullName;
        }
    }
}