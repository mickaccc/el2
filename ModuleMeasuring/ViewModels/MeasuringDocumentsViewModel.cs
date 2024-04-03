using El2Core.Constants;
using El2Core.Models;
using El2Core.Utils;
using El2Core.ViewModelBase;
using GongSolutions.Wpf.DragDrop;
using Lieferliste_WPF.Planning;
using Microsoft.EntityFrameworkCore;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace ModuleMeasuring.ViewModels
{
    class MeasuringDocumentsViewModel : ViewModelBase, IDropTarget
    {
        public MeasuringDocumentsViewModel(IContainerExtension container)
        {
            _container = container;
            VmpbCommand = new ActionCommand(onVmpbExecuted, onVmpbCanExecute);
            PruefDataCommand = new ActionCommand(onPruefExecuted, onPruefCanExecute);
            LoadData();
           orderViewSource.Filter += onFilterPredicate;
        }

        IContainerExtension _container;
        public ICommand? VmpbCommand { get; private set; }
        public ICommand? PruefDataCommand { get; private set; }
        private RelayCommand? _searchChanged;
        public RelayCommand SearchChangedCommand => _searchChanged ??= new RelayCommand(onSearchChanged);
        private string _rootPath;
        private List<OrderRb> _orders;
        public ICollectionView OrderList { get { return orderViewSource.View; } }
        private CollectionViewSource orderViewSource { get; } = new();
        public string OrderSearch { get; set; }
        private void LoadData()
        {
            using var db = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
            _rootPath = db.Rules.Single<Rule>(x => x.RuleName == "ROOT").RuleValue;
            _orders = new();
            var ord = db.OrderRbs
                .Include(x => x.MaterialNavigation)
                .Include(x => x.DummyMatNavigation)
                .Where(x => x.Abgeschlossen == false);
            _orders.AddRange(ord);
            orderViewSource.Source = _orders;
        }
        private void onSearchChanged(object obj)
        {
            OrderList.Refresh();
        }
        private bool onPruefCanExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission(Permissions.AddPruefDoc);
        }

        private void onPruefExecuted(object obj)
        {
            throw new NotImplementedException();
        }
        private bool onVmpbCanExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission(Permissions.AddVmpb);
        }
        private void onVmpbExecuted(object obj)
        {
            throw new NotImplementedException();
        }
        private void onFilterPredicate(object sender, FilterEventArgs e)
        {
            OrderRb ord = (OrderRb)e.Item;
            e.Accepted = ord.Aid.Contains(OrderSearch, StringComparison.OrdinalIgnoreCase);
        }
        public void DragOver(IDropInfo dropInfo)
        {
            if (PermissionsProvider.GetInstance().GetUserPermission(Permissions.AddPruefDoc))
            {
                if (dropInfo.Data is PlanMachine)
                {
                    dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                    dropInfo.Effects = DragDropEffects.All;
                }
            }
        }

        public void Drop(IDropInfo dropInfo)
        {
            FileInfo file = dropInfo.Data as FileInfo;
        }
    }
}
