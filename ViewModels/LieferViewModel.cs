
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

namespace Lieferliste_WPF.ViewModels
{
    class LieferViewModel : Base.ViewModelBase
    {


        private IDialogProvider DialogProvider { get; set; }
        public ICollectionView OrdersView { get { return ordersViewSource.View; } }
        public ICommand TextSearchCommand => textSearchCommand ??= new RelayCommand(OnTextSearch);



        private ObservableCollection<Vorgang> _orders { get; set; }
        public ActionCommand SortAscCommand { get; private set; }
        public ActionCommand SortDescCommand { get; private set; }
        public ActionCommand ShowRtbEditor { get; private set; }
        public ActionCommand SaveCommand { get; private set; }

        public String HasMouse { get; set; }
        private static bool isLoaded = false;
        private Dictionary<String, String> _filterCriterias;
        private String _sortField;
        private String _sortDirection;
        private RelayCommand textSearchCommand;
        private string _searchFilterText;
        internal CollectionViewSource ordersViewSource {get; private set;} = new();
        
    public LieferViewModel()
        {

            LoadData();
            //var collectionWrapper = new ObservableCollectionWrapper<object>(collection, this.Dispatcher);
            //var defaultView = CollectionViewSource.GetDefaultView(collectionWrapper);
            //OrdersView = CollectionViewSource.GetDefaultView(_orders);
            //OrdersView.MoveCurrentToFirst();
            ordersViewSource.Filter += OrdersView_Filter;
            ordersViewSource.Source = _orders;
            _filterCriterias = new();
            
            SortAscCommand = new ActionCommand(OnAscSortExecuted, OnAscSortCanExecute);
            SortDescCommand = new ActionCommand(OnDescSortExecuted, OnDescSortCanEcecute);
            //ShowRtbEditor = new ActionCommand(OnShowRtbEditorExecuted, OnSchowRtbEditorCanExecute);
            SaveCommand = new ActionCommand(OnSaveExecuted, OnSaveCanExecute);
            //addFilterCriteria("Ausgebl", "false");
            
        }

        private void OrdersView_Filter(object sender, FilterEventArgs e)
        {
            Vorgang ord = (Vorgang)e.Item;

            e.Accepted = true;

            if (!string.IsNullOrWhiteSpace(_searchFilterText))
            {
                _searchFilterText = _searchFilterText.ToUpper();
                if (!(e.Accepted = ord.Aid.ToUpper().Contains(_searchFilterText)))
                    if (!(e.Accepted = ord.AidNavigation.Material?.ToUpper().Contains(_searchFilterText) ?? false))
                        e.Accepted = ord.AidNavigation.MaterialNavigation?.Bezeichng?.ToUpper().Contains(_searchFilterText) ?? false;
            }
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
        public override void addFilterCriteria(string PropertyName, string CriteriaValue)
        {
            int repeat = 0;

            if (PropertyName == "alle") repeat = 3;

            do
            {
                switch (repeat)
                {
                    case 3: PropertyName = "TTNR"; break;
                    case 2: PropertyName = "Teil"; break;
                    case 1: PropertyName = "Auftrag"; break;
                }
                repeat--;
                if (PropertyName == "TTNR") PropertyName = "Material";
                if (PropertyName == "Teil") PropertyName = "Teil";
                if (PropertyName == "Auftrag") PropertyName = "AID";
                if (_filterCriterias.ContainsKey(PropertyName))
                {
                    _filterCriterias[PropertyName] = CriteriaValue;
                }
                else
                {
                    _filterCriterias.Add(PropertyName, CriteriaValue);
                }

            } while (repeat > 0);

            RaisePropertyChanged("ToolTip");
            OrdersView.Refresh();
        }

        private void OnTextSearch(object commandParameter)
        {
            if (commandParameter is TextChangedEventArgs change)
            {
                TextBox tb = (TextBox)change.OriginalSource;

                _searchFilterText = tb.Text;
                var uiContext = SynchronizationContext.Current;
                uiContext?.Send(x => ordersViewSource.View.Refresh(), null);
            }
        }
        public override void removeFilterCriteria(string PropertyName)
        {
            _filterCriterias.Remove(PropertyName);
            RaisePropertyChanged("ToolTip");
            OrdersView.Refresh();

        }
        private void OnSaveExecuted(object obj)
        {
           Dbctx.SaveChangesAsync();
        }

        private bool OnSaveCanExecute(object arg)
        {
            return Dbctx.ChangeTracker.HasChanges();           
        }

        private bool OnSchowRtbEditorCanExecute(object arg)
        {
            return true;
        }

        private void OnShowRtbEditorExecuted(object obj)
        {
            RichTextEditor w = new RichTextEditor();
            
            w.Show();
        }

        bool OnDescSortCanEcecute(object parameter)
        { 
            return !HasMouse.IsNullOrEmpty();

        }

        private void OnDescSortExecuted(object parameter)
        {

            if (OrdersView != null)
            {
                string v = Translate();


                if (v != String.Empty)
                {
                    
                    ordersViewSource.SortDescriptions.Clear();
                    ordersViewSource.SortDescriptions.Add(new SortDescription(v, ListSortDirection.Descending));
                    var uiContext = SynchronizationContext.Current;
                    uiContext?.Send(x => ordersViewSource.View.Refresh(), null);
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
                    ordersViewSource.SortDescriptions.Clear();
                    ordersViewSource.SortDescriptions.Add(new SortDescription(v, ListSortDirection.Ascending));
                    var uiContext = SynchronizationContext.Current;
                    uiContext?.Send(x => ordersViewSource.View.Refresh(), null);
                }
            }
        }
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
        private void LoadData()
        {
 
            IList<Vorgang>? result =null;
            
                result = Dbctx.Vorgangs
                .Include(m => m.ArbPlSapNavigation)
                .Include(d => d.RidNavigation)
                .Include(v => v.AidNavigation)
                .ThenInclude(x => x.MaterialNavigation)
                .Include(x => x.AidNavigation.DummyMatNavigation)
                .Where(x => x.Aktuell)
                .ToList();

                 _orders = new ObservableCollection<Vorgang>(result);
                
            isLoaded = true;                         
        }
    }
}
