
using CompositeCommands.Core;
using El2Core.Constants;
using El2Core.Models;
using El2Core.Utils;
using El2Core.ViewModelBase;
using GongSolutions.Wpf.DragDrop;
using Lieferliste_WPF.Interfaces;
using Lieferliste_WPF.Planning;
using Lieferliste_WPF.Utilities;
using Lieferliste_WPF.Views;
using Microsoft.EntityFrameworkCore;
using Prism.Events;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Unity;
using IViewModel = Lieferliste_WPF.Interfaces.IViewModel;

namespace Lieferliste_WPF.ViewModels
{
    [System.Runtime.Versioning.SupportedOSPlatform("windows7.0")]
    internal class MachinePlanViewModel : ViewModelBase, IDropTarget, IViewModel
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
        IEventAggregator _ea;
        public ICollectionView RessCV { get; private set; }
        public ICollectionView ProcessCV { get { return ProcessViewSource.View; } }
        public ICollectionView ParkingCV { get { return ParkingViewSource.View; } }
        private IApplicationCommands _applicationCommands;
        private static System.Timers.Timer? _timer;
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

        public MachinePlanViewModel(IContainerExtension container, IApplicationCommands applicationCommands, IEventAggregator ea)
        {
            _container = container;
            _applicationCommands = applicationCommands;
            _ea = ea;
            _DbCtx = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
            SaveCommand = new ActionCommand(OnSaveExecuted, OnSaveCanExecute);

            LoadWorkAreas();
            MachineTask = new NotifyTaskCompletion<ICollectionView>(LoadMachinesAsync());
            SetTimer();
            _ea.GetEvent<MessageVorgangChanged>().Subscribe(MessageReceived);
        }
        private void MessageReceived(Vorgang vrg)
        {
            var v = _processesAll.FirstOrDefault(x => x.VorgangId == vrg.VorgangId);
            if (v != null)
            {
                v.ActualEndDate = vrg.ActualEndDate;
                v.ActualStartDate = vrg.ActualStartDate;
                v.Aktuell = vrg.Aktuell;
                v.SysStatus = vrg.SysStatus;
                v.Termin = vrg.Termin;
                v.Text = vrg.Text;
                v.BemM = vrg.BemM;
                v.BemMa = vrg.BemMa;
                v.BemT = vrg.BemT;
                v.CommentMach = vrg.CommentMach;
                v.QuantityMiss = vrg.QuantityMiss;
                v.QuantityRework = vrg.QuantityRework;
                v.QuantityScrap = vrg.QuantityScrap;
                v.QuantityYield = vrg.QuantityYield;
                v.Bullet = vrg.Bullet;

                v.RunPropertyChanged();
            }
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
                    && y.Text != null
                    && y.Text.ToLower().Contains("starten") == false
                    && y.SysStatus.Contains("RÜCK") == false)
                  .ToListAsync();
                  
                _processesAll = query;
            }
            return _processesAll;
        }
        private void SetTimer()
        {
            // Create a timer with a 30 seconds interval.
            _timer = new System.Timers.Timer(2000);
            // Hook up the Elapsed event for the timer. 
            _timer.Elapsed += OnTimedEvent;
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }

        private void OnTimedEvent(object? sender, ElapsedEventArgs e)
        {
            if (_processesAll != null)
            {
                Task.Factory.StartNew(() =>
                {

                    using var db = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
                    {
                        var m = db.Msgs
                            .Include(x => x.Onl)
                            .Where(x => x.Onl.PcId == UserInfo.PC && x.Onl.UserId == UserInfo.User.UserIdent)
                            .ToList();
                        foreach (var mess in m)
                        {
                            db.Database.ExecuteSqlRaw(@"DELETE FROM msg WHERE id={0}", mess.Id);
                            var o = _processesAll.FirstOrDefault(x => x.VorgangId == mess.PrimaryKey);
                            if (o != null)
                            {
                                Vorgang vrg = db.Vorgangs.First(x => x.VorgangId == mess.PrimaryKey);
                                if (mess.TableName == "Vorgang")
                                {
                                    o.SysStatus = vrg.SysStatus;
                                    o.Spos = vrg.Spos;
                                    o.Text = vrg.Text;
                                    o.ActualEndDate = vrg.ActualEndDate;
                                    o.ActualStartDate = vrg.ActualStartDate;
                                    o.Aktuell = vrg.Aktuell;
                                    o.Bullet = vrg.Bullet;
                                    o.BemM = vrg.BemM;
                                    o.BemMa = vrg.BemMa;
                                    o.BemT = vrg.BemT;
                                    o.CommentMach = vrg.CommentMach;
                                    o.QuantityMiss = vrg.QuantityMiss;
                                    o.QuantityRework = vrg.QuantityRework;
                                    o.QuantityScrap = vrg.QuantityScrap;
                                    o.QuantityYield = vrg.QuantityYield;

                                    
                                }
                                else if (mess.TableName == "OrderRB")
                                {
                                    o.AidNavigation.Abgeschlossen = vrg.AidNavigation.Abgeschlossen;
                                    o.AidNavigation.Dringend = vrg.AidNavigation.Dringend;
                                    o.AidNavigation.Fertig = vrg.AidNavigation.Fertig;
                                    o.AidNavigation.Mappe = vrg.AidNavigation.Mappe;
                                }
                                o.RunPropertyChanged();
                                _ea.GetEvent<MessageVorgangChanged>().Publish(o);
                            }
                        }
                    }
                });
            }
            
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
                    PlanMachineFactory factory = _container.Resolve<PlanMachineFactory>();

                    foreach (var q in re)
                    { 
                        result.Add(factory.CreatePlanMachine(q.RessourceId));
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
                        RessCV.Filter = f =>
                        {
                            PlanMachine plm = f as PlanMachine;
                            return plm != null
                                && plm.WorkArea?.WorkAreaId == wa.WorkAreaId
                                && plm.Vis;
                        };
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
            if (l.Any(x => x.Rid == v.ArbPlSapNavigation?.RessourceId))
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
            if (PermissionsProvider.GetInstance().GetUserPermission(Permissions.MachDrop))
            {
                if (dropInfo.Data is Vorgang)
                {
                    dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                    dropInfo.Effects = DragDropEffects.Move;
                }
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
                    using var db = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
                    if (name == "parking")
                    {
                        int parkRid = _currentWorkArea * -1;
                        
                        if (db.Ressources.All(x => x.RessourceId != parkRid))
                        {
                            
                            db.Database.ExecuteSqlRaw(@"INSERT INTO dbo.Ressource(RessourceId) VALUES({0})", parkRid);
                            
                        }
                        vrg.Rid = parkRid;
                    }
                    else
                    {
                        vrg.Rid = null;
                        db.Vorgangs.First(x => x.VorgangId == vrg.VorgangId).Rid = vrg.Rid;
                        db.SaveChanges();
                    }
                    var source = ((ListCollectionView)dropInfo.DragInfo.SourceCollection);
                    if (source.IsAddingNew) { source.CommitNew(); }
                    source.Remove(vrg);
                    ((ListCollectionView)dropInfo.TargetCollection).AddNewItem(vrg);
                    ((ListCollectionView)dropInfo.TargetCollection).CommitNew();

                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {

                MessageBox.Show(e.Message, "Method Drop", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void Closing()
        {
            foreach(var m in _machines)
            {
                IViewModel vm = (IViewModel)m;
                vm.Closing();
            }
            if(_DbCtx.ChangeTracker.HasChanges()) _DbCtx.SaveChanges();
        }
    }
}
