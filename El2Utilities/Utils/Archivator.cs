using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Automation;

namespace El2Core.Utils
{

    public static class Archivator
    {

        public static bool IsChanged { get; set; } = false;
        public static List<ArchivatorRule> ArchiveRules { get; set { IsChanged = true; } } = [];

        public static string[]? FileExtensions { get; set { IsChanged = true; } }
        public static int DelayDays { get; set { IsChanged = true; } } = 0;

        public static int Archivate(string SourceLocation, int rule, out string Location)
        {
            int state = 0;
            Location = string.Empty;
            if (rule < 0 || rule >= ArchiveRules.Count) return 3;
            List<FileInfo> files = [];
            var dir = new DirectoryInfo(SourceLocation);
            if (dir.Exists == false) return 3;
            if (ArchiveRules[rule].FileExt == null)
            {
                files.AddRange([.. dir.GetFiles()]);
            }
            else
            {
                foreach (var ext in ArchiveRules[rule].FileExt)
                {
                    files.AddRange(dir.GetFiles($"*{ext}"));
                }
            }
            DirectoryInfo? arch = null;
            Location = ArchiveRules[rule].TargetPath;
            if (files.Count > 0)
            {
                arch = Directory.CreateDirectory(Path.Combine(Location, dir.Name));

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
                    foreach (var ext in ArchiveRules[rule].FileExt)
                    {
                        files.AddRange(d.GetFiles($"*{ext}"));
                    }
                }
                if (files.Count > 0)
                {
                    if (arch == null)
                    {
                        arch = Directory.CreateDirectory(Path.Combine(Location, dir.Name));
                    }
                    if (!arch.Exists)
                    {
                        Directory.CreateDirectory(Path.Combine(Location, dir.Name));
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
        public enum ArchivatorTarget
        {
            TTNR,
            OrderNumber
        }
    }
    public class ArchivatorRule
    {
        public string[]? FileExt { get; set; }
        public string? RegexString { get; set; }
        public Archivator.ArchivatorTarget MatchTarget { get; set; } = Archivator.ArchivatorTarget.TTNR;
        public string? TargetPath { get; set; }
        public ArchivatorRule(string regex, Archivator.ArchivatorTarget target, string targetPath, string extension)
        {
            FileExt = extension.Split(',');
            RegexString = regex;
            MatchTarget = target;
            TargetPath  = targetPath;
        }
        public ArchivatorRule() { }
    }
    

    
}
