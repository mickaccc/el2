﻿using El2Core.Converters;
using El2Core.Models;
using ModulePlanning.Planning;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;

namespace ModulePlanning.Dialogs
{
    /// <summary>
    /// Interaction logic for MachineView.xaml
    /// </summary>
    public partial class MachineView : UserControl
    {


        public string BemTInfo
        {
            get { return (string)GetValue(BemTInfoProperty); }
            set { SetValue(BemTInfoProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BemTInfo.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BemTInfoProperty =
            DependencyProperty.Register("BemTInfo", typeof(string), typeof(MachineView), new PropertyMetadata(""));


        public MachineView()
        {
            InitializeComponent();
        }


        private void Process_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName == "CostId" || e.PropertyName == "WorkArId" || e.PropertyName == "resv" || e.PropertyName == "Bullet")
            {
                e.Cancel = true;
            }
            else if (e.PropertyName == "Aid") e.Column.Header = "Auftrg";
            else if (e.PropertyName == "QuantityMiss") e.Column.Header = "offene Menge";
            else if (e.PropertyName == "Text") e.Column.Header = "Vorgang Kurztext";
            else if (e.PropertyName == "Arbid") e.Column.Header = "Arbeitsplatz";
            else if (e.PropertyType == typeof(DateTime?) || e.PropertyType == typeof(DateTime))
            {
                DataGridTextColumn? dgtc = e.Column as DataGridTextColumn;
                DateConverter con = new();
                if (dgtc != null)
                    ((Binding)dgtc.Binding).Converter = con;
            }
        }


        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            var dp = sender as DatePicker;
            if (dp?.DataContext is Vorgang vrg) { vrg.Termin = dp?.SelectedDate; }
        }

        //private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        //{
        //    var tx = sender as TextBox;
        //    if (tx?.DataContext is Vorgang vrg && DataContext is PlanMachine ctx)
        //    { ctx.Focused = new(vrg.VorgangId, TextBox.TextProperty.Name); }
        //}

        //private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        //{
        //    if (DataContext is PlanMachine ctx) ctx.Focused = null;
        //}
    }
}
