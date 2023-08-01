
using Lieferliste_WPF.ViewModels.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using Lieferliste_WPF.Data;
using Lieferliste_WPF.Data.Models;
using System.Security.Cryptography;
using System.Collections;
using Lieferliste_WPF.Utilities;
using System.Collections.ObjectModel;
using System.Reflection;
using System.ServiceModel.Channels;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.EntityFrameworkCore;
using Remotion.Linq.Collections;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Windows.Data;
using Lieferliste_WPF.Commands;
using System.Windows;
using Lieferliste_WPF.View;
using System.Diagnostics;
using Lieferliste_WPF.UserControls;
using System.DirectoryServices;
using System.Text;
using System.Windows.Navigation;
using Microsoft.IdentityModel.Tokens;
using System.Windows.Input;
using System.Windows.Controls;
using Microsoft.VisualBasic;
using System.Windows.Threading;
using Lieferliste_WPF.ViewModels.Base;
using HandlebarsDotNet.Helpers;
using NUnit.Framework.Interfaces;
using System.Collections.Specialized;

namespace Lieferliste_WPF.ViewModels
{
    class LieferViewModel : ViewModelBase
    {


        public ICollectionView OrdersView { get; }
        public ICommand TextSearchCommand => textSearchCommand ??= new RelayCommand(OnTextSearch);


        private ObservableCollection<Vorgang> _orders { get; } = new();
        public List<Vorgang> PrioOrders { get; } = new(20);
        public ActionCommand SortAscCommand { get; private set; }
        public ActionCommand SortDescCommand { get; private set; }
        public ActionCommand OrderViewCommand { get; private set; }
        public ActionCommand SaveCommand { get; private set; }
  
        public String HasMouse { get; set; } = String.Empty;
        private static bool isLoaded = false;
        private Dictionary<String, String> _filterCriterias;
        private String _sortField = String.Empty;
        private String _sortDirection = String.Empty;
        private RelayCommand textSearchCommand;
        private string _searchFilterText = String.Empty;
        internal CollectionViewSource OrdersViewSource {get; private set;} = new();

        public LieferViewModel()
        {
           
            LoadDataFast();
            LoadData();

            Debug.WriteLine("PrioOrders {0}", PrioOrders?.Count ?? -1);
            OrdersView = CollectionViewSource.GetDefaultView(_orders);
            OrdersView.Filter += OrdersView_FilterPredicate;
            
            _filterCriterias = new();

            SortAscCommand = new ActionCommand(OnAscSortExecuted, OnAscSortCanExecute);
            SortDescCommand = new ActionCommand(OnDescSortExecuted, OnDescSortCanEcecute);
            OrderViewCommand = new ActionCommand(OnOrderViewExecuted, OnOrderViewCanExecute);
            SaveCommand = new ActionCommand(OnSaveExecuted, OnSaveCanExecute);




        }



        private bool OrdersView_FilterPredicate(object value)
        {
            Vorgang ord = (Vorgang)value;

            bool accepted = true;

            if (!string.IsNullOrWhiteSpace(_searchFilterText))
            {
                _searchFilterText = _searchFilterText.ToUpper();
                if (!(accepted = ord.Aid.ToUpper().Contains(_searchFilterText)))
                    if (!(accepted = ord.AidNavigation.Material?.ToUpper().Contains(_searchFilterText) ?? false))
                        accepted = ord.AidNavigation.MaterialNavigation?.Bezeichng?.ToUpper().Contains(_searchFilterText) ?? false;
            }
            return accepted;
        }

        public string ToolTip
        {
            get
            {
                var toolTip = new StringBuilder();
                if (_filterCriterias.Count > 0)
                {
                    toolTip.Append("gefiltert:\n");
                    foreach (KeyValuePair<String, String> c in _filterCriterias)
                    {
                        toolTip.Append(c.Key).Append(" = ").Append(c.Value).Append("\n");
                    }
                }
                if (_sortField != string.Empty)
                {
                    toolTip.Append("sortiert nach:\n").Append(_sortField).Append(" / ").Append(_sortDirection);
                }

                return toolTip.ToString();
            }
        }


        private void OnTextSearch(object commandParameter)
        {
            if (commandParameter is TextChangedEventArgs change)
            {
                TextBox tb = (TextBox)change.OriginalSource;

                _searchFilterText = tb.Text;
                var uiContext = SynchronizationContext.Current;
                uiContext?.Send(x => OrdersView.Refresh(),null);           }
        }
        #region Commands
        private bool OnOrderViewCanExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission("OOPEN01");
        }
        private void OnOrderViewExecuted(object parameter)
        {
            if (parameter is Vorgang vrg)
            {
                OrderView ov = new(vrg.Aid);
                Window wnd = new();
                wnd.Owner = Application.Current.MainWindow;
                wnd.Content = ov;
                wnd.Show();
            }
        }
        private void OnSaveExecuted(object obj)
        {
            Dbctx.SaveChangesAsync();
        }
        private bool OnSaveCanExecute(object arg)
        {
            return Dbctx.ChangeTracker.HasChanges();
        }
        private bool OnDescSortCanEcecute(object parameter)
        {
            return !HasMouse.IsNullOrEmpty();

        }
        private void OnDescSortExecuted(object parameter)
        {
            
            if (OrdersView != null)
            {
                
                if (parameter is LieferlisteControl lvc)
                {
                    OrdersViewSource.SortDescriptions.Clear();
                    OrdersViewSource.SortDescriptions.Add(new SortDescription(lvc.HasMouseOver, ListSortDirection.Descending));
                    OrdersView.Refresh();
                }
            }
        }

        private bool OnAscSortCanExecute(object parameter)
        {

            return !HasMouse.IsNullOrEmpty();

        }

        private void OnAscSortExecuted(object parameter)
        {
            if (OrdersView != null)
            {
                string v = Translate();


                if (v != String.Empty)
                {
                    OrdersViewSource.SortDescriptions.Clear();
                    OrdersViewSource.SortDescriptions.Add(new SortDescription(v, ListSortDirection.Ascending));
                    var uiContext = SynchronizationContext.Current;
                    uiContext?.Send(x => OrdersView.Refresh(), null);
                }
            }
        } 
        #endregion
        private string Translate()
        {
            string v = HasMouse switch
            {
                "txtOrder" => "Aid",
                "txtVnr" => "Vorgangs.Vnr",
                "txtMatText" => "MaterialNavigation.Bezeichng",
                "txtMatTTNR" => "Material",
                "txtVrgText" => "Vorgangs.Text",
                "txtPlanT" => "Termin",
                "txtProj" => "ProId",
                "txtPojInfo" => "ProInfo",
                "txtQuant" => "Quantity",
                "txtWorkArea" => "Vorgangs.ArbplSap",
                "txtQuantYield" => "Vorgangs.QuantityYield",
                "txtScrap" => "Vorgangs.QuantityScrap",
                "txtOpen" => "Vorgangs.QuantityMiss",
                "txtRessTo" => "Vorgangs.RidNavigation.RessName",
                _ => String.Empty,
            };
            
            return v;
            
        }
        private async void LoadData()
        {
            
           var r = await Dbctx.Vorgangs
           .Include(m => m.ArbPlSapNavigation)
           .Include(d => d.RidNavigation)
           .Include(v => v.AidNavigation)
           .ThenInclude(x => x.MaterialNavigation)
           .Include(x => x.AidNavigation.DummyMatNavigation)
           .Where(x => !x.AidNavigation.Abgeschlossen && x.Aktuell)
           .ToListAsync();

            _orders.Clear();
            foreach(var item in r)
            {
                _orders.Add(item);
            }
                
            isLoaded = true;
           
        }
        private void LoadDataFast()
        {
            var vrg = Dbctx.Vorgangs
            .Include(m => m.ArbPlSapNavigation)
            .Include(d => d.RidNavigation)
            .Include(v => v.AidNavigation)
            .ThenInclude(x => x.MaterialNavigation)
            .Include(x => x.AidNavigation.DummyMatNavigation)
            .Where(x => !x.AidNavigation.Abgeschlossen && x.Aktuell)
            .Take(20)
            .ToList();

            PrioOrders.Clear();
            foreach (var item in vrg)
            {
                PrioOrders.Add(item);
            }
        }
    }
}
