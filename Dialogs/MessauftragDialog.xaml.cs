using System;
using System.Windows;


namespace Lieferliste_WPF.Dialogs
{
    /// <summary>
    /// Interaction logic for MessauftragDialog.xaml
    /// </summary>
    public partial class MessauftragDialog : Window
    {
        public MessauftragDialog(String VID)
        {
            InitializeComponent();
            this.HeaderInfo.DataContext = DbManager.Instance().getHeaderInfo(VID);
        }
    }
}
