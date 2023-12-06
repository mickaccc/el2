namespace Lieferliste_WPF.Views
{
    using El2Core.Converters;
    using SharpCompress.Common;
    using System;
    using System.Globalization;
    using System.Runtime.Versioning;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Threading;


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        [SupportedOSPlatform("windows7.0")]
        public MainWindow()
        {
            InitializeComponent();
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

        private void TabControl_Enter(object sender, DragEventArgs e)
        {
            this.Background = Brushes.Turquoise;
        }
        #endregion


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


        private void mainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            DispatcherTimer timer = new DispatcherTimer(TimeSpan.FromSeconds(1), DispatcherPriority.Normal, (object s, EventArgs ev) =>
            {
                this.myDateTime.Text = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss");
            }, this.Dispatcher);
            timer.Start();
            this.aktKW.Text = string.Format("  KW {0}  ", CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(DateTime.Now,
                CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday));
    
        }
    }
}
