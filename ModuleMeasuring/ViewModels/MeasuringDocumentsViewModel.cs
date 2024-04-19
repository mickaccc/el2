using El2Core.Constants;
using El2Core.Converters;
using El2Core.Models;
using El2Core.Utils;
using El2Core.ViewModelBase;
using GongSolutions.Wpf.DragDrop;
using Microsoft.EntityFrameworkCore;
//using Microsoft.Office.Interop.Excel;
//using Microsoft.Office.Interop.Word;
//using Microsoft.Office.Core;
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
            FirstDocumentManager = _container.Resolve<DocumentManager>();
            VmpbDocumentManager = _container.Resolve<DocumentManager>();
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
        private ObservableCollection<DocumentDisplay> _FirstDocumentItems = [];
        public ICollectionView FirstDocumentItems { get; private set; }
        private ObservableCollection<DocumentDisplay> _VmpbDocumentItems = [];
        public ICollectionView VmpbDocumentItems { get; private set; }
        private DocumentManager FirstDocumentManager;
        private DocumentManager VmpbDocumentManager;
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

            FirstDocumentItems = CollectionViewSource.GetDefaultView(_FirstDocumentItems);
            VmpbDocumentItems = CollectionViewSource.GetDefaultView(_VmpbDocumentItems);
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
                        DocumentBuilder Firstbuilder = new MeasureFirstPartBuilder();
                        var oa = new string[] { o.Material, o.Aid };
                        FirstDocumentManager.Construct(Firstbuilder, oa);
                        if (Directory.Exists(Firstbuilder.Document[DocumentPart.SavePath]))
                        {
                            foreach(var d in Directory.GetFiles(Firstbuilder.Document[DocumentPart.SavePath]))
                            {
                                FileInfo f = new FileInfo(d);
                                _FirstDocumentItems.Add(new DocumentDisplay() { FullName = f.FullName, Display = f.Name });
                            }                           
                        }
                        DocumentBuilder Vmpbbuilder = new VmpbPartBuilder();
                        VmpbDocumentManager.Construct(Vmpbbuilder, oa);
                        if (Directory.Exists(Vmpbbuilder.Document[DocumentPart.SavePath]))
                        {
                            foreach (var d in Directory.GetFiles(Vmpbbuilder.Document[DocumentPart.SavePath]))
                            {
                                FileInfo f = new FileInfo(d);
                                _VmpbDocumentItems.Add(new DocumentDisplay() { FullName = f.FullName, Display = f.Name });
                            }
                        }
                    }
                }
                else
                {
                    _FirstDocumentItems.Clear();
                    _VmpbDocumentItems.Clear();
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

            //#region FirstMeasure
            //DocumentBuilder FirstBuilder = new MeasureFirstPartBuilder();
            //var oa = new string[] { mes.Material, mes.Aid };
            //FirstDocumentManager.Construct(FirstBuilder, oa);
            //var target = FirstDocumentManager.Collect();

            //FileInfo Firstfile = new FileInfo(FirstBuilder.Document[DocumentPart.Template]);
            //var Firsttarg = FirstBuilder.GetDataSheet();
            //if (!Firsttarg.Exists)
            //    File.Copy(Firstfile.FullName, Firsttarg.FullName);

            //Microsoft.Office.Interop.Excel.Application excel = new();
            //Workbook wb = excel.Workbooks.Open(Firsttarg.FullName, ReadOnly: false, Editable: true);
            //Worksheet worksheet = wb.Worksheets.Item[1] as Worksheet;
            //if (worksheet != null)
            //{
            //    Microsoft.Office.Interop.Excel.Range row1 = (Microsoft.Office.Interop.Excel.Range)worksheet.Rows.Cells[3, 3];
            //    Microsoft.Office.Interop.Excel.Range row2 = (Microsoft.Office.Interop.Excel.Range)worksheet.Rows.Cells[3, 10];
            //    Microsoft.Office.Interop.Excel.Range row3 = (Microsoft.Office.Interop.Excel.Range)worksheet.Rows.Cells[4, 3];

            //    row1.Value = mes.MaterialNavigation.Bezeichng;
            //    row2.Value = ConvertToFormat.ConvertTTNR(mes.Material, 4, 3, '.', '-');
            //    row3.Value = mes.Aid;
            //    excel.ActiveWorkbook.Save();
            //    excel.ActiveWorkbook.Close();
            //    excel.Quit();

            //    int ws = Marshal.ReleaseComObject(worksheet);
            //    int wbo = Marshal.ReleaseComObject(wb);
            //    int ex = Marshal.ReleaseComObject(excel);
            //}
            //_FirstDocumentItems.Clear();
            //foreach (var d in Firsttarg.Directory.GetFiles())
            //{
            //    _FirstDocumentItems.Add(new DocumentDisplay() { FullName = d.FullName, Display = d.Name });
            //}
            //#endregion

        }
        private bool onVmpbCanExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission(Permissions.AddVmpb);
        }
        private void onVmpbExecuted(object obj)
        {
            //var mes = _orders.First(x => x.Aid == _orderSearch);
            //var oa = new string[] { mes.Material, mes.Aid };

            //DocumentBuilder FirstMeaDocBuilder = new MeasureFirstPartBuilder();
            //FirstDocumentManager.Construct(FirstMeaDocBuilder, oa);

            //DocumentBuilder VmpbBuilder = new VmpbPartBuilder();
            //VmpbDocumentManager.Construct(VmpbBuilder, oa);
            //var VmpbTarget = VmpbDocumentManager.Collect();

            //Microsoft.Office.Interop.Excel.Application excel = new();
            //Microsoft.Office.Interop.Excel.Workbook wb = excel.Workbooks.Open(FirstMeaDocBuilder.Document[DocumentPart.File], 
            //    ReadOnly: true);
            //Worksheet worksheet = wb.Worksheets.Item[1] as Worksheet;
            //if (worksheet != null)
            //{
            //    var row1 = worksheet.Names;


            //    //row1.Value = mes.MaterialNavigation.Bezeichng;
            //    //row2.Value = ConvertToFormat.ConvertTTNR(mes.Material, 4, 3, '.', '-');
            //    //row3.Value = mes.Aid;
            //    excel.ActiveWorkbook.Close();
            //    excel.Quit();

            //    int ws = Marshal.ReleaseComObject(worksheet);
            //    int wbo = Marshal.ReleaseComObject(wb);
            //    int ex = Marshal.ReleaseComObject(excel);
            //}

            //#region VmpbMeasure
            //FileInfo Vmpbfile = new FileInfo(VmpbBuilder.Document[DocumentPart.Template]);
            //var Vmpbtarg = VmpbBuilder.GetDataSheet();
            //if (!Vmpbtarg.Exists)
            //    File.Copy(Vmpbfile.FullName, Vmpbtarg.FullName);

            //Microsoft.Office.Interop.Word.Application word = new();
            //var doc = word.Documents.Open(Vmpbtarg.FullName, ReadOnly: false);
            //var tab = doc.Tables;
            //var t = tab.Count;
            //var tt = tab[10];
            //var c = tt.Cell(1, 1).Range.Text;
            //doc.Save();
            //doc.Close();
            //word.Quit();


            //_VmpbDocumentItems.Clear();
            //foreach (var d in Vmpbtarg.Directory.GetFiles())
            //{
            //    _VmpbDocumentItems.Add(new DocumentDisplay() { FullName = d.FullName, Display = d.Name });
            //}

            //#endregion
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
                    var builder = FirstDocumentManager.GetBuilder();
                    if (builder != null)
                    {
                        FileInfo source = new FileInfo(o[0]);
                        var target = new FileInfo(Path.Combine(builder.Document[DocumentPart.SavePath], source.Name));
                        File.Copy(source.FullName, target.FullName);
                        _FirstDocumentItems.Add(new DocumentDisplay() { FullName = target.FullName, Display = target.Name });
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
