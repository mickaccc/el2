﻿using System;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace WpfCustomControlLibrary
{
    public class ExpandPopup : ItemsControl
    {
        private Expander? _expand;
        private DataGrid? _datagrid;
        static ExpandPopup()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ExpandPopup), new FrameworkPropertyMetadata(typeof(ExpandPopup)));
        }

        protected bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsOpen.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(ExpandPopup), new PropertyMetadata(false));


        public string[] Headers
        {
            get { return (string[])GetValue(HeadersProperty); }
            set { SetValue(HeadersProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Headers.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeadersProperty =
            DependencyProperty.Register("Headers", typeof(string[]), typeof(ExpandPopup), new PropertyMetadata(default));



        public string[] OriginalHeaders
        {
            get { return (string[])GetValue(OriginalHeadersProperty); }
            set { SetValue(OriginalHeadersProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Values.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OriginalHeadersProperty =
            DependencyProperty.Register("OriginalHeadersValues", typeof(string[]), typeof(ExpandPopup), new PropertyMetadata(default));



        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _expand = (Expander)Template.FindName("PART_Expand", this);
            _expand.Expanded += OnExpand;
            _expand.Collapsed += OnCollapse;

            _datagrid = (DataGrid)Template.FindName("PART_DataGrid", this);
            _datagrid.AutoGeneratingColumn += OnAutoGenerating;

            LostFocus += OnLostFocus;
        }

        private void OnAutoGenerating(object? sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (OriginalHeaders.Length > 0 && OriginalHeaders.Length == Headers.Length)
            {
   
                int index = 0;
                var zip = OriginalHeaders.Zip(Headers);
                var dtg = (DataGrid)sender;
                if (OriginalHeaders.Any(x => x.Contains(e.PropertyName)))
                {
                    foreach (var z in zip)
                    {
                        if (dtg.Columns.All(x => x.Header.ToString() != z.Second))
                        {
                            if (z.First.Split('.').First() == e.PropertyName)
                            {
                                var textcolumn = new DataGridTextColumn();
                                textcolumn.Binding = new Binding(z.First);
                                textcolumn.Header = z.Second;

                                if (dtg.Columns.Count > index) dtg.Columns.Insert(index, textcolumn);
                                else dtg.Columns.Add(textcolumn);
                                break;
                            }
                            else
                            {

                                if (z.First == e.PropertyName)
                                {
                                    e.Column.Header = z.Second;
                                    if (dtg.Columns.Count > index) dtg.Columns.Insert(index, e.Column);
                                    else dtg.Columns.Add(e.Column);
                                    break;
                                }
                            }
                        }
                        index++;
                    }
                }
                e.Cancel = true;
            }
            else
            {
                throw new ArgumentOutOfRangeException("Failure on Headers or OriginalHeaders");
            }
        }

        private void OnLostFocus(object sender, RoutedEventArgs e)
        {
            _expand.IsExpanded = false;
            
        }

        private void OnCollapse(object sender, RoutedEventArgs e)
        {           
            IsOpen = false;
        }

        private void OnExpand(object sender, RoutedEventArgs e)
        {
            var ex = (Expander)e.Source;
            IsOpen = ex.IsExpanded;
        }
    }
}
