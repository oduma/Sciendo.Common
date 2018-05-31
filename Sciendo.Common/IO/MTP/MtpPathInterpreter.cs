using System;
using System.IO;

namespace Sciendo.Common.IO.MTP
{
    internal static class MtpPathInterpreter
    {
        public static bool IsMtpDevice(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;
            return path.Substring(1, 2) != @":\";
        }

        public static string GetMtpDeviceName(string mtpPath)
        {
            if(string.IsNullOrEmpty(mtpPath))
                throw new ArgumentNullException(nameof(mtpPath));
            return mtpPath.Split(new[] {Path.DirectorySeparatorChar}, StringSplitOptions.RemoveEmptyEntries)[0];
        }
    }
}