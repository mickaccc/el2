
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

        public List<WorkArea>? WorkAreas { get; private set; }
        public List<PlanMachine> Machines { get; }
        private ObservableCollection<Vorgang>? Priv_processes { get; set; }
        private ObservableCollection<Vorgang>? Priv_parking { get; set; }
        private DB_COS_LIEFERLISTE_SQLContext _DbCtx;
        private IContainerExtension _container;

        private readonly ICollectionView _ressCV;
        public ICollectionView ProcessCV { get { return ProcessViewSource.View; } }
        public ICollectionView ParkingCV { get { return ParkingViewSource.View; } }
        private IApplicationCommands _applicationCommands;

        private NotifyTaskCompletion<PlanMachine> _machineTask;

        public NotifyTaskCompletion<PlanMachine> MachineTask
        {
            get { return _machineTask; }
            set {
                if (_machineTask != value)
                {
                    _machineTask = value;
                    NotifyPropertyChanged(() =>  MachineTask);
                }
            }
        }
        private NotifyTaskCompletion<PlanMachine> _processTask;

        public NotifyTaskCompletion<PlanMachine> ProcessTask
        {
            get { return _processTask; }
            set
            {
                if (_processTask != value)
                {
                    _processTask = value;
                    NotifyPropertyChanged(() => ProcessTask);
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


        internal CollectionViewSource ProcessViewSource { get; } = new();
        internal CollectionViewSource ParkingViewSource { get; } = new();

        public MachinePlanViewModel(IContainerExtension container, IApplicationCommands applicationCommands)
        {
            _container = container;
            _applicationCommands = applicationCommands;
            _DbCtx = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
            Machines = new List<PlanMachine>();
            SaveCommand = new ActionCommand(OnSaveExecuted, OnSaveCanExecute);

            LoadData();
            _ressCV = CollectionViewSource.GetDefaultView(Machines);
            _ressCV.Filter = f => (f as PlanMachine)?.WorkArea?.WorkAreaId == WorkAreas?.First().WorkAreaId;
            _ressCV.MoveCurrentToFirst();
            ProcessViewSource.Source = Priv_processes;
            ProcessViewSource.Filter += ProcessCV_Filter;

            ParkingViewSource.Source = Priv_parking;
            ParkingCV.Filter = f => (f as Vorgang)?.Rid == WorkAreas?.First().WorkAreaId * -1;
            _currentWorkArea = WorkAreas?.First().WorkAreaId ?? 0;
            ProcessCV.Refresh();

        }

        private bool OnSaveCanExecute(object arg)
        {
            return _DbCtx.ChangeTracker.HasChanges();
        }

        private void OnSaveExecuted(object obj)
        {
            _DbCtx.SaveChanges();
        }

        private void LoadData()
        {

            var qp = _DbCtx.Vorgangs
            .Include(x => x.AidNavigation)
            .ThenInclude(x => x.MaterialNavigation)
            .Include(x => x.AidNavigation.DummyMatNavigation)
            .Include(x => x.ArbPlSapNavigation)
            .Include(x => x.RidNavigation)
            .Where(x => x.SysStatus != null
                        && !x.AidNavigation.Fertig
                        && !x.SysStatus.Contains("RÜCK")
                        && (x.Text != null) ? !x.Text.ToUpper().Contains("AUFTRAG STARTEN") : true)
            .ToList();


            var work = _DbCtx.WorkAreas
                .Include(x => x.UserWorkAreas)
                .Where(x => x.UserWorkAreas.Any(x => x.UserId.Equals(UserInfo.User.UserIdent)));

            WorkAreas = new List<WorkArea>(work);


            var re = _DbCtx.Ressources
                .Include(x => x.RessourceCostUnits)
                .Include(x => x.WorkArea)
                .Where(x => x.WorkArea != null).ToList();

            var ress = re.Where(y => y.RessourceCostUnits.IntersectBy(UserInfo.User.UserCosts.Select(e => e.CostId), a => a.CostId).Any());


            foreach (var q in ress)
            {

                PlanMachine plm = new(q.RessourceId, q.RessName ?? String.Empty, q.Inventarnummer ?? String.Empty, this)
                {
                    WorkArea = q.WorkArea,
                    CostUnits = q.RessourceCostUnits.Select(x => x.CostId).ToArray(),
                    Description = q.Info ?? String.Empty,
                    ApplicationCommands = _applicationCommands,
                    Vis = q.Visability
                };

                List<Vorgang> VrgList = qp.FindAll(x => x.Rid == q.RessourceId);

                foreach (var vrg in VrgList)
                {
                    if (vrg.VorgangId.Length > 0)
                        plm.Processes?.Add(vrg);
                }

                Machines.Add(plm);
            }
            List<Vorgang> list = new();

            foreach (var m in Machines)
            {
                foreach (var c in UserInfo.User.UserCosts)
                {
                    list.AddRange(qp.FindAll(x => x.ArbPlSapNavigation?.RessourceId == m.RID &&
                        x.ArbPlSapNavigation.CostId == c.CostId));
                }
            }
            Priv_processes = list.FindAll(x => x.Rid == null)
                .ToObservableCollection();
            Priv_parking = list.FindAll(x => x.Rid == -1)
                .ToObservableCollection();
        }
        private async NotifyTaskCompletion<PlanMachine> LoadMachinesAsync()
        {

        }
        private void SelectionChange(object commandParameter)
        {
            if (commandParameter is SelectionChangedEventArgs sel)
            {
                if (sel.AddedItems[0] is WorkArea wa)
                {
                    _ressCV.Filter = f => (f as PlanMachine)?.WorkArea?.WorkAreaId == wa.WorkAreaId;
                    ParkingCV.Filter = f => (f as Vorgang)?.Rid == wa.WorkAreaId * -1;

                    _currentWorkArea = wa.WorkAreaId;
                    ProcessCV.Refresh();
                    ParkingCV.Refresh();
                }
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
            var l = Machines.FindAll(x => x.WorkArea?.WorkAreaId == _currentWorkArea);
            if (l.Any(x => x.RID == v.ArbPlSapNavigation?.RessourceId))
            {
                e.Accepted = true;
                if (!string.IsNullOrWhiteSpace(_searchFilterText))
                {
                    _searchFilterText = _searchFilterText.ToUpper();
                    if (!(e.Accepted = v.Aid.ToUpper().Contains(_searchFilterText)))
                        if (!(e.Accepted = v.AidNavigation?.Material?.ToUpper().Contains(_searchFilterText) ?? false))
                            e.Accepted = v.AidNavigation?.MaterialNavigation?.Bezeichng?.ToUpper().Contains(_searchFilterText) ?? false;
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
                            db.Database.ExecuteSqlRaw(@"INSERT INTO dbo.Resource(RessourceId) VALUES({0})", parkRid);
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

                MessageBox.Show(e.Message,"Method Drop",MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
