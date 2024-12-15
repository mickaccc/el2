using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

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

            }
        }

        private bool FilterPredicate(object obj)
        {
            return true;
        }

        private void Searching(object sender, TextChangedEventArgs e)
        {
            ItemsView.Refresh();
        }

        private TextBox? _searchBox;
        public Visibility BoxIsVisible
        {
            get { return (Visibility)GetValue(BoxIsVisibleProperty); }
            set { SetValue(BoxIsVisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BoxIsVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BoxIsVisibleProperty =
            DependencyProperty.Register("BoxIsVisible", typeof(Visibility), typeof(SearchableComboBox), new PropertyMetadata(Visibility.Visible));

        ICollectionView ItemsView;
        public object[] ItemsSource
        {
            get { return (object[])GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(object[]), typeof(SearchableComboBox), new PropertyMetadata(null, ItemsSourceChanged));

        private static void ItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dep = d as SearchableComboBox;
            dep.ItemsView = CollectionViewSource.GetDefaultView(e.NewValue);
            dep.ItemsView.Filter += dep.FilterPredicate;
        }
    }
}
