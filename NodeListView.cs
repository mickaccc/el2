using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using Lieferliste_WPF.UserControls;
using Lieferliste_WPF.Working;
using Lieferliste_WPF.Planning;
using Lieferliste_WPF.Entities;
using System.Reflection;
using System.Collections.Specialized;
using System.Windows.Input;
using Lieferliste_WPF.Commands;


namespace Lieferliste_WPF
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Lieferliste_WPF"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Lieferliste_WPF;assembly=Lieferliste_WPF"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:NodeListView/>
    ///
    /// </summary>
    public class NodeListView : ContentControl
    {
        private StackPanel _listStack;
        private int _prevIndex = -1;
        public delegate Point GetPosition(IInputElement element);
        public static readonly DependencyProperty CriteriaProperty;
        public String ValueOrder { get; set; }
        public String ErrorOrders { get { return _errorOrders.ToString(); } }
        public String OverFlowOrders { get { return _overFlowOrders.ToString(); } }

        public DataColumn HeaderID { get; set; }
        public String[] ListProjectionHeaderText { get; set; }
        public String[] ListProjectionFields { get; set; }
        public int[] ListColumnWitdh { get; set; }
        internal List<IMachine> MachineContainer = new List<IMachine>();
        //public ObservableDictionary<int, PlannerControl> nodePlannerControls = new ObservableDictionary<int, PlannerControl>();
        private StringBuilder _errorOrders = new StringBuilder();
        private StringBuilder _overFlowOrders = new StringBuilder();

        private SortableObservableCollection<DataTable> _ressCollection { get; set; }

        static NodeListView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NodeListView), new FrameworkPropertyMetadata(typeof(NodeListView)));
            CriteriaProperty = DependencyProperty.Register("Criteria",
                typeof(int), typeof(NodeListView));


        }

        public override void OnApplyTemplate()
        {

            base.OnApplyTemplate();
            if (this.Template != null)
            {
                _listStack = this.Template.FindName("PART_ListStack", this) as StackPanel;

                if (_listStack != null)
                {
                        int c = 0;
                        foreach (IMachine m in MachineContainer)
                        {

                            StackPanel stP = new StackPanel();
                            stP.Tag = m.RID;
                            stP.Uid = "RESSOURCE";
                            Button btn = new Button();
                            btn.Content = m.MachineName;
                            btn.Uid = "RessName";
                            btn.Tag = m.RID;
                            btn.Click += btn_Click;
                            
                            if (c % 2 == 0)
                            {
                                btn.Background = Brushes.LightSeaGreen;
                            }
                            else { btn.Background = Brushes.LightGreen; }
                            c++;
                            DataGrid dataGrid = new DataGrid();
   
                            dataGrid.ItemsSource = m.ProcessesLine;
                            dataGrid.AutoGenerateColumns = false;
                            dataGrid.IsReadOnly = true;

                            if (ListProjectionFields.Length == ListProjectionHeaderText.Length &&
                                ListProjectionFields.Length == ListColumnWitdh.Length)
                            {
                                Process pro = new Process("");
                                    foreach (PropertyInfo orderProp in pro.GetType().GetProperties())
                                    {
                                        DataGridTextColumn newCol = new DataGridTextColumn();
                                        Binding b = new Binding(orderProp.Name);
                                        b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                                        b.Mode = BindingMode.OneWay;
                                        newCol.Binding = b;
                                        newCol.Visibility = System.Windows.Visibility.Hidden;
                                        int datagridWith=0;
                                        for (int i = 0; i < ListProjectionFields.Length; i++)
                                        {

                                            if (orderProp.Name == ListProjectionFields[i])
                                            {
                                                newCol.Header = ListProjectionHeaderText[i];
                                                newCol.Width = ListColumnWitdh[i];
                                                newCol.Visibility = System.Windows.Visibility.Visible;
                                                datagridWith =+ ListColumnWitdh[i];
                                            }
                                        }
                                        dataGrid.MinWidth = datagridWith;
                                        dataGrid.Columns.Add(newCol);
                                        }                             
                            }

                            dataGrid.HeadersVisibility = DataGridHeadersVisibility.All;
                            Binding actualHG = new Binding("ActualHeight");
                            actualHG.Source = _listStack;
                            dataGrid.SetBinding(DataGrid.HeightProperty,actualHG);
                            dataGrid.CanUserSortColumns = false;
                            dataGrid.AddHandler(ListView.MouseMoveEvent,
                                new RoutedEventHandler(dataGrid_MouseMove), true);
                            dataGrid.PreviewMouseLeftButtonDown += dataGrid_PreviewMouseLeftButtonDown;
                            dataGrid.DragEnter += dataGrid_DragEnter;
                            dataGrid.Drop += dataGrid_Drop;
                            dataGrid.DragLeave += dataGrid_DragLeave;
                            dataGrid.DragOver += dataGrid_DragOver;
                            dataGrid.Loaded += dataGrid_Loaded;
                            dataGrid.LoadingRow += dataGrid_initialNewItem;
                            ToggleButton chk = new ToggleButton();
                            chk.Tag = stP.Tag;
                            Binding selectB = new Binding("isSelected");
                            selectB.Source = m;
                            chk.SetBinding(ToggleButton.IsCheckedProperty,selectB);
                            chk.MinWidth = 10;
                            DockPanel headerDock = new DockPanel();
                            headerDock.Children.Add(btn);
                            headerDock.Children.Add(chk);
                            stP.Children.Add(headerDock);
                            stP.Children.Add(dataGrid);

                            _listStack.Children.Add(stP);

                            //PlannerControl pl = new PlannerControl();
                            //pl.myMachine = m;
                            //pl.create();
                            //nodePlannerControls.Add(m.RID, pl);

                        }
                }
            }
        }
        #region Events
        void dataGrid_initialNewItem(object sender, DataGridRowEventArgs e)
        {
            Process pro = e.Row.Item as Process;
            System.Drawing.Color c;
            e.Row.ToolTip = pro.Material + " " + pro.MaterialDescription + "\n" + pro.ExecutionShortText + "\nTermin: KW " + pro.deadKW.ToString() + "\nRestzeit: " + pro.ProcessTime + " min.";
            
            if (DateUtils.GetGermanCalendarWeek(DateTime.Now).CompareTo(pro.deadKW) == 1)
            {
                c = Properties.Settings.Default.outOfDate;
            }
            else
            {
                c = Properties.Settings.Default.inDate;
            }
                e.Row.Background=new SolidColorBrush(Color.FromRgb(c.R,c.G,c.B));
        }
        void dataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            DataGrid grid = e.Source as DataGrid;
            //foreach (Process item in grid.Items)
            //{
            //    var row = grid.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;
            //    if (row != null)
            //    {
            //        row.ToolTip = item.Material + " " + item.MaterialDescription + "\n" + item.ExecutionShortText + "\nTermin: KW " + item.deadKW.ToString() + "\nRestzeit: " + item.ProcessTime + " min.";
            //        if (this.ErrorOrders.Contains(item.OrderNumber))
            //        {
            //            row.Background = Brushes.Yellow;
            //        }
            //        if (this.OverFlowOrders.Contains(item.OrderNumber))
            //        {
            //            row.Background = Brushes.Red;
            //        }
            //    }
            //}
        }

        void dataGrid_MouseMove(object sender, RoutedEventArgs e)
        {



        }
        void dataGrid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DataGrid dgrd = e.Source as DataGrid;
            _prevIndex = GetCurrentRowIndex(dgrd, e.GetPosition);
            dgrd.SelectedIndex = _prevIndex;
            DataObject data = new DataObject(typeof(DataGrid), dgrd);
            DragDrop.DoDragDrop(dgrd, data, DragDropEffects.Move);
        }
        void dataGrid_DragEnter(object sender, RoutedEventArgs e)
        {
            //(e.Source as DataGrid).Background = Brushes.Yellow;
        }
        void dataGrid_DragLeave(object sender, RoutedEventArgs e)
        {
            (e.Source as DataGrid).Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xF0, 0xF0, 0xF0));
        }
        void dataGrid_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DataGrid)) || e.Data.GetDataPresent(typeof(ListView)))
                e.Effects = DragDropEffects.Move;
            else
                e.Effects = DragDropEffects.None;
            e.Handled = true;
        }
        void dataGrid_Drop(object sender, DragEventArgs e)
        {
            Process pro;

            DataGrid dtGrid = e.Source as DataGrid;
            if (dtGrid == null) return;
            StackPanel st = dtGrid.Parent as StackPanel;
            if (st == null) return;
            InternalMachine IMach = MachineContainer.First(x => x.RID == (int)st.Tag) as InternalMachine;
            if (e.Data.GetDataPresent(typeof(ListView)))
            {
                ListView lstMain = (ListView)e.Data.GetData(typeof(ListView));
                pro = lstMain.SelectedValue as Process;
                if (pro == null) return;
                
                if (IMach != null)
                {

                    double? r = IMach.addOrder(pro.deadKW, pro);
                    dtGrid.Items.Refresh();

                    if (r == null)
                    {
                        MessageBox.Show("Der Auftrag " + pro.OrderNumber
                            + " kann nicht eingeplant werden, da zuwenig Kapazität vorhanden ist!\nBenötigt wird "
                            + pro.ProcessTime + " Minuten.", IMach.MachineName, MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        ObservableList<Process> op = lstMain.ItemsSource as ObservableList<Process>;
                        op.Remove(pro);
                        Allocation alloc = lstMain.Parent as Allocation;

                    }
                }
            }
            else
            {
                DataGrid grd = e.Data.GetData(typeof(DataGrid)) as DataGrid;
                if (grd == null) return;
                pro = (Process)grd.SelectedValue;
                if (pro == null) return;
                StackPanel thisSt = grd.Parent as StackPanel;
                if (st.Tag != thisSt.Tag)
                {
                    double? r = IMach.addOrder(pro.deadKW, pro);
                    if (r == null)
                    {
                        MessageBox.Show("Der Auftrag " + pro.OrderNumber
                            + " kann nicht eingeplant werden, da zuwenig Kapazität vorhanden ist!\nBenötigt wird "
                            + pro.ProcessTime + " Minuten.", IMach.MachineName, MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        StackPanel sourceSt = grd.Parent as StackPanel;
                        InternalMachine IMachS = MachineContainer.First(x => x.RID == (int)sourceSt.Tag) as InternalMachine;
                        IMachS.Remove(pro);
                    }
                }
                else
                {
                    InternalMachine IMachThis = MachineContainer.Find(x => x.RID == (int)thisSt.Tag) as InternalMachine;

                    int index = GetCurrentRowIndex((DataGrid)sender, e.GetPosition);
                    if (index < 0) return;
                    if (_prevIndex < 0) return;
                    Process target = grd.Items[index] as Process;

                    IMachThis.moveOrder(target, pro);

                }
                grd.Items.Refresh();
                dtGrid.Items.Refresh();

            }
            dtGrid.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xF0, 0xF0, 0xF0));
            _prevIndex = -1;
        }

        void btn_Click(object sender, RoutedEventArgs e)
        {
            //int rid = (int)(sender as Button).Tag;
            //MainWindow.MainWindowRef.AddTabItem(nodePlannerControls[rid]);
        }
        #endregion Events


        private bool GetMouseTargetRow(Visual theTarget, GetPosition position)
        {
            if (theTarget == null) return false;
            Rect rect = VisualTreeHelper.GetDescendantBounds(theTarget);
            Point point = position((IInputElement)theTarget);
            return rect.Contains(point);
        }

        private DataGridRow GetRowItem(DataGrid dataGrid,int index)
        {

            if (dataGrid.ItemContainerGenerator.Status
                    != GeneratorStatus.ContainersGenerated)
                return null;
            return dataGrid.ItemContainerGenerator.ContainerFromIndex(index)
                                                            as DataGridRow;
        }

        private int GetCurrentRowIndex(DataGrid dataGrid,GetPosition pos)
        {
            int curIndex = -1;
            for (int i = 0; i < dataGrid.Items.Count; i++)
            {
                DataGridRow itm = GetRowItem(dataGrid,i);
                if (GetMouseTargetRow(itm, pos))
                {
                    curIndex = i;
                    break;
                }
            }
            return curIndex;
        }

        public int Criteria
        {
            get { return (int)GetValue(CriteriaProperty); }
            set { SetValue(CriteriaProperty, value); }
        }
    }
}

