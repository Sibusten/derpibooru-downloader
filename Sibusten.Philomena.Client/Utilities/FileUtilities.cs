using System;
using System.IO;
using System.Threading.Tasks;

namespace Sibusten.Philomena.Client.Utilities
{
    public static class FileUtilities
    {
        public static void CreateDirectoryForFile(string file)
        {
            string? fileDir = Path.GetDirectoryName(file);
            if (fileDir is null)
            {
                throw new DirectoryNotFoundException($"The file does not have a parent directory: {file}");
            }
            Directory.CreateDirectory(fileDir);
        }

        public static async Task SafeFileWrite(string file, Func<string, Task> writeToTempFile)
        {
            // Write to a temp file first
            string tempFile = $"{file}.tmp";
            await writeToTempFile(tempFile);

            // Move the temp file to the destination file
            File.Move(tempFile, file, overwrite: true);
        }
    }
}
