using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace El2Core.Utils
{

    public static class Archivator
    {
        
        private static bool _isChanged = false;
        public static bool IsChanged
        {
            get { return _isChanged; }
            set { _isChanged = value; }
        }
        private static List<ArchivatorRule> _ArchivRules = [];
        public static List<ArchivatorRule> ArchiveRules
        {
            get { return _ArchivRules; }
            set { _ArchivRules = value; _isChanged = true; }
        }
        private static string[]? _FileExtensions;
        public static string[]? FileExtensions
        {
            get { return _FileExtensions; }
            set
            {
                _isChanged = true;
                _FileExtensions = value;
            }
        }
        private static int _DelayDays =0;
        public static int DelayDays
        { 
            get { return _DelayDays; }
            set { _isChanged = true; _DelayDays = value; }
        }

        public static ArchivState Archivate(string SourceLocation, int rule, out string Location, out int MovedFiles)
        {
            
            ArchivState state = 0;
            Location = string.Empty;
            MovedFiles = 0;
            if (rule < 0 || rule >= ArchiveRules.Count) return ArchivState.NoRule;
            List<FileInfo> files = [];
            var dir = new DirectoryInfo(SourceLocation);
            if (dir.Exists == false) return ArchivState.NoDirectory;
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
            Location = ArchiveRules[rule].TargetPath;
            if (files.Count > 0)
            {
                arch = Directory.CreateDirectory(Path.Combine(Location, dir.Name));

                _ = MoveFilesAsync([.. files], arch.FullName, 0);
            }
            foreach (var d in dir.GetDirectories())
            {
                int repeatNo = 0;
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
                    if (arch == null || !arch.Exists)
                    {
                        arch = Directory.CreateDirectory(Path.Combine(Location, dir.Name));
                    }

                    var subArch = Directory.CreateDirectory(Path.Combine(arch.FullName, d.Name));
                    ValueTuple<int, string> result = new (0, string.Empty);
                    do
                    {
                       result = MoveFilesAsync([.. files], subArch.FullName, repeatNo).Result;
                       MovedFiles += result.Item1;
                        if (result.Item2 != null)
                        {
                            var f = new DirectoryInfo(result.Item2);
                            files.Clear();
                            if (FileExtensions == null)
                            {
                                files.AddRange([.. f.GetFiles()]);
                            }
                            else
                            {
                                foreach (var ext in FileExtensions)
                                {
                                    files.AddRange(f.GetFiles($"*{ext}"));
                                }
                            }
                            repeatNo++;
                        }
                    } while (result.Item2 != string.Empty);
                }
            }
            if (MovedFiles == 0)
            {
                state = ArchivState.NoFiles;
            }
            else
            {
                state = ArchivState.Archivated;
            }
            return state;
        }
        private static void MoveFiles(FileInfo[] source, string target, ref ArchivState state)
        {
            foreach (var file in source)
            {
                try
                {
                    file.MoveTo(Path.Combine(target, file.Name));
                    state = ArchivState.Archivated;
                }
                catch (Exception ex)
                {
                    state = 0;
                }
            }
        }
        private static async Task<ValueTuple<int, string>> MoveFilesAsync(FileInfo[] source, string target, int repeatNr)
        {
            int result = 0;
            int dirCount = 0;
            string path = string.Empty;
            await Task.Run(() =>
            {

                foreach (var file in source)
                {
  
                    try
                    {
                        AdvanceFileOperations.MoveFileAsync(file.FullName, Path.Combine(target, file.Name)).Wait();
                        result++;
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                    dirCount = file.Directory != null ? file.Directory.GetDirectories().Length : 0;
                    if (dirCount > 0) path = file.Directory.GetDirectories()[repeatNr].FullName;
                }            
            });
            return new (result, path);
        }

        public enum ArchivatorTarget
        {
            TTNR,
            OrderNumber
        }
        public enum ArchivState
        {
            None,
            Archivated,
            NoFiles,
            NoDirectory,
            NoRule
        }
    }
    public class ArchivatorRule
    {
        public string? Name { get; set; }
        public string? RegexString { get; set; }
        public Archivator.ArchivatorTarget MatchTarget { get; set; } = Archivator.ArchivatorTarget.TTNR;
        public string? TargetPath { get; set; }
        public ArchivatorRule(string name, string regex, Archivator.ArchivatorTarget target, string targetPath)
        {
            Name = name;
            RegexString = regex;
            MatchTarget = target;
            TargetPath  = targetPath;
  
        }
        public ArchivatorRule() { }
    }
    

    
}
