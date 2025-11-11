using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace El2Core.Utils
{
    public static class AdvanceFileOperations
    {

        /// <summary>
        /// Asynchronously moves a file by copying it and then deleting the source.
        /// </summary>
        public static async Task MoveFileAsyncStream(string sourcePath, string destinationPath, bool overwrite = false, CancellationToken cancellationToken = default)
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
            var bufferSize = 4096; // 4 KB buffer
            var fileOptions = FileOptions.Asynchronous | FileOptions.SequentialScan;
            using (FileStream sourceStream = new FileStream(sourcePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize,fileOptions))
            using (FileStream destinationStream = new FileStream(destinationPath, overwrite ? FileMode.Create : FileMode.CreateNew, FileAccess.Write, FileShare.None, bufferSize, fileOptions))
            {
                await sourceStream.CopyToAsync(destinationStream, bufferSize, cancellationToken).ConfigureAwait(false);
            }

            // Delete source file after successful copy
            File.Delete(sourcePath);
        }
        public static async Task CopyFileAsyncStream(string sourceFile, string destinationFile, CancellationToken cancellationToken = default)
        {
            var fileOptions = FileOptions.Asynchronous | FileOptions.SequentialScan;
            var bufferSize = 4096;

            using (var sourceStream =
                  new FileStream(sourceFile, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, fileOptions))

            using (var destinationStream =
                  new FileStream(destinationFile, FileMode.CreateNew, FileAccess.Write, FileShare.None, bufferSize, fileOptions))

                await sourceStream.CopyToAsync(destinationStream, bufferSize, cancellationToken)
                                  .ConfigureAwait(false);
        }
        public static async Task MoveFileAsync(string sourcePath, string destinationPath)
        { 
            var Fileinfo = new FileInfo(destinationPath);
            Directory.CreateDirectory(Fileinfo.Directory!.FullName);
            await Task.Run(() => File.Move(sourcePath, destinationPath, true));
        }
    }

}

