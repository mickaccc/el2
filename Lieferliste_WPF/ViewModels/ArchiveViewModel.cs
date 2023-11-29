﻿using CompositeCommands.Core;
using El2Core.Constants;
using El2Core.Models;
using El2Core.Utils;
using El2Core.ViewModelBase;
using Microsoft.EntityFrameworkCore;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Lieferliste_WPF.ViewModels
{
    
    internal class ArchiveViewModel: ViewModelBase
    {
        IContainerExtension _container;
        private NotifyTaskCompletion<ICollectionView> _contentTask;
        public string Title { get; } = "Archiv";
        private string _searchValue;
        private readonly ConcurrentObservableCollection<OrderRb> result = [];
        public ICollectionView CollectionView { get; set; }
        public NotifyTaskCompletion<ICollectionView> ContentTask
        {
            get
            {
                return _contentTask;
            }
            set
            {
                if (value != _contentTask)
                {
                    _contentTask = value;
                    NotifyPropertyChanged(() => ContentTask);
                }
            }
        }
        public ICommand DeArchivateCommand { get; set; }
        private RelayCommand? _textSearchCommand;
        public ICommand TextSearchCommand => _textSearchCommand ??= new RelayCommand(OnTextSearch);

        private void OnTextSearch(object obj)
        {
            _searchValue = (string)obj;
            CollectionView.Refresh();
        }

        public ArchiveViewModel(IContainerExtension container)
        {
            _container = container;
            DeArchivateCommand = new ActionCommand(OnDeArchivateExecuted, OnDeArchivateCanExecute);
            ContentTask = new NotifyTaskCompletion<ICollectionView>(LoadAsync());
        }

        private bool OnDeArchivateCanExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission(Permissions.DeArchivate);
        }

        private void OnDeArchivateExecuted(object obj)
        {
            if (obj is OrderRb o)
            {
                o.Abgeschlossen = false;
                using var db = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
                db.Update(o);
                db.SaveChangesAsync();
                result.Remove(o);
                CollectionView.Refresh();
            }
        }

        private async Task<ICollectionView> LoadAsync()
        {
            
            using (var db = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>())
            {
                var ord = await db.OrderRbs.AsNoTracking()
                    .Include(x => x.MaterialNavigation)
                    .Include(x => x.DummyMatNavigation)
                    .Where(x => x.Abgeschlossen)
                    .ToListAsync();
                result.AddRange(ord);
            }
   
            CollectionView = CollectionViewSource.GetDefaultView(result);
            CollectionView.Filter += OnFilter;
            return CollectionView;
        }

        private bool OnFilter(object obj)
        {
            bool ret = true;
            if (!string.IsNullOrEmpty(_searchValue))
            {
                if (obj is OrderRb o)
                {
                    ret = o.Aid.Contains(_searchValue) ||
                        ((o.Material != null) && o.Material.Contains(_searchValue)) ||
                        ((o.MaterialNavigation != null) && o.MaterialNavigation.Ttnr.Contains(_searchValue));
                }
            }
            return ret;
        }
    }
}
