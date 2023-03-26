namespace Lieferliste_WPF.View
{
    using Lieferliste_WPF.ViewModels;
    using Lieferliste_WPF.Utilities;
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using Lieferliste_WPF;
    using Lieferliste_WPF.UserControls;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //this.DataContext = ToolCase.This;
           // order.ItemsSource = OrderViewModel.orders;

            //ToolCase.This.InitCommandBindings(this);
        }

        #region Events
        private void About_Click(object sender, RoutedEventArgs e)
        {
            WPFAboutBox about = new WPFAboutBox(this);
            about.ShowDialog();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            //using (var db = new DB_COS_LIEFERLISTE_SQLEntities())
            //{
            //    DeliveryListViewModel persp = null;
            //    foreach (var l in (this.DataContext as ToolCase).LeaderPanes)
            //    {
            //        if (l.GetType() == (typeof(DeliveryListViewModel)))
            //            persp = l as DeliveryListViewModel;

            //        //if (persp != null)
            //        //{


            //        //    foreach (var lie in db.lieferlistes)
            //        //    {
            //        //        lie.Bem_M = persp.Processes.Find(y => y.VID == lie.VID).Bem_M;
            //        //        lie.Bem_MA = persp.Processes.Find(y => y.VID == lie.VID).Bem_MA;
            //        //        lie.Bem_T = persp.Processes.Find(y => y.VID == lie.VID).Bem_T;
            //        //        lie.Dringend = persp.Processes.Find(y => y.VID == lie.VID).Dringend;
            //        //        lie.Bemerkung = persp.Processes.Find(y => y.VID == lie.VID).Bemerkung;
            //        //        lie.Mappe = persp.Processes.Find(y => y.VID == lie.VID).Mappe;
            //        //    }
            //        //}

            //    }
            //    db.SaveChanges();
            //}
            base.OnClosing(e);
        }
        #endregion




        #region ToArchiveCommand
        #endregion ToArchiveCommand

        private void OnDumpToConsole(object sender, RoutedEventArgs e)
        {

        }

        private void txtSearch_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            TextBox t = e.Source as TextBox;

            //(this.DataContext as ToolCase).ActivePerspective.LeaderPanes[0].addFilterCriteria(this.cmbFields.Text, this.txtSearch.Text);
        }



        private void Perspective_Click(object sender, RoutedEventArgs e)
        {
            Button chk = sender as Button;
            (this.DataContext as ToolCase).ChangeActivePerpective((String)chk.Content);
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            Window setting = new View.Settings();
            setting.ShowDialog();
        }




    }
}
