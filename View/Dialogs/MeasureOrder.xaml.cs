using System;
using System.Linq;
using System.Windows;
using Lieferliste_WPF.ViewModels.Support;
using Lieferliste_WPF.Utilities;

namespace Lieferliste_WPF.View.Dialogs
{
    /// <summary>
    /// Interaction logic for MeasureOrder.xaml
    /// </summary>
    public partial class MeasureOrder : Window
    {
        public MeasureOrder(String VID)
        {
            InitializeComponent();

            this.MainGrid.DataContext = DbManager.GetInstance().GetVorgangSelect(VID);
            cmbOrderer.ItemsSource = DbManager.GetInstance().getUsers();
            cmbRess.ItemsSource = DbManager.GetInstance().getResources(); //.Where(x => x.Abteilung == "COS").OrderBy(x => x.RessName);

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
