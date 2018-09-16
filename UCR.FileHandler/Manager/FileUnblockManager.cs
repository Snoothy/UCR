using System;
using System.IO;
using System.Security.Principal;
using Trinet.Core.IO.Ntfs;

namespace HidWizards.UCR.Utilities
{
    public static class FileUnblockManager
    {

        public static bool IsAdmin()
        {
            return new WindowsPrincipal
                (WindowsIdentity.GetCurrent()).IsInRole
                (WindowsBuiltInRole.Administrator);
        }

        public static bool UnblockAllProgramFiles(string directory)
        {
            var success = true;
            foreach (var file in Directory.GetFiles(directory, "*", SearchOption.AllDirectories))
            {
                success &= UnblockFile(file);
            }

            return success;
        }

        private static bool UnblockFile(string path)
        {
            var file = new FileInfo(path);
            if (!file.AlternateDataStreamExists("Zone.Identifier")) return true;
            
            return file.DeleteAlternateDataStream("Zone.Identifier");
        }
    }
}
