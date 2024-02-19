using System.Configuration;
using System.Windows;

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
        void Upgrade();
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

        public string Theme
        {
            get { return Properties.Settings.Default.Theme; }
            set { Properties.Settings.Default[nameof(Theme)] = value; }
        }
        public bool UpgradeFlag
        {
            get { return Properties.Settings.Default.UpgradeFlag; }
            set { Properties.Settings.Default[nameof(UpgradeFlag)] = value; }
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

        public void Upgrade()
        {
            if(Properties.Settings.Default.UpgradeFlag == false)
            {
                try
                {
                    var s = new SettingsContext();
                    var c = this.MemberwiseClone();
                    
                    Properties.Settings.Default.Upgrade();

                    var autosave = Properties.Settings.Default.GetPreviousVersion("IsAutoSave");
                    if (autosave != null) { IsAutoSave = (bool)autosave; }
                    var savemsg = Properties.Settings.Default.GetPreviousVersion("IsSaveMessage");
                    if (savemsg != null) { IsSaveMessage = (bool)savemsg; }
                    var theme = Properties.Settings.Default.GetPreviousVersion("Theme");
                    if (theme != null) { Theme = (string)theme; }
                    UpgradeFlag = false;
                    Save();
                }
                catch (System.Exception e)
                {

                    MessageBox.Show(string.Format("{0}\n{1}", e.Message, e.InnerException), "Upgrade", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
            //var a = conf.AppSettings;
            //var s = conf.Sections;
            //var sg = conf.SectionGroups;
            //var ug = conf.GetSectionGroup("userSettings");

            //UpgradeFlag = false;
            //Properties.Settings.Default.Save();  
        }
    }
}
