﻿using El2Core.Constants;
using El2Core.Models;
using El2Core.Utils;
using El2Core.ViewModelBase;
using GongSolutions.Wpf.DragDrop;
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
        private List<OrderRb> _orders;
        public ICollectionView OrderList { get { return orderViewSource.View; } }
        private CollectionViewSource orderViewSource { get; } = new();
        private string _orderSearch { get; set; }
        private void LoadData()
        {
            using var db = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();

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
            if (obj is string s)
                if (s.Length > 3)
                {
                    _orderSearch = s;
                    OrderList.Refresh();
                }
        }
        private bool onPruefCanExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission(Permissions.AddPruefDoc);
        }

        private void onPruefExecuted(object obj)
        {
            var mes = _orders.First(x => x.Aid == _orderSearch);
            DocumentManager documentManager = _container.Resolve<DocumentManager>();
            DocumentBuilder builder = new MeasureFirstPartBuilder();
            documentManager.Construct(builder, mes.Material, mes.Aid);
            documentManager.Collect();
            //Microsoft.Office.Interop.Excel.Application excel = new();
            
            //Microsoft.Office.Interop.Excel.Workbook wb = excel.Workbooks.Open(builder.Document[DocumentPart.Template]);
            //wb.Activate();
        }
        private bool onVmpbCanExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission(Permissions.AddVmpb);
        }
        private void onVmpbExecuted(object obj)
        {
            
        }
        private void onFilterPredicate(object sender, FilterEventArgs e)
        {
            if (!string.IsNullOrEmpty(_orderSearch))
            {
                OrderRb ord = (OrderRb)e.Item;
                e.Accepted = ord.Aid.Contains(_orderSearch, StringComparison.OrdinalIgnoreCase);
            }
        }
        public void DragOver(IDropInfo dropInfo)
        {
            if (PermissionsProvider.GetInstance().GetUserPermission(Permissions.AddPruefDoc))
            {
                if (dropInfo.Data is File)
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
