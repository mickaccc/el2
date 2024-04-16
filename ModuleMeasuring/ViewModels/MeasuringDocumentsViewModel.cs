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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace ModuleMeasuring.ViewModels
{
    class MeasuringDocumentsViewModel : ViewModelBase, IDropTarget
    {
        public string Title { get; } = "Messdokumente";
        public MeasuringDocumentsViewModel(IContainerExtension container)
        {
            _container = container;
            VmpbCommand = new ActionCommand(onVmpbExecuted, onVmpbCanExecute);
            PruefDataCommand = new ActionCommand(onPruefExecuted, onPruefCanExecute);
            OpenFileCommand = new ActionCommand(onOpenFileExecuted, onOpenFileCanExecute);
            LoadData();
            orderViewSource.Filter += onFilterPredicate;
            documentManager = _container.Resolve<DocumentManager>();
        }

        IContainerExtension _container;
        public ICommand? VmpbCommand { get; private set; }
        public ICommand? PruefDataCommand { get; private set; }
        public ICommand? OpenFileCommand { get; private set; }
        private RelayCommand? _searchChanged;
        public RelayCommand SearchChangedCommand => _searchChanged ??= new RelayCommand(onSearchChanged);
        private RelayCommand? _selectionCommand;
        public RelayCommand SelectionCommand => _selectionCommand ??= new RelayCommand(onSelectionChanged);
        private List<OrderRb> _orders;
        public ICollectionView OrderList { get { return orderViewSource.View; } }
        private CollectionViewSource orderViewSource { get; } = new();
        private ObservableCollection<DocumentDisplay> _MaterialItems = [];
        public ICollectionView MaterialItems { get; private set; }
        private DocumentManager documentManager;
        private string _orderSearch;
        public string OrderSearch
        {
            get { return _orderSearch; }
            set
            {
                if (_orderSearch != value)
                {
                    _orderSearch = value;
                    NotifyPropertyChanged(() => OrderSearch);
                }
            }
        }
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

            MaterialItems = CollectionViewSource.GetDefaultView(_MaterialItems);
            OrderList.CollectionChanged += OnOrderChanged;

        }

        private void OnOrderChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            var list = sender as ListCollectionView;
            if (list != null)
            {
                if (list.Count == 1)
                {
                    if (list.CurrentItem is OrderRb o)
                    {
                        DocumentBuilder builder = new MeasureFirstPartBuilder();
                        documentManager.Construct(builder, o.Material);
                        if (Directory.Exists(builder.Document[DocumentPart.SavePath]))
                        {
                            foreach(var d in Directory.GetFiles(builder.Document[DocumentPart.SavePath]))
                            {
                                FileInfo f = new FileInfo(d);
                                _MaterialItems.Add(new DocumentDisplay() { FullName = f.FullName, Display = f.Name });
                            }
                            
                        }
                    }
                }
                else
                {
                    _MaterialItems.Clear();
                }
            }

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
        private void onSelectionChanged(object obj)
        {
            if (obj is OrderRb o)
            {
                OrderSearch = o.Aid;
                OrderList.Refresh();
            }
        }
        private bool onOpenFileCanExecute(object arg)
        {
            return true;
        }

        private void onOpenFileExecuted(object obj)
        {
            if (obj is DocumentDisplay s) new Process() { StartInfo = new ProcessStartInfo(s.FullName) { UseShellExecute = true } }.Start();
        }
        private bool onPruefCanExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission(Permissions.AddPruefDoc);
        }

        private void onPruefExecuted(object obj)
        {
            var mes = _orders.First(x => x.Aid == _orderSearch);

            DocumentBuilder builder = new MeasureFirstPartBuilder();
            documentManager.Construct(builder, mes.Material);
            var target = documentManager.Collect();

            FileInfo file = new FileInfo(builder.Document[DocumentPart.Template]);
            var targ = builder.GetDataSheet();
            if (!targ.Exists)
                File.Copy(file.FullName, targ.FullName);

            Microsoft.Office.Interop.Excel.Application excel = new();
            Workbook wb = excel.Workbooks.Open(targ.FullName, ReadOnly: false, Editable: true);
            Worksheet worksheet = wb.Worksheets.Item[1] as Worksheet;
            if (worksheet != null)
            {
                Microsoft.Office.Interop.Excel.Range row1 = (Microsoft.Office.Interop.Excel.Range)worksheet.Rows.Cells[3, 3];
                Microsoft.Office.Interop.Excel.Range row2 = (Microsoft.Office.Interop.Excel.Range)worksheet.Rows.Cells[3, 10];
                Microsoft.Office.Interop.Excel.Range row3 = (Microsoft.Office.Interop.Excel.Range)worksheet.Rows.Cells[4, 3];

                row1.Value = mes.MaterialNavigation.Bezeichng;
                row2.Value = ConvertToFormat.ConvertTTNR(mes.Material, 4, 3, '.', '-');
                row3.Value = mes.Aid;
                excel.ActiveWorkbook.Save();
                excel.ActiveWorkbook.Close();
                excel.Quit();

                int ws = Marshal.ReleaseComObject(worksheet);
                int wbo = Marshal.ReleaseComObject(wb);
                int ex = Marshal.ReleaseComObject(excel);
            }
            _MaterialItems.Clear();
            foreach(var d in targ.Directory.GetFiles())
            {
                _MaterialItems.Add(new DocumentDisplay() { FullName = d.FullName, Display = d.Name });
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

                dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                dropInfo.Effects = DragDropEffects.All;

            }
        }

        public void Drop(IDropInfo dropInfo)
        {
            if (dropInfo.Data is IDataObject f)
            {
                var o = (string[])f.GetData(DataFormats.FileDrop);
                
                if (o.Length > 0)
                {
                    var builder = documentManager.GetBuilder();
                    if (builder != null)
                    {
                        FileInfo source = new FileInfo(o[0]);
                        var target = new FileInfo(Path.Combine(builder.Document[DocumentPart.SavePath], source.Name));
                        File.Copy(source.FullName, target.FullName);
                        _MaterialItems.Add(new DocumentDisplay() { FullName = target.FullName, Display = target.Name });
                    }
                }
            }
        }
        public struct DocumentDisplay
        {
            public string FullName { get; set; }
            public string Display { get; set; }
        }
    }
}
