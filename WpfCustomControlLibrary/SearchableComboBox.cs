using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;

namespace WpfCustomControlLibrary
{
    public class SearchableComboBox : ContentControl
    {
        static SearchableComboBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(SearchableComboBox), new FrameworkPropertyMetadata(typeof(SearchableComboBox)));
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (this.Template != null)
            {
                _searchBox = Template.FindName("PART_SearchBox", this) as TextBox;
                _searchBox.TextChanged += Searching;
                _itemsBox = Template.FindName("PART_ItemsBox", this) as Popup;
                _itemsList = Template.FindName("PART_List", this) as DataGrid;
                _itemsList.MouseDoubleClick += Row_DoubleClick;
                _itemsList.ItemsSource = ItemsViewSource.View;

            }
        }

        private TextBox? _searchBox;
        private Popup? _itemsBox;
        private DataGrid? _itemsList;
        private static string _searchBoxText = string.Empty;
        internal CollectionViewSource ItemsViewSource;
        public bool CanUserAddRows
        {
            get { return (bool)GetValue(CanUserAddRowsProperty); }
            set { SetValue(CanUserAddRowsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CanUserAddRows.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CanUserAddRowsProperty =
            DependencyProperty.Register("CanUserAddRows", typeof(bool), typeof(SearchableComboBox), new PropertyMetadata(false));


        public bool CanUserDeleteRows
        {
            get { return (bool)GetValue(CanUserDeleteRowsProperty); }
            set { SetValue(CanUserDeleteRowsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CanUserDeleteRows.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CanUserDeleteRowsProperty =
            DependencyProperty.Register("CanUserDeleteRows", typeof(bool), typeof(SearchableComboBox), new PropertyMetadata(false));

        // Using a DependencyProperty as the backing store for BoxIsVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BoxIsVisibleProperty =
            DependencyProperty.Register("BoxIsVisible", typeof(Visibility), typeof(SearchableComboBox), new PropertyMetadata(Visibility.Visible));

        public IEnumerable<dynamic> ItemsSource
        {
            get { return (IEnumerable<dynamic>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable<dynamic>), typeof(SearchableComboBox), new PropertyMetadata(null, ItemsSourceChanged));



        private static void ItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dep = d as SearchableComboBox;
            dep.ItemsViewSource = new CollectionViewSource() { Source = e.NewValue };
            dep.ItemsViewSource.Filter += new FilterEventHandler(FilterEvent);
        }


        public string SelectedValue
        {
            get { return (string)GetValue(SelectedValueProperty); }
            set { SetValue(SelectedValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedValueProperty =
            DependencyProperty.Register("SelectedValue", typeof(string), typeof(SearchableComboBox), new PropertyMetadata(""));


        private static void FilterEvent(object sender, FilterEventArgs e)
        {

            var ft = e.Item.GetType();
            var fis = ft.GetRuntimeFields();
            var va = fis.First().GetValue(e.Item).ToString();

            e.Accepted = va.Contains(_searchBoxText);
            
        }
        private void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Ensure row was clicked and not empty space
            var row = ItemsControl.ContainerFromElement((DataGrid)sender,
                                                e.OriginalSource as DependencyObject) as DataGridRow;

            if (row == null) return;
            var field0 = row.Item.GetType().GetRuntimeFields().ElementAt(0);
            var field1 = row.Item.GetType().GetRuntimeFields().ElementAt(1);
            _searchBox.Text = field0.GetValue(row.Item).ToString();
            _itemsBox.IsOpen = false;
            SelectedValue = string.Format("{0}-{1}",field0, field1);
        }

        private void Searching(object sender, TextChangedEventArgs e)
        {
            _searchBoxText = _searchBox.Text;
            _itemsBox.IsOpen = true;
            ItemsViewSource.View.Refresh();
        }
    }
}
