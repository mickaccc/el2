using System;
using System.IO;
using System.Threading.Tasks;

namespace El2Core.Utils
{
    public class AdvanceFileOperations
    {

        /// <summary>
        /// Asynchronously moves a file by copying it and then deleting the source.
        /// </summary>
        public static async Task MoveFileAsync(string sourcePath, string destinationPath, bool overwrite = false)
        {
            // Validate parameters
            if (string.IsNullOrWhiteSpace(sourcePath))
                throw new ArgumentException("Source path cannot be null or empty.", nameof(sourcePath));
            if (string.IsNullOrWhiteSpace(destinationPath))
                throw new ArgumentException("Destination path cannot be null or empty.", nameof(destinationPath));

            if (!File.Exists(sourcePath))
                throw new FileNotFoundException("Source file not found.", sourcePath);

            // Ensure destination directory exists
            Directory.CreateDirectory(Path.GetDirectoryName(destinationPath)!);

            // If overwrite is false and file exists, throw
            if (!overwrite && File.Exists(destinationPath))
                throw new IOException($"Destination file '{destinationPath}' already exists.");

            // Copy asynchronously
            const int bufferSize = 81920; // 80 KB buffer
            using (FileStream sourceStream = new FileStream(sourcePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, useAsync: true))
            using (FileStream destinationStream = new FileStream(destinationPath, overwrite ? FileMode.Create : FileMode.CreateNew, FileAccess.Write, FileShare.None, bufferSize, useAsync: true))
            {
                await sourceStream.CopyToAsync(destinationStream);
            }

            // Delete source file after successful copy
            File.Delete(sourcePath);
        }
    }

}

