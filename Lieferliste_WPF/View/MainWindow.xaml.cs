﻿namespace Lieferliste_WPF.View
{
    using Lieferliste_WPF;
    using Lieferliste_WPF.ViewModels;
    using System;
    using System.Runtime.Versioning;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;



    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [SupportedOSPlatform("windows7.0")]
        public MainWindow()
        {
            InitializeComponent();
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



        private static TabItem GetTargetTabItem(object originalSource)
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

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Window w = new Window1();
            w.Show();
        }
    }
}