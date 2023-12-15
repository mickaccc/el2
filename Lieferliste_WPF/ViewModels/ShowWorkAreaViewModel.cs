using El2Core.Models;
using El2Core.Utils;
using El2Core.ViewModelBase;
using GongSolutions.Wpf.DragDrop;
using Microsoft.EntityFrameworkCore;
using Prism.Ioc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Lieferliste_WPF.ViewModels
{
    class ShowWorkAreaViewModel : ViewModelBase, IDropTarget
    {
        private string _title = "Bereich Editor";

        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        private bool _editMode;

        public bool EditMode
        {
            get { return _editMode; }
            set
            {
                if (_editMode != value)
                {
                    _editMode = value;
                    NotifyPropertyChanged(() => EditMode);
                }
            }
        }
        private IContainerProvider _container;
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand AddCommand { get; set; }
        private RelayCommand? _endEditCommand;
        public ICommand EndEditCommand => _endEditCommand ??= new RelayCommand(OnEndEdit);

        private List<WorkArea> _workAreas = new();
        private DB_COS_LIEFERLISTE_SQLContext _dbctx;
        public ICollectionView WorkAreas;
        private CollectionViewSource _workAreaCVS = new();
        private NotifyTaskCompletion<ICollectionView> _waTask;
        public NotifyTaskCompletion<ICollectionView> WaTask
        {
            get { return _waTask; }
            set
            {
                if (_waTask != value)
                {
                    _waTask = value;
                    NotifyPropertyChanged(() => WaTask);
                }
            }
        }

        public ShowWorkAreaViewModel(IContainerProvider container)
        {
            
            _container = container;
            _dbctx = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
            EditCommand = new ActionCommand(OnEditExecuted, OnEditCanExecute);
            DeleteCommand = new ActionCommand(OnDeleteExecuted, OnDeleteExecute);
            AddCommand = new ActionCommand(OnAddExecuted, OnAddExecute);
            WaTask = new NotifyTaskCompletion<ICollectionView>(LoadAsync());
        }

        private bool OnAddExecute(object arg)
        {
            return true;
        }

        private void OnAddExecuted(object obj)
        {
            if (obj is WorkArea w)
            {

            }
        }

        private bool OnDeleteExecute(object arg)
        {
            return true;
        }

        private void OnDeleteExecuted(object obj)
        {
            if(obj is WorkArea w)
            {
                _workAreas.Remove(w);
            }
        }

        private bool OnEditCanExecute(object arg)
        {
            return true;
        }

        private void OnEditExecuted(object obj)
        {
            var lcv = WorkAreas as ListCollectionView;
            lcv.EditItem(obj);
            EditMode = !EditMode;
        }
        private void OnEndEdit(object obj)
        {
            EditMode = false;           
        }
 


        private async Task<ICollectionView> LoadAsync()
        {
            var w = await _dbctx.WorkAreas.ToListAsync();
            _workAreas.AddRange(w);
            WorkAreas = CollectionViewSource.GetDefaultView(_workAreas);
            WorkAreas.SortDescriptions.Add(new SortDescription("Sort",ListSortDirection.Ascending));
            return WorkAreas;
        }

        public void DragOver(IDropInfo dropInfo)
        {
            if (EditMode) return;
            if (dropInfo == null) return;
            
            if (dropInfo.DragInfo.SourceCollection.Equals(dropInfo.TargetCollection))
            {
                if (dropInfo.Data is WorkArea)
                {
                    dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                    dropInfo.Effects = DragDropEffects.Move;
                } 
            }
        }

        public void Drop(IDropInfo dropInfo)
        {

            if (dropInfo.Data is WorkArea w)
            {
                var s = dropInfo.DragInfo.SourceCollection as ListCollectionView;
                if (s.CanRemove) s.Remove(w);
                var t = dropInfo.TargetCollection as ListCollectionView;
                var src = t?.SourceCollection as List<WorkArea>;
                int ind = dropInfo.InsertIndex;
                if (ind > t?.Count)
                {
                    ((IList)t.SourceCollection).Add(w);
                }
                else
                {
                    Debug.Assert(t != null, nameof(t) + " != null");
                    ((IList)t.SourceCollection).Insert(ind, w);
                }
                if (src != null)
                {
                    byte i = 1;
                    foreach (var wa in src)
                    {
                        wa.Sort = i;
                        i++;
                    }
                }
                WorkAreas.Refresh();
                _dbctx.SaveChanges();
            }
        }
    }
}
