
using CompositeCommands.Core;
using El2Core.Models;
using El2Core.Utils;
using El2Core.ViewModelBase;
using GongSolutions.Wpf.DragDrop;
using Lieferliste_WPF.Planning;
using Lieferliste_WPF.Utilities;
using Microsoft.EntityFrameworkCore;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Lieferliste_WPF.ViewModels
{
    [System.Runtime.Versioning.SupportedOSPlatform("windows7.0")]
    internal class MachinePlanViewModel : ViewModelBase, IDropTarget
    {
        public string Title { get; } = "Teamleiter Zuteilung";
        private RelayCommand? _selectionChangeCommand;
        private RelayCommand? _textSearchCommand;
        public ICommand SelectionChangeCommand => _selectionChangeCommand ??= new RelayCommand(SelectionChange);
        public ICommand TextSearchCommand => _textSearchCommand ??= new RelayCommand(OnTextSearch);

        public ICommand SaveCommand { get; private set; }

        public List<WorkArea> WorkAreas { get; } = [];
        private ConcurrentObservableCollection<PlanMachine> _machines { get; } = new();
        private ObservableCollection<Vorgang>? Priv_processes { get; set; }
        private ObservableCollection<Vorgang>? Priv_parking { get; set; }
        private DB_COS_LIEFERLISTE_SQLContext _DbCtx;
        private IContainerExtension _container;

        public ICollectionView RessCV { get; private set; }
        public ICollectionView ProcessCV { get { return ProcessViewSource.View; } }
        public ICollectionView ParkingCV { get { return ParkingViewSource.View; } }
        private IApplicationCommands _applicationCommands;

        private NotifyTaskCompletion<ICollectionView> _machineTask;

        public NotifyTaskCompletion<ICollectionView> MachineTask
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
        private List<Vorgang> _processesAll;
        internal CollectionViewSource ProcessViewSource { get; } = new();
        internal CollectionViewSource ParkingViewSource { get; } = new();

        public MachinePlanViewModel(IContainerExtension container, IApplicationCommands applicationCommands)
        {
            _container = container;
            _applicationCommands = applicationCommands;
            _DbCtx = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
            SaveCommand = new ActionCommand(OnSaveExecuted, OnSaveCanExecute);

            LoadWorkAreas();
            MachineTask = new NotifyTaskCompletion<ICollectionView>(LoadMachinesAsync());

        }

        private bool OnSaveCanExecute(object arg)
        {
            try
            {
                return _DbCtx.ChangeTracker.HasChanges();
            }
            catch (InvalidOperationException e)
            {
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message,"SaveCanExecute",MessageBoxButton.OK,MessageBoxImage.Error);
            }
            return false;
        }

        private void OnSaveExecuted(object obj)
        {
            _DbCtx.SaveChanges();
        }

        private void LoadWorkAreas()
        {

            var work = _DbCtx.WorkAreas
                .Include(x => x.UserWorkAreas)
                .Where(x => x.UserWorkAreas.Any(x => x.UserId.Equals(UserInfo.User.UserIdent)));

            WorkAreas.AddRange(work);

        }
        private async Task<List<Vorgang>> GetVorgangsAsync()
        {
            if (_processesAll == null)
            {
                var query = await _DbCtx.Vorgangs
                  .Include(x => x.AidNavigation)
                  .ThenInclude(x => x.MaterialNavigation)
                  .Include(x => x.AidNavigation.DummyMatNavigation)
                  .Include(x => x.ArbPlSapNavigation)
                  .Include(x => x.RidNavigation)
                  .Where(y => y.AidNavigation.Abgeschlossen == false
                    && y.SysStatus != null
                    && y.SysStatus.Contains("RÜCK") == false
                    && y.Text != null
                    && y.Text.ToLower().Contains("starten") == false)
                  .ToListAsync();
                _processesAll = query;
            }
            return _processesAll;
        }

        private async Task<ICollectionView> LoadMachinesAsync()
        {
            var uiContext = TaskScheduler.FromCurrentSynchronizationContext();
            var re = await _DbCtx.Ressources
            .Include(x => x.RessourceCostUnits)
            .Include(x => x.WorkArea)
            .Where(x => (x.WorkArea != null) && WorkAreas.Contains(x.WorkArea)).ToListAsync();

            var proc = await GetVorgangsAsync();

            await Task.Factory.StartNew(() =>
            {
                HashSet<PlanMachine> result = [];
                lock (_lock)
                {

                    foreach (var q in re)
                    {

                        PlanMachine plm = new(q.RessourceId, q.RessName ?? String.Empty, q.Inventarnummer ?? String.Empty)
                        {
                            WorkArea = q.WorkArea,
                            CostUnits = q.RessourceCostUnits.Select(x => x.CostId).ToArray(),
                            Description = q.Info ?? String.Empty,
                            ApplicationCommands = _applicationCommands,
                            Vis = q.Visability
                        };

                        List<Vorgang> VrgList = proc.FindAll(x => x.Rid == q.RessourceId);

                        foreach (var vrg in VrgList)
                        {
                            if (vrg.VorgangId.Length > 0)
                                plm.Processes?.Add(vrg);
                        }

                        result.Add(plm);
                    }
                }
                _machines.AddRange(result);

                List<Vorgang> list = new();
                foreach (PlanMachine m in _machines)
                {
                    foreach (var c in UserInfo.User.UserCosts)
                    {
                        list.AddRange(proc.FindAll(x => x.ArbPlSapNavigation?.RessourceId == m.RID &&
                            x.ArbPlSapNavigation.CostId == c.CostId));
                    }
                }
            
            Priv_processes = list.FindAll(x => x.Rid == null)
                .ToObservableCollection();
            Priv_parking = list.FindAll(x => x.Rid < 0)
                .ToObservableCollection();
            ProcessViewSource.Source = Priv_processes;
            ParkingViewSource.Source = Priv_parking;
            
            RessCV = CollectionViewSource.GetDefaultView(_machines);
            RessCV.MoveCurrentToFirst();
                _currentWorkArea = ((PlanMachine)RessCV.CurrentItem).WorkArea.WorkAreaId;
  
            }, CancellationToken.None, TaskCreationOptions.None, uiContext);

            NotifyPropertyChanged(() => ProcessCV);
            NotifyPropertyChanged(() => ParkingCV);
            RessCV.Filter = f => (f as PlanMachine)?.WorkArea?.WorkAreaId == _currentWorkArea;
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
                        RessCV.Filter = f => (f as PlanMachine)?.WorkArea?.WorkAreaId == wa.WorkAreaId;
                        ParkingCV.Filter = f => (f as Vorgang)?.Rid == wa.WorkAreaId * -1;

                        _currentWorkArea = wa.WorkAreaId;
                        ProcessCV.Refresh();
                        ParkingCV.Refresh();
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
            }
        }
        private void ProcessCV_Filter(object sender, FilterEventArgs e)
        {
            Vorgang v = (Vorgang)e.Item;
            e.Accepted = false;
            var l = _machines.Where(x => x.WorkArea?.WorkAreaId == _currentWorkArea);
            if (l.Any(x => x.RID == v.ArbPlSapNavigation?.RessourceId))
            {
                e.Accepted = true;
                if (!string.IsNullOrWhiteSpace(_searchFilterText))
                {
                    
                    if (!(e.Accepted = v.Aid.Contains(_searchFilterText, StringComparison.CurrentCultureIgnoreCase)))
                        if (!(e.Accepted = v.AidNavigation?.Material?.Contains(_searchFilterText, StringComparison.CurrentCultureIgnoreCase) ?? false))
                            e.Accepted = v.AidNavigation?.MaterialNavigation?.Bezeichng?.Contains(_searchFilterText, StringComparison.CurrentCultureIgnoreCase) ?? false;
                }
            }
        }
        public void DragOver(IDropInfo dropInfo)
        {
            if (dropInfo.Data is Vorgang)
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                dropInfo.Effects = DragDropEffects.Move;
            }
        }

        internal void Exit()
        {
            _DbCtx.SaveChanges();
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
                        using var db = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
                        if (db.Ressources.All(x => x.RessourceId != parkRid))
                        {
                            
                            db.Database.ExecuteSqlRaw(@"INSERT INTO dbo.Ressource(RessourceId) VALUES({0})", parkRid);
                            
                        }
                        vrg.Rid = parkRid;
                    }
                    else
                    {
                        vrg.Rid = null;
                    }
                    var source = ((ListCollectionView)dropInfo.DragInfo.SourceCollection);
                    if (source.IsAddingNew) { source.CommitNew(); }
                    source.Remove(vrg);
                    ((ListCollectionView)dropInfo.TargetCollection).AddNewItem(vrg);
                    ((ListCollectionView)dropInfo.TargetCollection).CommitNew();
                }
            }
            catch (Exception e)
            {

                MessageBox.Show(e.Message, "Method Drop", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
