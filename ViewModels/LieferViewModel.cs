﻿using Lieferliste_WPF.Commands;
using Lieferliste_WPF.Data;
using Lieferliste_WPF.Data.Models;
using Lieferliste_WPF.Interfaces;
using Lieferliste_WPF.UserControls;
using Lieferliste_WPF.Utilities;
using Lieferliste_WPF.View;
using Lieferliste_WPF.ViewModels.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace Lieferliste_WPF.ViewModels
{
    class LieferViewModel : ViewModelBase, IProgressbarInfo
    {


        public ICollectionView OrdersView { get; }
        public ICommand TextSearchCommand => _textSearchCommand ??= new RelayCommand(OnTextSearch);


        private ConcurrentObservableCollection<Vorgang> _orders { get; } = new();
        private ObservableCollection<Vorgang> PrioOrders { get; } = new();
        public ActionCommand SortAscCommand { get; private set; }
        public ActionCommand SortDescCommand { get; private set; }
        public ActionCommand OrderViewCommand { get; private set; }
        public ActionCommand SaveCommand { get; private set; }
  
        public string HasMouse { get; set; } = string.Empty;
        private readonly Dictionary<string, string> _filterCriterias = new();
        private readonly string _sortField = string.Empty;
        private readonly string _sortDirection = string.Empty;
        private RelayCommand _textSearchCommand;
        private string _searchFilterText = string.Empty;
        private static double _progressValue;
        private bool _progressIsBusy;
        internal CollectionViewSource OrdersViewSource {get; private set;} = new();

        public LieferViewModel()
        {         
            OrdersView = CollectionViewSource.GetDefaultView(_orders);
            OrdersView.Filter += OrdersView_FilterPredicate;

            SortAscCommand = new ActionCommand(OnAscSortExecuted, OnAscSortCanExecute);
            SortDescCommand = new ActionCommand(OnDescSortExecuted, OnDescSortCanEcecute);
            OrderViewCommand = new ActionCommand(OnOrderViewExecuted, OnOrderViewCanExecute);
            SaveCommand = new ActionCommand(OnSaveExecuted, OnSaveCanExecute);

            ProgressIsBusy = true;


            Task.Run(async () =>
            {
                _orders.AddRange(await LoadAsync());
                
            });

        }


        private bool OrdersView_FilterPredicate(object value)
        {
            var ord = (Vorgang)value;

            var accepted = true;

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

        public double ProgressValue { get { return _progressValue; } 
            set
            {
               _progressValue = value;
                NotifyPropertyChanged(() => ProgressValue);
            } }
        public bool ProgressIsBusy { get { return _progressIsBusy; }
            set
            {
                _progressIsBusy = value;
                NotifyPropertyChanged(() => ProgressIsBusy);
            } }

        private void OnTextSearch(object commandParameter)
        {
            if (commandParameter is TextChangedEventArgs change)
            {
                var tb = (TextBox)change.OriginalSource;
                if (tb.Text.Length == 0 || tb.Text.Length >= 3)
                {
                    _searchFilterText = tb.Text;
                    var uiContext = SynchronizationContext.Current;
                    uiContext?.Send(x => OrdersView.Refresh(), null);
                }
            }
        }
        #region Commands
        private static bool OnOrderViewCanExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission("OOPEN01");
        }
        private static void OnOrderViewExecuted(object parameter)
        {
            if (parameter is Vorgang vrg)
            {
                OrderView ov = new(vrg.Aid);
                Window wnd = new()
                {
                    Owner = Application.Current.MainWindow,
                    Content = ov
                };
                wnd.Show();
            }
        }
        private static void OnSaveExecuted(object obj)
        {
            Dbctx.SaveChangesAsync();
        }
        private static bool OnSaveCanExecute(object arg)
        {

            try
            {
                return Dbctx.ChangeTracker.HasChanges();
            }
            catch (InvalidOperationException e)
            {
                MessageBox.Show(e.Message, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return false;
        }
        private bool OnDescSortCanEcecute(object parameter)
        {
            return !HasMouse.IsNullOrEmpty();

        }
        private void OnDescSortExecuted(object parameter)
        {
            
            if (parameter is LieferlisteControl lvc)
            {
                OrdersViewSource.SortDescriptions.Clear();
                OrdersViewSource.SortDescriptions.Add(new SortDescription(lvc.HasMouseOver, ListSortDirection.Descending));
                OrdersView.Refresh();
            }
            
        }

        private bool OnAscSortCanExecute(object parameter)
        {

            return !HasMouse.IsNullOrEmpty();

        }

        private void OnAscSortExecuted(object parameter)
        {

            var v = Translate();

            if (v != string.Empty)
            {
                OrdersViewSource.SortDescriptions.Clear();
                OrdersViewSource.SortDescriptions.Add(new SortDescription(v, ListSortDirection.Ascending));
                var uiContext = SynchronizationContext.Current;
                uiContext?.Send(x => OrdersView.Refresh(), null);
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
                _ => string.Empty,
            };
            
            return v;
            
        }

        private static Task<List<Vorgang>> LoadAsync()
        {

            return Dbctx.Vorgangs
            .Include(m => m.ArbPlSapNavigation)
            .Include(v => v.AidNavigation)
            .ThenInclude(x => x.MaterialNavigation)
            .Include(x => x.AidNavigation.DummyMatNavigation)
            .Where(x => !x.AidNavigation.Abgeschlossen && x.Aktuell)
            .AsNoTracking()
            .ToListAsync();
        }


        public void SetProgressIsBusy()
        {
            throw new NotImplementedException();
        }
    }
}
