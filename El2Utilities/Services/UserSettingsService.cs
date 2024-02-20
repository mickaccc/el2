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
            if(Properties.Settings.Default.UpgradeFlag == true)
            {
                try
                {

                    Properties.Settings.Default.Upgrade();
                    Properties.Settings.Default.Reload();
   
                    UpgradeFlag = false;
                    Save();
                }
                catch (System.Exception e)
                {

                    MessageBox.Show(string.Format("{0}\n{1}", e.Message, e.InnerException), "Upgrade", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
        }
    }
}
