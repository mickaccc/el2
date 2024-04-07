
using CompositeCommands.Core;
using El2Core.Constants;
using El2Core.Models;
using El2Core.Services;
using El2Core.Utils;
using El2Core.ViewModelBase;
using GongSolutions.Wpf.DragDrop;
using Microsoft.EntityFrameworkCore;
using ModulePlanning.Planning;
using Prism.Events;
using Prism.Ioc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Unity;


namespace ModulePlanning.ViewModels
{
    [System.Runtime.Versioning.SupportedOSPlatform("windows10.0")]
    internal class MachinePlanViewModel : ViewModelBase, IDropTarget, IViewModel
    {
        public string Title { get; } = "Teamleiter Zuteilung";
        public bool HasChange => Changed();
        private RelayCommand? _selectionChangeCommand;
        private RelayCommand? _textSearchCommand;
        private RelayCommand? _textSearchArbPlCommand;
        public ICommand SelectionChangeCommand => _selectionChangeCommand ??= new RelayCommand(SelectionChange);
        public ICommand TextSearchCommand => _textSearchCommand ??= new RelayCommand(OnTextSearch);
        public ICommand TextSearchArbPlCommand => _textSearchArbPlCommand ??= new RelayCommand(OnTextSearchArbPl);
        public ICommand SaveCommand { get; private set; }
        public List<WorkArea> WorkAreas { get; } = [];
        private ConcurrentObservableCollection<PlanMachine> _machines { get; } = new();
        private ObservableCollection<Vorgang>? Priv_processes { get; set; }
        private ObservableCollection<Vorgang>? Priv_parking { get; set; }
        private DB_COS_LIEFERLISTE_SQLContext _DbCtx;
        private IContainerExtension _container;
        private IEventAggregator _ea;
        private IUserSettingsService _settingsService;
        public ICollectionView? RessCV { get; private set; }
        public ICollectionView ProcessCV { get { return ProcessViewSource.View; } }
        public ICollectionView ParkingCV { get { return ParkingViewSource.View; } }
        private IApplicationCommands _applicationCommands;
        private static System.Timers.Timer? _timer;
        private static System.Timers.Timer? _autoSaveTimer;
        private NotifyTaskCompletion<ICollectionView>? _machineTask;

        public NotifyTaskCompletion<ICollectionView>? MachineTask
        {
            get { return _machineTask; }
            set
            {
                if (_machineTask != value)
                {
                    _machineTask = value;
                    NotifyPropertyChanged(() => MachineTask);
                }
            }
        }

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

        private int _currentWorkArea;
        private string? _searchFilterText;
        private readonly object _lock = new();
        private string? _searchFilterTextArbPl;
        private List<Vorgang> _processAll = [];
        internal CollectionViewSource ProcessViewSource { get; } = new();
        internal CollectionViewSource ParkingViewSource { get; } = new();

        public MachinePlanViewModel(IContainerExtension container, IApplicationCommands applicationCommands, IEventAggregator ea, IUserSettingsService settingsService)
        {
            _container = container;
            _applicationCommands = applicationCommands;
            _ea = ea;
            _settingsService = settingsService;
            _DbCtx = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
            SaveCommand = new ActionCommand(OnSaveExecuted, OnSaveCanExecute);

            LoadWorkAreas();
            MachineTask = new NotifyTaskCompletion<ICollectionView>(LoadMachinesAsync());
            _ea.GetEvent<MessageOrderChanged>().Subscribe(MessageOrderReceived);
            _ea.GetEvent<MessageVorgangChanged>().Subscribe(MessageVorgangReceived);
            if (_settingsService.IsAutoSave) SetAutoSaveTimer();

        }

        private void MessageVorgangReceived(List<string?> list)
        {
            try
            {
                
                foreach (var item in list)
                {
                    if ((item != null))
                    {
                        var vo = _processAll.FirstOrDefault(x => x.VorgangId == item);
                        
                        if (vo != null)
                        {
                            _DbCtx.Entry(vo).Reload();
                            if (vo.Rid != null)
                            {
                                Application.Current.Dispatcher.InvokeAsync(() => Priv_processes?.Remove(vo));
                                _DbCtx.ChangeTracker.Entries<Vorgang>().First(x => x.Entity.VorgangId == vo.VorgangId).State = EntityState.Unchanged;
                            }
                            else
                            {
                                Application.Current.Dispatcher.InvokeAsync(() => Priv_processes?.Add(vo));
                                _DbCtx.ChangeTracker.Entries<Vorgang>().First(x => x.Entity.VorgangId == vo.VorgangId).State = EntityState.Unchanged;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "MessageVorgangReceved MachinePlan", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MessageOrderReceived(List<string?> list)
        {
            try
            {
                foreach (var item in list)
                {
                    if (Priv_processes?.Any(x => x.Aid == item) ?? false)
                    {
                        Task.Factory.StartNew(async () =>
                        {
                            ///todo///
                            ///Add New Order
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "MessageOrderReceived MachinePlan", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
 
        private bool OnSaveCanExecute(object arg)
        {
            try
            {
                return Changed();
            }
            catch (InvalidOperationException e)
            {
                MessageBox.Show(e.Message, "SaveCanExecute", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "SaveCanExecute", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return false;
        }

        private void OnSaveExecuted(object obj)
        {
            try
            {
                _DbCtx.SaveChanges();
               foreach (var mach in _machines.Where(x => x.HasChange)) { mach.SaveAll(); }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "OnSave MachPlan", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadWorkAreas()
        {
            var w = UserInfo.User.UserWorkAreas.OrderByDescending(x => x.Standard);
            foreach (var workArea in w)
            {
                WorkAreas.Add(workArea.WorkArea);
            }
        }
        private async Task<List<Vorgang>> GetVorgangsAsync(string? aid)
        {

            var query = await _DbCtx.Vorgangs
              .Include(x => x.AidNavigation)
              .ThenInclude(x => x.MaterialNavigation)
              .Include(x => x.AidNavigation.DummyMatNavigation)
              .Include(x => x.ArbPlSapNavigation)
              .Include(x => x.RidNavigation)
              .ThenInclude(x => x.RessourceWorkshifts)
              .ThenInclude(x => x.SidNavigation)
              .Where(y => y.AidNavigation.Abgeschlossen == false
                && y.SysStatus != null
                && y.Text != null
                && y.ArbPlSapNavigation.Ressource.WorkAreaId != 5
                && y.Text.ToLower().Contains("starten") == false
                && y.SysStatus.Contains("RÜCK") == false)
              .ToListAsync();

            var result = new List<Vorgang>();
            if (aid != null && query != null)
                result.AddRange(query.Where(x => x.Aid == aid).ToList());
            else if (query != null) result.AddRange(query);
            return result;
        }

        private void SetAutoSaveTimer()
        {
            _autoSaveTimer = new System.Timers.Timer(60000);
            _autoSaveTimer.Elapsed += OnAutoSave;
            _autoSaveTimer.AutoReset = true;
            _autoSaveTimer.Enabled = true;
        }

        private void OnAutoSave(object? sender, ElapsedEventArgs e)
        {
            try
            {
                if (_DbCtx.ChangeTracker.HasChanges()) _DbCtx.SaveChangesAsync();
               // foreach (var mach in _machines.Where(x => x.HasChange)) { mach.SaveAll(); }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "AutoSave MachPlan", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task<ICollectionView?> LoadMachinesAsync()
        {
            var uiContext = TaskScheduler.FromCurrentSynchronizationContext();
            var re = await _DbCtx.Ressources
            .Include(x => x.RessourceCostUnits)
            .Include(x => x.WorkArea)
            .Where(x => (x.WorkArea != null) && WorkAreas.Contains(x.WorkArea)).OrderBy(x => x.Sort).ToListAsync();

            var proc = await GetVorgangsAsync(null);

            await Task.Factory.StartNew(() =>
            {
                HashSet<PlanMachine> result = [];
                lock (_lock)
                {
                    PlanMachineFactory factory = _container.Resolve<PlanMachineFactory>();

                    foreach (var q in re)
                    {
                        result.Add(factory.CreatePlanMachine(q.RessourceId, proc.Where(x => x.Rid == q.RessourceId).ToList()));
                    }
                }
                _machines.AddRange(result);

                List<Vorgang> list = new();
                foreach (PlanMachine m in _machines)
                {
                    foreach (var c in UserInfo.User.UserCosts)
                    {
                        list.AddRange(proc.FindAll(x => x.ArbPlSapNavigation?.RessourceId == m.Rid &&
                            x.ArbPlSapNavigation.CostId == c.CostId));
                    }
                }
                _processAll.AddRange(list);
                Priv_processes = list.FindAll(x => x.Rid == null)
                    .ToObservableCollection();
                Priv_parking = list.FindAll(x => x.Rid < 0)
                    .ToObservableCollection();
                ProcessViewSource.Source = Priv_processes;
                ParkingViewSource.Source = Priv_parking;

                RessCV = CollectionViewSource.GetDefaultView(_machines);
                RessCV.MoveCurrentTo(_machines.First(x => x.WorkArea?.WorkAreaId == WorkAreas[0].WorkAreaId));
                _currentWorkArea = ((PlanMachine)RessCV.CurrentItem).WorkArea?.WorkAreaId ?? 0;

            }, CancellationToken.None, TaskCreationOptions.None, uiContext);

            NotifyPropertyChanged(() => ProcessCV);
            NotifyPropertyChanged(() => ParkingCV);
            if(RessCV != null)
                    RessCV.Filter = f => (f as PlanMachine)?.WorkArea?.WorkAreaId == _currentWorkArea &&
                        (f as PlanMachine).Vis;
            ParkingCV.Filter = f => (f as Vorgang)?.Rid == _currentWorkArea * -1;
            ProcessViewSource.Filter += ProcessCV_Filter;

            return RessCV;
        }
        private void SelectionChange(object commandParameter)
        {

            try
            {
                if (commandParameter is SelectionChangedEventArgs sel)
                {
                    if (sel.AddedItems[0] is WorkArea wa)
                    {
                        _currentWorkArea = wa.WorkAreaId;
                        ProcessCV.Refresh();
                        ParkingCV.Refresh();
                        RessCV?.Refresh();
                    }
                }
            }

            catch (Exception e)
            {
                MessageBox.Show(e.Message, "SelectionChange", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void OnTextSearch(object commandParameter)
        {
            if (commandParameter is string change)
            {
                _searchFilterText = change;
                ProcessCV.Refresh();
                _ea.GetEvent<SearchTextFilter>().Publish(_searchFilterText);
            }
        }
        private void OnTextSearchArbPl(object obj)
        {
            if (obj is string change)
            {
                _searchFilterTextArbPl = change;
                ProcessCV.Refresh();
            }
        }
        private void ProcessCV_Filter(object sender, FilterEventArgs e)
        {
            Vorgang v = (Vorgang)e.Item;
            e.Accepted = false;
            var l = _machines.Where(x => x.WorkArea?.WorkAreaId == _currentWorkArea);
            if (l.Any(x => x.Rid == v.ArbPlSapNavigation?.RessourceId))
            {
                e.Accepted = true;
                if (!string.IsNullOrWhiteSpace(_searchFilterText))
                {
                    if (!(e.Accepted = v.Aid.Contains(_searchFilterText, StringComparison.CurrentCultureIgnoreCase)))
                        if (!(e.Accepted = v.AidNavigation?.Material?.Contains(_searchFilterText, StringComparison.CurrentCultureIgnoreCase) ?? false))
                            e.Accepted = v.AidNavigation?.MaterialNavigation?.Bezeichng?.Contains(_searchFilterText, StringComparison.CurrentCultureIgnoreCase) ?? false;
                }
                else if (string.IsNullOrWhiteSpace(_searchFilterTextArbPl) == false)
                    e.Accepted = v.ArbPlSap?.Contains(_searchFilterTextArbPl, StringComparison.CurrentCultureIgnoreCase) ?? false;
            }
        }
        public void DragOver(IDropInfo dropInfo)
        {           
            if (PermissionsProvider.GetInstance().GetRelativeUserPermission(Permissions.MachDrop, _currentWorkArea))
            {
                if (dropInfo.Data is Vorgang)
                {
                    dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                    dropInfo.Effects = DragDropEffects.Move;
                }
            }
            if (PermissionsProvider.GetInstance().GetRelativeUserPermission(Permissions.MachSort, _currentWorkArea))
            {
                if(dropInfo.Data is PlanMachine)
                {
                    dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                    dropInfo.Effects = DragDropEffects.Move;
                }
            }
        }

        public void Drop(IDropInfo dropInfo)

        {
            string? name = dropInfo.VisualTarget.GetValue(FrameworkElement.NameProperty) as string;
            try
            {
                if (dropInfo.Data is Vorgang vrg)
                {
                    if (name == "parking")
                    {
                        int parkRid = _currentWorkArea * -1;

                        if (_DbCtx.Ressources.All(x => x.RessourceId != parkRid))
                        {
                            _DbCtx.Database.ExecuteSqlRaw(@"SET IDENTITY_INSERT dbo.Ressource ON");
                            _DbCtx.Database.ExecuteSqlRaw(@"INSERT INTO dbo.Ressource(RessourceId) VALUES({0})", parkRid);
                            _DbCtx.Database.ExecuteSqlRaw(@"SET IDENTITY_INSERT dbo.Ressource OFF");
                        }
                        vrg.Rid = parkRid;
                    }
                    else
                    {
                        vrg.Rid = null;
                        _DbCtx.Vorgangs.First(x => x.VorgangId == vrg.VorgangId).Rid = vrg.Rid;
                    }
                    var source = ((ListCollectionView)dropInfo.DragInfo.SourceCollection);
                    if (source.IsAddingNew) { source.CommitNew(); }
                    source.Remove(vrg);
                    ((ListCollectionView)dropInfo.TargetCollection).AddNewItem(vrg);
                    ((ListCollectionView)dropInfo.TargetCollection).CommitNew();
                }
                else if(dropInfo.Data is PlanMachine plm)
                {
                    var s = dropInfo.DragInfo.SourceCollection as ListCollectionView;
                    var t = dropInfo.TargetCollection as ListCollectionView;
                    if (s.CanRemove) s.Remove(plm);
                    var v = dropInfo.InsertIndex;

                        Debug.Assert(t != null, nameof(t) + " != null");
                        ((IList)t.SourceCollection).Insert(v, plm);
                }

                for (var i = 0; i < _machines.Count; i++)
                {
                    var vv = _DbCtx.Ressources.First(x => x.RessourceId == _machines[i].Rid);
                    vv.Sort = (vv.Visability) ? i : 1000;
                }              
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Method Drop", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private bool Changed()
        {
            var ret = _DbCtx.ChangeTracker.HasChanges();
            if (!ret)
            {
                ret = _machines.Any(x => x.HasChange == true);
            }
            return ret;
        }
        public void Closing()
        {
            try
            {
                foreach (var m in _machines)
                {
                    IViewModel vm = (IViewModel)m;
                    vm.Closing();
                }
                if (_DbCtx.ChangeTracker.HasChanges())
                {
                    if (_settingsService.IsSaveMessage)
                    {
                        var result = MessageBox.Show("Sollen die Änderungen in Teamleiter-Zuteilungen gespeichert werden?",
                            Title, MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (result == MessageBoxResult.Yes)
                        {
                            _DbCtx.SaveChanges();
                        }
                    }
                    else _DbCtx.SaveChanges();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Closing", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
