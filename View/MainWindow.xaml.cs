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
    using GongSolutions.Wpf.DragDrop.Utilities;
    using Gat.Controls.Framework;


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

        #region Events
           
        private void About_Click(object sender, RoutedEventArgs e)
        {
            WPFAboutBox about = new WPFAboutBox(this);
            about.ShowDialog();
        }



        private void TabItem_Drag(object sender, MouseEventArgs e)
        {


            if (e.Source is TabItem tabItem)
            {
                if (Mouse.PrimaryDevice.LeftButton == MouseButtonState.Pressed)
                    DragDrop.DoDragDrop(tabItem, tabItem, DragDropEffects.Move);
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
            if (tabItemSource != null)
            {
                TabControl tabCrt = (TabControl)sender;
                Window wnd = new Tabable
                {
                    Owner = this,
                    Title = tabItemSource.Header.ToString(),
                    Content = tabItemSource.Content,
                    Tag = "MPL"

                };
                
                MainWindowViewModel mv = DataContext as MainWindowViewModel;
                mv.WindowTitles.Add(tabItemSource);
                mv.TabTitles.Remove(tabItemSource);

                wnd.Show();

                this.Background = Brushes.White;
            }
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

        private void TbControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.MiddleButton == MouseButtonState.Pressed)
                {
                    TabControl tabCrt = (TabControl)sender;
                    TabItem tabItemSource = (TabItem)tabCrt.SelectedItem;
                    Window wnd = new Tabable
                    {
                        Owner = this,
                        Title = (string)tabItemSource.Header

                    };

                    (wnd as Tabable).Tabable_TabControl.Items.Add(tabItemSource);
                    MainWindowViewModel mv = DataContext as MainWindowViewModel;
                    mv.TabTitles.Remove(tabItemSource);
                    mv.WindowTitles.Add(tabItemSource);

                    wnd.Show();
                }
            }
            
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message,"Fehlermeldung",MessageBoxButton.OK,MessageBoxImage.Error);
            }
        }

        private void mainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MainWindowViewModel vm = (MainWindowViewModel)DataContext;
            if (vm != null)
            {
                if (vm.CheckChanges())
                {
                    MessageBoxResult r = MessageBox.Show("Sollen die Änderungen noch in\n die Datenbank gespeichert werden?",
                        "MS SQL Datenbank", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes);
                    if (r == MessageBoxResult.Yes) vm.SaveChanges();
                }
            }
        }
    }
}
