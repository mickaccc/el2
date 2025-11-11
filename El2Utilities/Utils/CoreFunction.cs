using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace El2Core.Utils
{
    public class CoreFunction
    {
        public static Process PriorProcess()
        // Returns a System.Diagnostics.Process pointing to
        // a pre-existing process with the same name as the
        // current one, if any; or null if the current process
        // is unique.
        {
            Process curr = Process.GetCurrentProcess();
            Process[] procs = Process.GetProcessesByName(curr.ProcessName);
            foreach (Process p in procs)
            {
                if ((p.Id != curr.Id) &&
                    (p.MainModule.FileName == curr.MainModule.FileName))
                    return p;
            }
            return null;
        }

        /// </summary>
        /// <param name="path">Directory path to delete.</param>
        /// <param name="recursive">Whether to delete subdirectories and files.</param>
        /// <param name="timeoutMs">Maximum wait time in milliseconds.</param>
        /// <param name="retryDelayMs">Delay between retries in milliseconds.</param>
        public static void DeleteDirectoryWithWait(string path, bool recursive = true, int timeoutMs = 10000, int retryDelayMs = 500)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Path cannot be null or empty.", nameof(path));

            if (!Directory.Exists(path))
                return; // Nothing to delete

            var startTime = DateTime.UtcNow;

            while (true)
            {
                try
                {
                    Directory.Delete(path, recursive);
                    return; // Success
                }
                catch (IOException)
                {
                    // File or directory is in use
                }
                catch (UnauthorizedAccessException)
                {
                    // Could be due to file locks or permissions
                }

                if ((DateTime.UtcNow - startTime).TotalMilliseconds > timeoutMs)
                    throw new TimeoutException($"Could not delete directory '{path}' within {timeoutMs} ms.");

                Thread.Sleep(retryDelayMs);
            }
        }
    }
}
