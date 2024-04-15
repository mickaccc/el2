using El2Core.Constants;
using El2Core.Converters;
using El2Core.Models;
using El2Core.Utils;
using El2Core.ViewModelBase;
using GongSolutions.Wpf.DragDrop;
using Microsoft.EntityFrameworkCore;
using Microsoft.Office.Interop.Excel;
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
            documentManager.Construct(builder, mes.Material);
            var target = documentManager.Collect();
            FileInfo file = new FileInfo(builder.Document[DocumentPart.Template]);
            var targ = Path.Combine(target, file.Name.Replace("Messblatt", mes.Material));
            File.Copy(file.FullName, targ);
        
            Microsoft.Office.Interop.Excel.Application excel = new();
            Microsoft.Office.Interop.Excel.Workbook wb = excel.Workbooks.Open(targ, ReadOnly: false, Editable: true);
            Worksheet worksheet = wb.Worksheets.Item[1] as Worksheet;
            if (worksheet != null )
            {
                Microsoft.Office.Interop.Excel.Range row1 = worksheet.Rows.Cells[3, 3];
                Microsoft.Office.Interop.Excel.Range row2 = worksheet.Rows.Cells[3, 10];
                Microsoft.Office.Interop.Excel.Range row3 = worksheet.Rows.Cells[4, 3];

                row1.Value = mes.MaterialNavigation.Bezeichng;
                row2.Value = mes.Material;
                row3.Value = mes.Aid;
                excel.ActiveWorkbook.Save();
                excel.Quit();
            }
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
