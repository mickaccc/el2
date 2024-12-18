﻿using System;
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
                _searchBox.Text = _PreSelect;
                _searchBox.TextChanged += Searching;
                _searchBox.GotFocus += SearchGotFocus;
                _itemsBox = Template.FindName("PART_ItemsBox", this) as Popup;
                _itemsList = Template.FindName("PART_List", this) as DataGrid;
                _itemsList.MouseDoubleClick += Row_DoubleClick;
                _itemsList.ItemsSource = ItemsViewSource?.View;

            }
        }

        private void SearchGotFocus(object sender, RoutedEventArgs e)
        {
            _searchBox.Text = string.Empty;
        }


        private TextBox? _searchBox;
        private Popup? _itemsBox;
        private DataGrid? _itemsList;
        private static string _searchBoxText = string.Empty;
        private string _PreSelect = string.Empty;
        internal CollectionViewSource? ItemsViewSource;
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


        public double PopupWidth
        {
            get { return (double)GetValue(PopupWidthProperty); }
            set { SetValue(PopupWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PopupWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PopupWidthProperty =
            DependencyProperty.Register("PopupWidth", typeof(double), typeof(SearchableComboBox), new PropertyMetadata(200.0));



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
            var dep = (SearchableComboBox)d;
            dep.ItemsViewSource = new CollectionViewSource() { Source = e.NewValue };
            dep.ItemsViewSource.Filter += new FilterEventHandler(FilterEvent);
        }


        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(SearchableComboBox), new PropertyMetadata(null, SelectItemChanged));

        private static void SelectItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dep = (SearchableComboBox)d;

            dep._PreSelect = e.NewValue.GetType().GetRuntimeFields().ElementAt(0).GetValue(e.NewValue).ToString();
        }

        public int[] DisplayFieldNumbers
        {
            get { return (int[])GetValue(DisplayFieldNumbersProperty); }
            set { SetValue(DisplayFieldNumbersProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DisplayFieldNumbers.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisplayFieldNumbersProperty =
            DependencyProperty.Register("DisplayFieldNumbers", typeof(int[]), typeof(SearchableComboBox), new PropertyMetadata(new int[1], DisplayNumbers));


        private static void DisplayNumbers(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private static void FilterEvent(object sender, FilterEventArgs e)
        {

            var ft = e.Item.GetType();
            var fis = ft.GetRuntimeFields();

            var va = fis.ElementAt(0).GetValue(e.Item).ToString();
           
            e.Accepted = va.Contains(_searchBoxText, StringComparison.InvariantCultureIgnoreCase);
            
        }
        private void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Ensure row was clicked and not empty space

            if (ItemsControl.ContainerFromElement((DataGrid)sender,
                    e.OriginalSource as DependencyObject) is not DataGridRow row) return;
            var fields = row.Item.GetType().GetRuntimeFields();
            var field1 = fields.ElementAt(DisplayFieldNumbers[0]).GetValue(row.Item);
            _searchBox.Text = field1.ToString();
            _itemsBox.IsOpen = false;
            SetValue(SelectedItemProperty, ItemsViewSource?.View.CurrentItem);
        }

        private void Searching(object sender, TextChangedEventArgs e)
        {
            _searchBoxText = _searchBox.Text;
            
            _itemsBox.IsOpen = true;
            ItemsViewSource?.View.Refresh();
        }
    }
}
