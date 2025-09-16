using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace El2Core.Utils
{

    public static class Archivator
    {
        private const string DefaultArchivLocation = @"C:\Lieferliste\Archiv";

        public static string ArchivLocation { get; set; } = DefaultArchivLocation;
        public static string[] FileExtensions { get; set; }
        public static int DelayDays { get; set; } = 0;

        public static int Archivate(string SourceLocation)
        {
            int state = 0;
            
            List<FileInfo> files = [];
            var dir = new DirectoryInfo(SourceLocation);
            if (dir.Exists == false) return 3;
            if (FileExtensions == null)
            {
                files.AddRange([.. dir.GetFiles()]);
            }
            else
            {
                foreach (var ext in FileExtensions)
                {
                    files.AddRange(dir.GetFiles($"*{ext}"));  
                }
            }
            DirectoryInfo? arch = null;
            if (files.Count > 0)
            {
                arch = Directory.CreateDirectory(Path.Combine(ArchivLocation, dir.Name));

                MoveFiles([.. files], arch.FullName, ref state);
            }
            foreach (var d in dir.GetDirectories())
            {

                
                files.Clear();
                if (FileExtensions == null)
                {
                    files.AddRange([.. d.GetFiles()]);
                }
                else
                {
                    foreach (var ext in FileExtensions)
                    {
                        files.AddRange(d.GetFiles($"*{ext}"));
                    }
                }
                if (files.Count > 0)
                {
                    if (arch == null)
                    {
                        arch = Directory.CreateDirectory(Path.Combine(ArchivLocation, dir.Name));
                    }
                    if (!arch.Exists)
                    {
                        Directory.CreateDirectory(Path.Combine(ArchivLocation, dir.Name));
                    }
                    var subArch = Directory.CreateDirectory(Path.Combine(arch.FullName, d.Name));
                    MoveFiles([.. files], subArch.FullName, ref state);
                }
            }
            state = (state == 0) ? 2 : state;
            return state;
        }
        private static void MoveFiles(FileInfo[] source, string target, ref int state)
        {
            foreach (var file in source)
            {
                try
                {
                    file.MoveTo(Path.Combine(target, file.Name));
                    state = 1;
                }
                catch (Exception ex)
                {
                    state = 0;
                    
                }
            }
        }
    }
}
