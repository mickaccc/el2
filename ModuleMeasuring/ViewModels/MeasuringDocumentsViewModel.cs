using El2Core.Constants;
using El2Core.Models;
using El2Core.Utils;
using El2Core.ViewModelBase;
using GongSolutions.Wpf.DragDrop;
using Microsoft.EntityFrameworkCore;
using Prism.Ioc;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace ModuleMeasuring.ViewModels
{
    class MeasuringDocumentsViewModel : ViewModelBase, IDropTarget
    {
        public string Title { get; } = "Messdokumente";
        private MeasureFirstPartInfo FirstPartInfo { get; set; }
        private DocumentInfo VmpbInfo { get; set; }
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
                        //DocumentBuilder Firstbuilder = new MeasureFirstPartBuilder();
                        //var oa = new string[] { o.Material, o.Aid };
                        //FirstDocumentManager.Construct(Firstbuilder, oa);
                        //if (Directory.Exists(Firstbuilder.Document[DocumentPart.SavePath]))
                        //{
                        //    foreach(var d in Directory.GetFiles(Firstbuilder.Document[DocumentPart.SavePath]))
                        //    {
                        //        FileInfo f = new FileInfo(d);
                        //        _FirstDocumentItems.Add(new DocumentDisplay() { FullName = f.FullName, Display = f.Name });
                        //    }                           
                        //}
                        //DocumentBuilder Vmpbbuilder = new VmpbPartBuilder();
                        //VmpbDocumentManager.Construct(Vmpbbuilder, oa);
                        //if (Directory.Exists(Vmpbbuilder.Document[DocumentPart.SavePath]))
                        //{
                        //    foreach (var d in Directory.GetFiles(Vmpbbuilder.Document[DocumentPart.SavePath]))
                        //    {
                        //        FileInfo f = new FileInfo(d);
                        //        _VmpbDocumentItems.Add(new DocumentDisplay() { FullName = f.FullName, Display = f.Name });
                        //    }
                        FirstPartInfo = new MeasureFirstPartInfo(_container);
                        var docu = FirstPartInfo.CreateDocumentInfos([o.Material]);
                        string path = Path.Combine(docu[DocumentPart.RootPath], docu[DocumentPart.SavePath]);
                        if (Directory.Exists(path))
                        {
                            foreach (var d in Directory.GetFiles(path))
                            {
                                FileInfo f = new FileInfo(d);
                                _FirstDocumentItems.Add(new DocumentDisplay() { FullName = f.FullName, Display = f.Name });
                            }
                        }

                        VmpbInfo = new VmpbDocumentInfo(_container);
                        var vmdocu = VmpbInfo.CreateDocumentInfos([o.Material, o.Aid]);
                        string vmpath = Path.Combine(docu[DocumentPart.RootPath], vmdocu[DocumentPart.SavePath]);
                        if (Directory.Exists(vmpath))
                        {
                            foreach (var d in Directory.GetFiles(vmpath))
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

            //_FirstDocumentItems.Clear();
            //foreach (var d in Firsttarg.Directory.GetFiles())
            //{
            //    _FirstDocumentItems.Add(new DocumentDisplay() { FullName = d.FullName, Display = d.Name });
            //}
            //#endregion
            var f = new MeasureFirstPartInfo(_container);
            var docu = f.CreateDocumentInfos([mes.Material]);
            f.Collect();
            FileInfo Firstfile = new FileInfo(docu[DocumentPart.Template]);
            var Firsttarg = new FileInfo(docu[DocumentPart.File]);
            if (!Firsttarg.Exists)
                File.Copy(Firstfile.FullName, Firsttarg.FullName);

            _FirstDocumentItems.Clear();
            foreach (var d in Firsttarg.Directory.GetFiles())
            {
                _FirstDocumentItems.Add(new DocumentDisplay() { FullName = d.FullName, Display = d.Name });
            }

        }
        private bool onVmpbCanExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission(Permissions.AddVmpb);
        }
        private void onVmpbExecuted(object obj)
        {
            var mes = _orders.First(x => x.Aid == _orderSearch);
            var oa = new string[] { mes.Material, mes.Aid };

            //DocumentBuilder FirstMeaDocBuilder = new MeasureFirstPartBuilder();
            //FirstDocumentManager.Construct(FirstMeaDocBuilder, oa);

            //DocumentBuilder VmpbBuilder = new VmpbPartBuilder();
            //VmpbDocumentManager.Construct(VmpbBuilder, oa);
            //var VmpbTarget = VmpbDocumentManager.Collect();

            //#region VmpbMeasure
            //FileInfo Vmpbfile = new FileInfo(VmpbBuilder.Document[DocumentPart.Template]);
            //var Vmpbtarg = VmpbBuilder.GetDataSheet();
            //if (!Vmpbtarg.Exists)
            //    File.Copy(Vmpbfile.FullName, Vmpbtarg.FullName);

            //_VmpbDocumentItems.Clear();
            //foreach (var d in Vmpbtarg.Directory.GetFiles())
            //{
            //    _VmpbDocumentItems.Add(new DocumentDisplay() { FullName = d.FullName, Display = d.Name });
            //}

            //#endregion

            var vm = new VmpbDocumentInfo(_container);
            var docu = vm.CreateDocumentInfos(oa);
            vm.Collect();
            FileInfo vmfile = new FileInfo(docu[DocumentPart.Template]);
            var vmtarg = new FileInfo(docu[DocumentPart.File]);
            if (!vmtarg.Exists)
                File.Copy(vmfile.FullName, vmtarg.FullName);

            _VmpbDocumentItems.Clear();
            foreach (var d in vmtarg.Directory.GetFiles())
            {
                _VmpbDocumentItems.Add(new DocumentDisplay() { FullName = d.FullName, Display = d.Name });
            }
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
                    //var builder = FirstDocumentManager.GetBuilder();
                    //if (builder != null)
                    //{
                    //    FileInfo source = new FileInfo(o[0]);
                    //    var target = new FileInfo(Path.Combine(FirstPartInfo.Document[DocumentPart.SavePath], source.Name));
                    //    File.Copy(source.FullName, target.FullName);
                    //    _FirstDocumentItems.Add(new DocumentDisplay() { FullName = target.FullName, Display = target.Name });
                    //}
                    var docu = new WorkareaDocumentInfo(_container).CreateDocumentInfos();
                    FileInfo source = new FileInfo(o[0]);
                    var target = new FileInfo(Path.Combine(docu[DocumentPart.SavePath], source.Name));
                    File.Copy(source.FullName, target.FullName);
                    _FirstDocumentItems.Add(new DocumentDisplay() { FullName = target.FullName, Display = target.Name });

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
