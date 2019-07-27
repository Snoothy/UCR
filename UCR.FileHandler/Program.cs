using System;
using System.IO;
using HidWizards.UCR.Utilities;

namespace UCR.FileHandler
{
    class Program
    {
        static int Main(string[] args)
        {
            String directory;

            try
            {
                directory = args[0];
            }
            catch (IndexOutOfRangeException e)
            {
                directory = Directory.GetCurrentDirectory();
            }

            var success = FileUnblockManager.UnblockAllProgramFiles(directory);

            if (!success) return -1;

            Console.WriteLine("Successfully unblocked all files in " + directory);
            Console.WriteLine("Press any key to close...");

            Console.ReadKey();

            return 0;
        }
    }
}
