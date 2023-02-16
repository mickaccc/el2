using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Lieferliste_WPF.Entities;
using System.Windows.Documents;
using System.ComponentModel;
using Lieferliste_WPF.ViewModels;
using System.Windows.Input;
using System.Reflection;

namespace Lieferliste_WPF.UserControls
{
    /// <summary>
    /// Interaction logic for Allocation.xaml
    /// </summary>

    public partial class Allocation : UserControl
    {
        private GridViewColumnHeader listViewSortCol = null;
        private SortAdorner listViewSortAdorner = null;
        private int draggedIndex, itemIndex;
        private MachineViewModel sourceMachine;
        public int Criteria { get; set; }

        public Allocation()
        {

            InitializeComponent();

            lstMain.AddHandler(ListView.MouseDownEvent, new RoutedEventHandler(lstMain_MouseDown), true);
            lstMain.DragEnter += lstMain_DragEnter;
            lstMain.DragLeave += lstMain_DragLeave;
            lstMain.DragOver += lstMain_DragOver;
            lstMain.Drop += lstMain_Drop;

            lstItems.Drop += lstItems_Drop;
            lstItems.DragOver += lstItems_DragOver;
            
            lstItems.AddHandler(ListView.MouseDownEvent, new RoutedEventHandler(lstItems_MouseDown),true);
            

            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(lstMain.ItemsSource);
            if(view != null)
                view.Filter = lstMainFilter;

            if(lstItems.HasItems) lstItems.SelectedIndex = 0;

        }

 

        #region Events
        private void lstMain_MouseDown(object sender, RoutedEventArgs e)
        {

            Process pro = lstMain.SelectedItem as Process;

            if (pro != null)
            {
                draggedIndex = -1;
                // Package the Data.
                DataObject data = new DataObject(typeof(Process),pro);
 
                DragDrop.DoDragDrop(lstMain, data, DragDropEffects.Move);

            }
        }

        private void lstMain_DragEnter(object sender, RoutedEventArgs e)
        {
            (e.Source as ListView).Background = Brushes.Yellow;
        }
        private void lstMain_DragLeave(object sender, RoutedEventArgs e)
        {
            (e.Source as ListView).Background = Brushes.White;
        }
        private void lstMain_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DataGrid)))
                e.Effects = DragDropEffects.Move;
            else
                e.Effects = DragDropEffects.None;
            e.Handled = true;
        }
        private void lstMain_Drop(object sender, DragEventArgs e)
        {

            DataGrid dragged = e.Data.GetData(typeof(DataGrid)) as DataGrid;
            if (dragged == null) return;
            Process pro = dragged.SelectedValue as Process;
            StackPanel st = dragged.Parent as StackPanel;

            if (dragged != null && pro != null)
            {

                //InternalMachine IMach = this.ressOrdered.MachineContainer.First(x => x.RID == (int)st.Tag) as InternalMachine;
                //if (IMach != null)
                //{
                //    IMach.Remove(pro);
                //    dragged.Items.Refresh();

                //}
                dragged.Background = Brushes.White;

            }
        }

        private void lstItems_MouseDown(object sender, RoutedEventArgs e)
        {

            ListBox l = sender as ListBox;

            ListView lv = e.Source as ListView;
            

                Process pro = lv.SelectedItem as Process;
                if (pro != null)
                {
    
                    DataObject data = new DataObject(typeof(Process), pro);
                    data.SetData("dragSource", l);
                    DragDrop.DoDragDrop(l, data, DragDropEffects.Move);
                }
            

        }
        private void ListView_MouseMove(object sender, MouseEventArgs e)
        {
            
            ListView l = sender as ListView;
            if (l != null && e.LeftButton == MouseButtonState.Pressed)
            {
                itemIndex = GetCurrentIndex(l, e.GetPosition);

                var pro = l.Items.GetItemAt(itemIndex);
                if (pro != null)
                {
                    foreach (var it in l.Items)
                    {
                        pro = it;
                    }
                    DataObject data = new DataObject(typeof(Process), pro);
                    data.SetData("dragSource", l);
                    DragDrop.DoDragDrop(l, data, DragDropEffects.Move);
                }
            }
        }

        private void lstItems_Drop(object sender, DragEventArgs e)
        {

            var dragged = e.Data.GetData(typeof(Process)) as Process;
            var source = e.Data.GetData("dragSource") as ListView;
            var target = sender as ListView;
            int index = GetCurrentIndex(target, e.GetPosition);
            var mach = target.Items.GetItemAt(index) as MachineViewModel;

            if (dragged != null)
            {
                if (draggedIndex == -1)
                {
                    //from OrderPool to machine
                    if (!mach.Processes.Contains(dragged))
                    {
                        if (mach.addProcess(dragged))
                        {
                            mach.MachineContainerViewModel.OrderPool.Remove(dragged);
                        }
                    }
                }
                else
                {
                    //from machine to machine
                    var machSource = source.DataContext as MachineViewModel;
                    if (!mach.Equals(machSource))
                    {
                        if (!mach.Processes.Contains(dragged))
                        {
                            if (mach.addProcess(dragged))
                            {
                                machSource.Processes.Remove(dragged);
                            }
                        }
                    }
                    else
                    {
                        //order Processes into machine
                        int itemIdx = GetCurrentIndex(target, e.GetPosition);
                        var targetProcess = target.Items.GetItemAt(itemIdx) as Process;
                        mach.moveProcess(dragged, targetProcess);
                        //todo: make complete

                    }
                }
            }
        }
        private void lstItems_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(Process)))
                e.Effects = DragDropEffects.Move;
            else
                e.Effects = DragDropEffects.None;
            e.Handled = true;
        }
        private void lstMain_LostFocus(object sender, RoutedEventArgs e)
        {
            ListView lv = sender as ListView;
            lv.SelectedItems.Clear();
        }


        private void ressOrder_DoubleClick(object sender, RoutedEventArgs e)
        {

            Visual frWorkEle = e.OriginalSource as Visual;

            UIElement UI = null;
            int RID = 0;
            Label label = null;
            switch (frWorkEle.GetType().Name)
            {
                //case "Label":
                //    label = frWorkEle as Label;
                //    RID = (int)label.Tag;
                //    UI = ressOrdered.nodePlannerControls[RID];
                //    break;
                case "TextBlock":
                    TextBlock txt = frWorkEle as TextBlock;
                    if (txt.Parent != null)
                    {
                        UI = new OrderView();
                    }
                    else
                    {
                        while (VisualTreeHelper.GetParent(frWorkEle).GetType() != typeof(DateUtils)) // MainWindow))
                        {
                            frWorkEle = VisualTreeHelper.GetParent(frWorkEle) as Visual;

                            if (frWorkEle != null)
                            {
                                if (frWorkEle.GetType() == typeof(Label))
                                {
                                    label = frWorkEle as Label;
                                    break;
                                }
                            }
                        }

                        if (label != null)
                        {
                            RID = (int)label.Tag;
                            //UI = ressOrdered.nodePlannerControls[RID];
                        }
                    }

                    break;
                default:
                    while (VisualTreeHelper.GetParent(frWorkEle).GetType() != typeof(DateUtils))//MainWindow))
                    {
                        frWorkEle = VisualTreeHelper.GetParent(frWorkEle) as Visual;

                        if (frWorkEle != null)
                        {
                            if (frWorkEle.GetType() == typeof(Label))
                            {
                                label = frWorkEle as Label;
                                break;
                            }
                        }
                    }

                    if (label != null)
                    {
                        RID = (int)label.Tag;
                        //UI = ressOrdered.nodePlannerControls[RID];
                    }
                    break;

            }
            if (UI != null)
            {
                //MainWindow.ViewList.Add(UI);

                
            }

        }

        private void lvAllocColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader column = (sender as GridViewColumnHeader);
            string sortBy = column.Tag.ToString();
            if (listViewSortCol != null)
            {
                AdornerLayer.GetAdornerLayer(listViewSortCol).Remove(listViewSortAdorner);
                lstMain.Items.SortDescriptions.Clear();
            }

            ListSortDirection newDir = ListSortDirection.Ascending;
            if (listViewSortCol == column && listViewSortAdorner.Direction == newDir)
                newDir = ListSortDirection.Descending;

            listViewSortCol = column;
            listViewSortAdorner = new SortAdorner(listViewSortCol, newDir);
            AdornerLayer.GetAdornerLayer(listViewSortCol).Add(listViewSortAdorner);
            lstMain.Items.SortDescriptions.Add(new SortDescription(sortBy, newDir));
        }

        #endregion Events
        private bool lstMainFilter(object item)
        {
            bool r = false;
            //if (Lieferliste_WPF.MainWindow.SearchBar == null) return true;
            String Expression = "";// (Lieferliste_WPF.MainWindow.SearchBar.FindName("txtSearch") as TextBox).Text;
            String fields = "";// (Lieferliste_WPF.MainWindow.SearchBar.FindName("cmbFields") as ComboBox).Text;
            Process lstItem = item as Process;
            if (String.IsNullOrEmpty(Expression))
                return true;
            else
            {
                switch (fields)
                {
                    case "TTNR":
                        r = (lstItem.Material.IndexOf(Expression, StringComparison.OrdinalIgnoreCase) >= 0);
                        break;
                    case "Teil":
                        r = (lstItem.MaterialDescription.IndexOf(Expression, StringComparison.OrdinalIgnoreCase) >= 0);
                        break;
                    case "Auftrag":
                        r = (lstItem.OrderNumber.IndexOf(Expression, StringComparison.OrdinalIgnoreCase) >= 0);
                        break;
                    case "alle":
                        r = ((lstItem.Material.IndexOf(Expression, StringComparison.OrdinalIgnoreCase) >= 0)
                            || (lstItem.MaterialDescription.IndexOf(Expression, StringComparison.OrdinalIgnoreCase) >= 0)
                            || (lstItem.OrderNumber.IndexOf(Expression, StringComparison.OrdinalIgnoreCase) >= 0));
                        break;
                }
            }
            return r;
        }
        private MachineContainerViewModel ViewModel
        {
            get { return (MachineContainerViewModel)this.DataContext; }
        }
        private void Machines_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView lv = e.Source as ListView;
            ViewModel.ChangeActiveMachine(lv.Items.CurrentItem as MachineViewModel);

        }
        int GetCurrentIndex(ListBox listBox, Func<IInputElement, Point> getPosition)
        {
            for (int index = 0; index < listBox.Items.Count; index++)
            {
                var item = listBox.ItemContainerGenerator.ContainerFromIndex(index) as ListBoxItem;
                if (item != null && this.IsMouseOverTarget(item, getPosition))
                {
                    return index;
                }
            }
            return -1;
        }
        bool IsMouseOverTarget(Visual target, Func<IInputElement, Point> getPosition)
        {
            Rect bounds = VisualTreeHelper.GetDescendantBounds(target);
            Point position = getPosition((IInputElement)target);
            return bounds.Contains(position);
        }


    }

}
