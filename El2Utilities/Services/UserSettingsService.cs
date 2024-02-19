using System.Configuration;

namespace El2Core.Services
{
    public interface IUserSettingsService
    {
        string ExplorerPath { get; set; }
        string ExplorerRoot { get; set; }
        bool IsAutoSave { get; set; }
        bool IsSaveMessage { get; set; }
        string Theme { get; set; }
        void Save();
        void Reset();
        void Reload();
        void UpgradeIfRequired();
    }
    public class UserSettingsService : ApplicationSettingsBase, IUserSettingsService
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

        public string Theme
        {
            get { return Properties.Settings.Default.Theme; }
            set { Properties.Settings.Default[nameof(Theme)] = value; }
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

        public void UpgradeIfRequired()
        {
           
        }
    }
}
