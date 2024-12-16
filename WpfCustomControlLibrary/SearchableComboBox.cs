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

namespace WpfCustomControlLibrary
{
    public class SearchableComboBox : DataGrid
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
                //_searchBox.TextChanged += Searching;
                //_itemsBox = Template.FindName("PART_ItemsBox", this) as Popup;
                _itemsList = Template.FindName("PART_List", this) as DataGrid;
                _itemsList.ItemsSource = ItemsViewSource.View;
                //ItemsViewSource = new CollectionViewSource() { Source = _itemsList.ItemsSource };
                //ItemsViewSource.Filter += FilterEvent;
                _itemsList.SelectionChanged += Selected;
                _itemsList.AutoGeneratingColumn += AutoGenerating;

            }
        }

        private void AutoGenerating(object? sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            var dt = sender as DataGrid;
            var ty = e.PropertyName;
            e.Column.Header = "TE";
        }

        private void Selected(object sender, SelectionChangedEventArgs e)
        {
            _itemsBox.IsOpen = false;
        }

        private bool FilterPredicate(object obj)
        {
            var ty = obj.GetType();
            var pr = ty.GetFields();
            var fi = pr[0].GetValue(obj) as string;

            return fi.Contains(_searchBoxText);
        }

        private void Searching(object sender, TextChangedEventArgs e)
        {
            _searchBoxText = _searchBoxText;
            _itemsBox.IsOpen = true;
            //ItemsView.Refresh();
        }

        private TextBox? _searchBox;
        private Popup? _itemsBox;
        private DataGrid? _itemsList;
        private static string _searchBoxText = string.Empty;
        public Visibility BoxIsVisible
        {
            get { return (Visibility)GetValue(BoxIsVisibleProperty); }
            set { SetValue(BoxIsVisibleProperty, value); }
        }
        // This property gives you flexibility to hide or unhide FullTextSearch functionality  
        public static readonly DependencyProperty EnableFullTextSearchProperty =
            DependencyProperty.Register("EnableFullTextSearch", typeof(bool),
                typeof(SearchableComboBox), new UIPropertyMetadata(false));
        public bool EnableFullTextSearch
        {
            get { return (bool)GetValue(EnableFullTextSearchProperty); }
            set { SetValue(EnableFullTextSearchProperty, value); }
        }
        // Using a DependencyProperty as the backing store for BoxIsVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BoxIsVisibleProperty =
            DependencyProperty.Register("BoxIsVisible", typeof(Visibility), typeof(SearchableComboBox), new PropertyMetadata(Visibility.Visible));

        internal ICollectionView ItemsView;
        internal CollectionViewSource ItemsViewSource;
        private List<dynamic> _items = [];
        public IEnumerable<object> ItemsSource
        {
            get { return (IEnumerable<object>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable<object>), typeof(SearchableComboBox), new PropertyMetadata(null, ItemsSourceChanged));



        private static void ItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dep = d as SearchableComboBox;
            foreach (var item in (IEnumerable<object>)e.NewValue)
            {
                dep._items.Add(item);
            }
            dep.ItemsViewSource = new CollectionViewSource() { Source = dep._items };
            dep.ItemsViewSource.Filter += new FilterEventHandler(FilterEvent);
            

        }

        private static void FilterEvent(object sender, FilterEventArgs e)
        {

            var fi = e.Item.GetType().GetFields().First().GetValue(e.Item).ToString();
            
            e.Accepted = fi.Contains(_searchBoxText);
        }
        public struct ListItem()
        {
            public string Header = string.Empty;
            public string Value = string.Empty;
        }
    }
}
