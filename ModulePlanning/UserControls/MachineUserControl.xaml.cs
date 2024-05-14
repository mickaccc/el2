using El2Core.Services;
using ModulePlanning.Planning;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using static ModulePlanning.Specials.Constances;

namespace ModulePlanning.UserControls
{
    /// <summary>
    /// Interaction logic for MachineUserControl.xaml
    /// </summary>
    public partial class MachineUserControl : UserControl
    {
        public MachineUserControl()
        {         
            InitializeComponent();
        }

        private void Pl_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "ScrollItem")
            {
                Planed.ScrollIntoView((sender as PlanMachine)?.ScrollItem);
            }
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var dtx = this.DataContext as PlanMachine;
            if (dtx != null)
            {
                var list = dtx.ProcessesCV as ListCollectionView;
                if (list.IsAddingNew) { list.CommitNew(); }
                if (list.IsEditingItem) { list.CommitEdit(); }
                dtx.ProcessesCV.SortDescriptions.Clear();
                dtx.ProcessesCV.SortDescriptions.Add(new SortDescription("SortPos", ListSortDirection.Ascending));
            }
        }

        private void HideDetails_Click(object sender, RoutedEventArgs e)
        {
            var pl = FindName("Planed") as DataGrid;
            pl.SelectedIndex = -1;
            e.Handled = true;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var pl = this.DataContext as PlanMachine;
            pl.PropertyChanged += Pl_PropertyChanged;

            var sett = new UserSettingsService().TlColumns;
            bool f = false;
            foreach (var col in sett)
            {
                var tl = TLColumn.ColumnNames[col];
                if (Planed.Columns.Count == 2 && !f)
                {
                    var c = Planed.Columns[1] as DataGridTextColumn;

                    c.Header = tl.Item1;
                    var b = new Binding(tl.Item2);
                    if (tl.Item3 != "") b.Converter = (IValueConverter)Activator.CreateInstance(Type.GetType(tl.Item3));
                    if (tl.Item4 != "") b.StringFormat = tl.Item4;
                    b.Mode = BindingMode.OneTime;
                    c.Binding = b;
                    f = true;
                }
                else
                {
                    DataGridTextColumn txtCol = new();
                    txtCol.Header = tl.Item1;
                    var b = new Binding(tl.Item2);
                    if (tl.Item3 != "") b.Converter = (IValueConverter)Activator.CreateInstance(Type.GetType(tl.Item3));
                    if (tl.Item4 != "") b.StringFormat = tl.Item4;
                    b.Mode = BindingMode.OneTime;
                    txtCol.Binding = b;
                    Planed.Columns.Add(txtCol);                   
                }
            }           
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            shiftOpen.IsChecked = false;
        }
    }
}
