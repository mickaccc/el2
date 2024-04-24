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
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace ModuleMeasuring.ViewModels
{
    class MeasuringDocumentsViewModel : ViewModelBase, IDropTarget
    {
        public string Title { get; } = "Messdokumente";
        private MeasureFirstPartInfo FirstPartInfo { get; set; }
        private VmpbDocumentInfo VmpbInfo { get; set; }
        public MeasuringDocumentsViewModel(IContainerExtension container)
        {
            _container = container;
            VmpbCommand = new ActionCommand(onVmpbExecuted, onVmpbCanExecute);
            PruefDataCommand = new ActionCommand(onPruefExecuted, onPruefCanExecute);
            OpenFileCommand = new ActionCommand(onOpenFileExecuted, onOpenFileCanExecute);
            LoadData();
            FirstPartInfo = new MeasureFirstPartInfo(_container);
            VmpbInfo = new VmpbDocumentInfo(_container);
        }

        IContainerExtension _container;
        public ICommand? VmpbCommand { get; private set; }
        public ICommand? PruefDataCommand { get; private set; }
        public ICommand? OpenFileCommand { get; private set; }
        private List<OrderRb> _orders;
        public ICollectionView OrderList { get { return orderViewSource.View; } }
        private CollectionViewSource orderViewSource { get; } = new();
        private ObservableCollection<DocumentDisplay> _FirstDocumentItems = [];
        public ICollectionView FirstDocumentItems { get; private set; }
        private ObservableCollection<DocumentDisplay> _VmpbDocumentItems = [];
        public ICollectionView VmpbDocumentItems { get; private set; }
        private ObservableCollection<DocumentDisplay> _PartDocumentItems = [];
        public ICollectionView PartDocumentItems { get; private set; }
        private OrderRb _SelectedItem;

        public OrderRb SelectedItem
        {
            get
            {
                return _SelectedItem;
            }
            set
            {
                if (value != _SelectedItem)
                {
                    _SelectedItem = value;
                    NotifyPropertyChanged(() => SelectedItem);                   
                }
            }
        }
        private string _SelectedValue;

        public string? SelectedValue
        {
            get
            {
                return _SelectedValue;
            }
            set
            {
                if (value != _SelectedValue)
                {
                    _SelectedValue = value;
                    NotifyPropertyChanged(() => SelectedValue);
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
            OrderList.CollectionChanged += OnOrderChanged;
            FirstDocumentItems = CollectionViewSource.GetDefaultView(_FirstDocumentItems);
            VmpbDocumentItems = CollectionViewSource.GetDefaultView(_VmpbDocumentItems);
            PartDocumentItems = CollectionViewSource.GetDefaultView(_PartDocumentItems);
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
            var mes = _orders.First(x => x.Aid == _SelectedValue);

    
            var docu = FirstPartInfo.CreateDocumentInfos([mes.Material]);
            FirstPartInfo.Collect();
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
            try
            {
                var mes = _orders.First(x => x.Aid == _SelectedValue);
                var oa = new string[] { mes.Material, mes.Aid };
                var size = (string)obj;
      
                var docu = VmpbInfo.CreateDocumentInfos(oa);
                VmpbInfo.Collect();
                FileInfo vmFile;
                switch (size)
                {
                    case "size1":
                        vmFile = new FileInfo(docu[DocumentPart.Template]);
                        break;
                    case "size2":
                        vmFile = new FileInfo(docu[DocumentPart.Template_Size2]);
                        break;
                    case "size3":
                        vmFile = new FileInfo(docu[DocumentPart.Template_Size3]);
                        break;
                    default:
                        throw new NotImplementedException();
                }
                var vmtarg = new FileInfo(docu[DocumentPart.File]);
                if (!vmtarg.Exists)
                    File.Copy(vmFile.FullName, vmtarg.FullName);

                _VmpbDocumentItems.Clear();
                foreach (var d in vmtarg.Directory.GetFiles())
                {
                    _VmpbDocumentItems.Add(new DocumentDisplay() { FullName = d.FullName, Display = d.Name });
                }
            }
            catch (NotImplementedException)
            {
                MessageBox.Show("Keine Vorlage definiert", "Vormusterprüfbericht", MessageBoxButton.OK);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Vmpb", MessageBoxButton.OK, MessageBoxImage.Error);
            }
 
        }
        private void OnOrderChanged(object? sender, EventArgs e)
        {

            if (SelectedItem != null)
            {

                
                var docu = FirstPartInfo.CreateDocumentInfos([SelectedItem.Material]);
                string path = Path.Combine(docu[DocumentPart.RootPath], docu[DocumentPart.SavePath]);
                if (Directory.Exists(path))
                {
                    foreach (var d in Directory.GetFiles(path))
                    {
                        FileInfo f = new FileInfo(d);
                        _FirstDocumentItems.Add(new DocumentDisplay() { FullName = f.FullName, Display = f.Name });
                    }
                }

                
                var vmdocu = VmpbInfo.CreateDocumentInfos([SelectedItem.Material, SelectedItem.Aid]);
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
            else
            {
                _FirstDocumentItems.Clear();
                _VmpbDocumentItems.Clear();
                _PartDocumentItems.Clear();
            }
        }
        public void DragOver(IDropInfo dropInfo)
        {
            if (PermissionsProvider.GetInstance().GetUserPermission(Permissions.AddPruefDoc) && SelectedItem != null)
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
                
                if (o.Length > 0 && SelectedItem != null)
                {
                    var t = (ItemsControl)dropInfo.VisualTarget;
                    if (t.Name == "first")
                    {
                        var docu = FirstPartInfo.CreateDocumentInfos([SelectedItem.Material]);
                        FirstPartInfo.Collect();
                        FileInfo source = new FileInfo(o[0]);
                        var target = new FileInfo(Path.Combine(docu[DocumentPart.RootPath], docu[DocumentPart.SavePath], source.Name));
                        File.Copy(source.FullName, target.FullName);
                        _FirstDocumentItems.Add(new DocumentDisplay() { FullName = target.FullName, Display = target.Name }); 
                    }
                    if (t.Name == "vmpb")
                    {
                        var docu = VmpbInfo.CreateDocumentInfos([SelectedItem.Material, SelectedItem.Aid]);
                        VmpbInfo.Collect();
                        FileInfo source = new FileInfo(o[0]);
                        var target = new FileInfo(Path.Combine(docu[DocumentPart.RootPath], docu[DocumentPart.SavePath], source.Name));
                        File.Copy(source.FullName, target.FullName);
                        _VmpbDocumentItems.Add(new DocumentDisplay() { FullName = target.FullName, Display = target.Name });
                    }
                    if (t.Name == "part")
                    {
                        var docu = VmpbInfo.CreateDocumentInfos([SelectedItem.Material, SelectedItem.Aid]);
                        VmpbInfo.Collect();
                        FileInfo source = new FileInfo(o[0]);
                        var target = new FileInfo(Path.Combine(docu[DocumentPart.RootPath], docu[DocumentPart.SavePath], "Messen", source.Name));
                        File.Copy(source.FullName, target.FullName);
                        _PartDocumentItems.Add(new DocumentDisplay() { FullName = target.FullName, Display = target.Name });
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
