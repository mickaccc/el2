namespace Lieferliste_WPF.View
{
  using System.IO;
  using System.Windows;
  using System.Windows.Input;
  using AvalonDock.Layout.Serialization;
    using Lieferliste_WPF.Commands;
    using Lieferliste_WPF.ViewModels;
    using Lieferliste_WPF.Entities;
    using System.Windows.Controls;
    using Lieferliste_WPF.ViewModels.Base;
    using System;
    using System.Data.SqlClient;
    using System.Data.Entity.Core.EntityClient;
    using System.Data.Entity;

  /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = ToolCase.This;

            ToolCase.This.InitCommandBindings(this);
        }
        #region Events
        private void About_Click(object sender, RoutedEventArgs e)
        {
            WPFAboutBox about = new WPFAboutBox(this);
            about.ShowDialog();
        }

        protected override void  OnClosing(System.ComponentModel.CancelEventArgs e)
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

        #region LoadLayoutCommand
        RelayCommand _loadLayoutCommand = null;
        public ICommand LoadLayoutCommand
        {
            get
            {
                if (_loadLayoutCommand == null)
                {
                    _loadLayoutCommand = new RelayCommand((p) => OnLoadLayout(p), (p) => CanLoadLayout(p));
                }

                return _loadLayoutCommand;
            }
        }

        private bool CanLoadLayout(object parameter)
        {
            return File.Exists(@".\AvalonDock.Layout.config");
        }

        private void OnLoadLayout(object parameter)
        {
            var layoutSerializer = new XmlLayoutSerializer(dockManager);
            //Here I've implemented the LayoutSerializationCallback just to show
            // a way to feed layout desarialization with content loaded at runtime
            //Actually I could in this case let AvalonDock to attach the contents
            //from current layout using the content ids
            //LayoutSerializationCallback should anyway be handled to attach contents
            //not currently loaded
            layoutSerializer.LayoutSerializationCallback += (s, e) =>
                {
                    //if (e.Model.ContentId == FileStatsViewModel.ToolContentId)
                    //    e.Content = Workspace.This.FileStats;
                    //else if (!string.IsNullOrWhiteSpace(e.Model.ContentId) &&
                    //    File.Exists(e.Model.ContentId))
                    //    e.Content = Workspace.This.Open(e.Model.ContentId);
                };
            layoutSerializer.Deserialize(@".\AvalonDock.Layout.config");
        }

        #endregion 

        #region SaveLayoutCommand
        RelayCommand _saveLayoutCommand = null;
        public ICommand SaveLayoutCommand
        {
            get
            {
                if (_saveLayoutCommand == null)
                {
                    _saveLayoutCommand = new RelayCommand((p) => OnSaveLayout(p), (p) => CanSaveLayout(p));
                }

                return _saveLayoutCommand;
            }
        }

        private bool CanSaveLayout(object parameter)
        {
            return true;
        }

        private void OnSaveLayout(object parameter)
        {
            var layoutSerializer = new XmlLayoutSerializer(dockManager);
            layoutSerializer.Serialize(@".\AvalonDock.Layout.config");
        }

        #endregion 

        #region ToArchiveCommand
        #endregion ToArchiveCommand

        private void OnDumpToConsole(object sender, RoutedEventArgs e)
        {
#if DEBUG
            dockManager.Layout.ConsoleDump(0);
#endif
        }

        private void txtSearch_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            TextBox t = e.Source as TextBox;

                (this.DataContext as ToolCase).ActivePerspective.LeaderPanes[0].addFilterCriteria(this.cmbFields.Text,this.txtSearch.Text);
        }



        private void Perspective_Click(object sender, RoutedEventArgs e)
        {
            Button chk = sender as Button;
            (this.DataContext as ToolCase).ChangeActivePerpective((String)chk.Content);
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            Window setting = new Settings();
            setting.ShowDialog();
        }




    }
}
