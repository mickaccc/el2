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

        private IContainerProvider _container;
        private List<WorkArea> _workAreas = new();
        private DB_COS_LIEFERLISTE_SQLContext _dbctx;
        public ICollectionView WorkAreas;
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
            WaTask = new NotifyTaskCompletion<ICollectionView>(LoadAsync());
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
