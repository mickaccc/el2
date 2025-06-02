using CompositeCommands.Core;
using El2Core.Constants;
using El2Core.Models;
using El2Core.Utils;
using El2Core.ViewModelBase;
using Microsoft.EntityFrameworkCore;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace Lieferliste_WPF.ViewModels
{
    [System.Runtime.Versioning.SupportedOSPlatform("windows10.0")]
    internal class ArchiveViewModel : ViewModelBase
    {
        private IContainerExtension _container;
        private IApplicationCommands _applicationCommands;
        public IApplicationCommands ApplicationCommands
        {
            get { return _applicationCommands; }
            set
            {
                if (_applicationCommands != null)
                {
                    _applicationCommands = value;
                    NotifyPropertyChanged(() => ApplicationCommands);
                }
            }
        }
        private NotifyTaskCompletion<ICollectionView>? _contentTask;
        public string Title { get; } = "Archiv";
        private string? _searchValue;
        private readonly ConcurrentObservableCollection<OrderRb> result = [];
        public ICollectionView? CollectionView { get; private set; }
        private MeasureDocumentInfo MeasureInfo { get; set; }
        public NotifyTaskCompletion<ICollectionView>? ContentTask
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
        public ICommand RetriveCommand { get; set; }
        public ICommand ArchivateCommand { get; set; }
        private RelayCommand? _textSearchCommand;
        public ICommand TextSearchCommand => _textSearchCommand ??= new RelayCommand(OnTextSearch);

        private void OnTextSearch(object obj)
        {
            _searchValue = (string)obj;
            CollectionView?.Refresh();
        }

        public ArchiveViewModel(IContainerExtension container, IApplicationCommands applicationCommands)
        {
            _container = container;
            _applicationCommands = applicationCommands;
            RetriveCommand = new ActionCommand(OnRetriveExecuted, OnRetriveCanExecute);
            ArchivateCommand = new ActionCommand(OnArchivateExecuted, OnArchivateCanExecute);
            MeasureInfo = new MeasureDocumentInfo(container);
            ContentTask = new NotifyTaskCompletion<ICollectionView>(LoadAsync());
        }

        private bool OnArchivateCanExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission(Permissions.Archivate);
        }

        private void OnArchivateExecuted(object obj)
        {
            string path;
            if (RuleInfo.Rules.TryGetValue("MeasureArchivFolder", out Rule? archiv))
            {
                path = archiv.RuleValue;

                foreach (var col in CollectionView)
                {
                    var order = (OrderRb)col;
                    if (order.Material != null)
                    {
                        MeasureInfo.CreateDocumentInfos([order.Material, order.Aid]);
                    }
                }
            }
        }

        private bool OnRetriveCanExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission(Permissions.RetriveOrder);
        }

        private void OnRetriveExecuted(object obj)
        {
            if (obj is OrderRb o)
            {
                o.Abgeschlossen = false;
                using var db = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
                db.Update(o);
                db.SaveChangesAsync();
                result.Remove(o);
                CollectionView?.Refresh();
            }
        }

        private async Task<ICollectionView> LoadAsync()
        {
            var uiContext = TaskScheduler.FromCurrentSynchronizationContext();
            using (var db = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>())
            {
                var ord = await db.OrderRbs.AsNoTracking()
                    .Include(x => x.MaterialNavigation)
                    .Include(x => x.DummyMatNavigation)
                    .Include(x => x.Pro)
                    .Include(x => x.Vorgangs)
                    .Where(x => x.Abgeschlossen)
                    .ToListAsync();

                await Task.Factory.StartNew(() =>
                {
                    List<OrderRb> o = [];
                    foreach (var item in ord.OrderBy(x => x.Eckende))
                    {
                        var simple = item.Vorgangs.MaxBy(x => x.Vnr);
                        if (simple != null)
                            item.ActualEnd = simple.ActualEndDate;
                        o.Add(item);
                    }
                    result.AddRange(o);
                }, CancellationToken.None, TaskCreationOptions.RunContinuationsAsynchronously, uiContext);
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
                        ((o.Material != null) && o.Material.Contains(_searchValue, System.StringComparison.CurrentCultureIgnoreCase)) ||
                        ((o.MaterialNavigation != null) && o.MaterialNavigation.Ttnr.Contains(_searchValue, System.StringComparison.CurrentCultureIgnoreCase)) ||
                        ((o.MaterialNavigation?.Bezeichng != null) && o.MaterialNavigation.Bezeichng.Contains(_searchValue, System.StringComparison.CurrentCultureIgnoreCase)) ||
                        ((o.ProId != null) && o.ProId.Contains(_searchValue, System.StringComparison.CurrentCultureIgnoreCase));
                }
            }
            return ret;
        }
    }
}
