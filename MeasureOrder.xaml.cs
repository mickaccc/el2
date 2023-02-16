using System;
using System.Linq;
using System.Windows;

namespace Lieferliste_WPF.Dialogs
{
    /// <summary>
    /// Interaction logic for MeasureOrder.xaml
    /// </summary>
    public partial class MeasureOrder : Window
    {
        public MeasureOrder(String VID)
        {
            InitializeComponent();

            this.MainGrid.DataContext = DbManager.Instance().getVorgSelect(VID);
            cmbOrderer.ItemsSource= DbManager.Instance().getUsers();
            cmbRess.ItemsSource= DbManager.Instance().getResources().Where(x => x.Abteilung=="COS").OrderBy(x => x.RessName);
            
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (this.cmbOrderer.SelectedValue != null &&
                this.cmbRess.SelectedItem != null) { } 
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
