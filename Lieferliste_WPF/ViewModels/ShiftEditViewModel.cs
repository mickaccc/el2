
using El2Core.Models;
using El2Core.Utils;
using El2Core.ViewModelBase;
using Lieferliste_WPF.Utilities;
using Prism.Ioc;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using System.Xml;

namespace Lieferliste_WPF.ViewModels
{
    internal class ShiftEditViewModel : ViewModelBase
    {
        public ShiftEditViewModel(IContainerExtension container)
        {
            _container = container;
            SaveCommand = new ActionCommand(onSaveExecuted, onSaveCanExecute);
            AddCommand = new ActionCommand(onAddExecuted, onAddCanExecute);
            RemoveCommand = new ActionCommand(onRemoveExecuted, onRemoveCanExecute);

            LoadData();
            ShiftView = CollectionViewSource.GetDefaultView(WorkShiftCollection);
            ShiftView.MoveCurrentToFirst();
            ShiftView.CurrentChanged += onCurrentChanged;
        }

        public string Title { get; } = "Schicht Editor";
        IContainerExtension _container;
        public ObservableCollection<WorkShiftService> WorkShiftCollection { get; private set; } = [];
        public ObservableCollection<DataTable> ShiftDataSets { get; private set; } = [];
        private ObservableCollection<WorkShiftItem> WorkShiftItems = [];
        private bool changed;
        private bool isAdding;

        public bool IsAdding
        {
            get
            {
                return isAdding;
            }
            set
            {
                if (value != isAdding)
                {
                    isAdding = value;
                    NotifyPropertyChanged(() => IsAdding);
                }
            }
        }

        public DataSet dataSet { get; private set; }
        public DataView dataView { get; private set; }
        public ICollectionView ShiftView { get; private set; }
        public ICollectionView ShiftItemsView { get; private set; }
        public string xmlString { get; set; }
        public XmlDataProvider xmlDataProvider { get; private set; }
        public ICommand SaveCommand { get; private set; }
        public ICommand AddCommand { get; private set; }
        public ICommand RemoveCommand { get; private set; }
        private RelayCommand? lostFocusCommand;
        public RelayCommand LostFocusCommand => lostFocusCommand ??= new RelayCommand(onLostFocus);
  
        private void LoadData()
        {
            using var db = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
            var ws = db.WorkShifts.ToList();
            var serializer = XmlSerializerHelper.GetSerializer(typeof(ObservableCollection<WorkShiftItem>));
            foreach (var w in ws)
            {
                using XmlReader reader = XmlReader.Create(new StringReader(w.ShiftDef));
                var wos = new WorkShiftService()
                {
                    ShiftName = w.ShiftName,
                    id = w.Sid,
                    Items = (ObservableCollection<WorkShiftItem>)serializer.Deserialize(reader),
                    Changed = false
                };
                foreach (var item in wos.Items)
                {
                    item.Changed = false;
                }

                WorkShiftCollection.Add(wos);
            }
 
        }

        private void onCurrentChanged(object? sender, EventArgs e)
        {
            var send = sender as ListCollectionView;
            changed = send.IsAddingNew || send.IsEditingItem;
        }

        private bool onSaveCanExecute(object arg)
        {
            return true;
        }

        private void onSaveExecuted(object obj)
        {
            using var db = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
            var wo = db.WorkShifts;
            var serializer = XmlSerializerHelper.GetSerializer(typeof(ObservableCollection<WorkShiftItem>));

            foreach (var w in WorkShiftCollection)
            {
                var sw = new StringWriter();
                if (w.id == 0)
                {
                    serializer.Serialize(sw, w.Items);    
                    var work = new WorkShift() { ShiftName = w.ShiftName, ShiftDef = sw.ToString() };
                    
                    wo.Add(work);
            }
                else if (w.Items.Any(x => x.Changed))
            {
                serializer.Serialize(sw, w.Items);
                    var work = wo.First(x => x.Sid == w.id);
                    work.ShiftName = w.ShiftName;
                    work.ShiftDef = sw.ToString();
                }
                foreach (var item in w.Items.Where(x => x.Changed))
                {
                    item.Changed = false;
                }
            }
            db.SaveChanges();
            changed = false;

        }

        private bool onRemoveCanExecute(object arg)
        {
            return true;
        }

        private void onRemoveExecuted(object obj)
        {
            if(obj is WorkShiftItem wsi)
            {
                ((WorkShiftService)ShiftView.CurrentItem).Items.Remove(wsi);
            }
            if(obj is WorkShiftService ws)
            {
                WorkShiftCollection.Remove(ws);
            }
        }

        private bool onAddCanExecute(object arg)
        {
            bool accept = false;
            var current = (WorkShiftService)ShiftView.CurrentItem;
            if (arg == null)
            {
                accept = string.IsNullOrEmpty(current.ShiftName) == false && current.Items.All(x => x.EndTime != null && x.StartTime != null);
            }
            else if (arg is string and "ITEM")
                accept = current.Items.All(x => x.EndTime != null && x.StartTime != null);
            return accept;
        }

        private void onAddExecuted(object obj)
        {
            IsAdding = true;
            if (obj == null)
            {
                WorkShiftCollection.Add(new WorkShiftService());
                ShiftView.MoveCurrentToLast();
            }
            WorkShiftService e = (WorkShiftService)ShiftView.CurrentItem;
            e.Items.Add(new WorkShiftItem());
            changed = true;
        }
        private void onLostFocus(object obj)
        {
            var e = ShiftView.CurrentItem;
            var v = WorkShiftCollection.Count;
        }
    }
}
