using System;
using System.IO;
using Beefeater;

namespace csmacnz.Coveralls
{
    public class FileSystem : IFileSystem
    {
        public Option<string> TryLoadFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                return File.ReadAllText(filePath);
            }
            return null;
        }

        public bool WriteFile(string outputFile, string fileData)
        {
            try
            {
                File.WriteAllText(outputFile, fileData);
            }
            catch (Exception)
            {
                //Maybe should give reason.
                return false;
            }
            return true;
        }
    }
}