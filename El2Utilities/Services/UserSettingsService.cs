using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace El2Core.Services
{
    public interface IUserSettingsService
    {
        string ExplorerPath { get; set; }
        string ExplorerRoot { get; set; }
        bool IsAutoSave { get; set; }
        bool IsSaveMessage { get; set; }
        void Save();
        void Reset();
        void Reload();
    }
    public class UserSettingsService : IUserSettingsService
    {
        public string ExplorerPath
        {
            get { return Properties.Settings.Default.ExplorerPath; }
            set { Properties.Settings.Default[nameof(ExplorerPath)] = value; }
        }
        public string ExplorerRoot
        {
            get { return Properties.Settings.Default.ExplorerRoot; }
            set { Properties.Settings.Default[nameof(ExplorerRoot)] = value; }
        }
        public bool IsAutoSave
        {
            get { return Properties.Settings.Default.IsAutoSave; }
            set { Properties.Settings.Default[nameof(IsAutoSave)] = value; }
        }
        public bool IsSaveMessage
        {
            get { return Properties.Settings.Default.IsSaveMessage; }
            set { Properties.Settings.Default[nameof(IsSaveMessage)] = value; }
        }
        public void Save()
        {
            Properties.Settings.Default.Save();
        }
        public void Reset()
        {
            Properties.Settings.Default.Reset();
        }

        public void Reload()
        {
            Properties.Settings.Default.Reload();
        }
    }
}
