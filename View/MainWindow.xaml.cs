namespace Lieferliste_WPF.View
{
    using Lieferliste_WPF.ViewModels;
    using Lieferliste_WPF.Utilities;
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using Lieferliste_WPF;
    using Lieferliste_WPF.UserControls;
    using System.Windows.Media;
    using System.Windows.Input;
    using Lieferliste_WPF.Commands;
    using Lieferliste_WPF.View.Dialogs;
    using System.CodeDom;
    using System.Diagnostics;


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainWindowViewModel viewModel;
        public MainWindow()
        {
            InitializeComponent();
            viewModel = new();
            DataContext = viewModel;
            TbControl.SelectedIndex = 0;

        }
        void HandleMainWindowLoaded(object sender, RoutedEventArgs e)
        {
            CommandBindings.Add(new CommandBinding(
                ELCommands.ShowUserMgmt, HandleMenageUserExecuted,
                HandelMenageUserCanExecute));
            CommandBindings.Add(new CommandBinding(
                ELCommands.ShowMachinePlan, HandleMachinePlanExecuted, HandleMachinePlanCanExecute));

        }
        #region Events
        private void HandleMachinePlanCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = PermissionsProvider.GetInstance().GetUserPermission("MP00");
        }

        private void HandleMachinePlanExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            bool mp = false;
            Window? window = null;
            
            foreach (Window wnd in this.OwnedWindows)
            {
                if (wnd.Name=="tabable")
                {
                    window = (wnd.Tag.ToString() == "MPL") ? wnd : null;                  
                }
            }
            if (window == null)
            {
                foreach (TabItem item in TbControl.Items)
                {
                    if (item.Name == "MPL") { TbControl.Items.MoveCurrentTo(item); mp = true; break; }
                }
                
            }
            else  window?.Activate();
            
            if(window == null && !mp)
            {
                Frame frame = new()
                {
                    Source = new Uri("/View/MachinePlan.xaml", UriKind.Relative),
                    Name="MPL"
                    
                };
                TabItem tabItem = new()
                {
                    Name="MPL",
                    Header = "Maschinen Zuteilung",
                    Content = frame,
                    IsSelected = true                   
                };
                TbControl.Items.Add(tabItem);

            }
        }
        
        private void HandelMenageUserCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = PermissionsProvider.GetInstance().GetUserPermission("UM00");
        }

        private void HandleMenageUserExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            UserEdit usr = new UserEdit();
            usr.Owner = this;
            usr.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            usr.Show();
        }
        
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

        private void TabItem_Drag(object sender, MouseEventArgs e)
        {


            if (e.Source is TabItem tabItem)
            {
                //if (Mouse.PrimaryDevice.LeftButton == MouseButtonState.Pressed)
                    //DragDrop.DoDragDrop(tabItem, tabItem, DragDropEffects.Move);
            }
        }

        private void TabItem_Drop(object sender, DragEventArgs e)
        {
            var tabItemTarget = GetTargetTabItem(e.OriginalSource);
            var tabItemSource = (TabItem)e.Data.GetData(typeof(TabItem));
            if (tabItemTarget != null)
            {

                if (tabItemTarget != tabItemSource)
                {
                    TabControl tabCrtTarget = (TabControl)tabItemTarget.Parent;
                    TabControl tabCrtSource = (TabControl)tabItemSource.Parent;
                    if (tabCrtTarget.Equals(tabCrtSource))
                    {
                        int targetIndex = tabCrtTarget.Items.IndexOf(tabItemTarget);

                        tabCrtSource.Items.Remove(tabItemSource);
                        tabCrtTarget.Items.Insert(targetIndex, tabItemSource);
                        tabItemSource.IsSelected = true;
                    }
                }
            }
        }
        private void TabControl_Leave(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Move;

            var tabItemSource = (TabItem)e.Data.GetData(typeof(TabItem));
            TabControl tabCrt = (TabControl)sender;
            Window wnd = new Tabable
            {
                Owner = this,
                Title = tabItemSource.Header.ToString(),
                Content = tabItemSource.Content,
                Tag = "MPL"

            };
            MainWindowViewModel mv = DataContext as MainWindowViewModel;
            mv.WindowTitles.Add((Page)tabItemSource.Content);
            tabCrt.Items.Remove(tabItemSource);

            wnd.Show();

            this.Background = Brushes.White;
        }
        private void TabControl_Enter(object sender, DragEventArgs e)
        {
            this.Background = Brushes.Turquoise;

        }
        #endregion

        #region ToArchiveCommand
        #endregion ToArchiveCommand

        private void Perspective_Click(object sender, RoutedEventArgs e)
        {
            Button chk = sender as Button;
            (this.DataContext as ToolCase).ChangeActivePerpective((String)chk.Content);
        }


        private TabItem GetTargetTabItem(object originalSource)
        {
            var current = originalSource as DependencyObject;

            while (current != null)
            {
                var tabItem = current as TabItem;
                if (tabItem != null)
                {
                    return tabItem;
                }

                current = VisualTreeHelper.GetParent(current);
            }

            return null;
        }


    }
}
