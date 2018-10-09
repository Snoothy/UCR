using System;
using System.Runtime.InteropServices;
using HidWizards.UCR.Utilities;

namespace UCR.FileHandler
{
    class Program
    {
        static int Main(string[] args)
        {
            var success = FileUnblockManager.UnblockAllProgramFiles(args[0]);
            if (!success) return -1;

            return 0;
        }
    }
}
