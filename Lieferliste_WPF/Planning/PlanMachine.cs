using BionicCode.Utilities.Net.Standard.Extensions;
using CompositeCommands.Core;
using El2Core.Constants;
using El2Core.Models;
using El2Core.Services;
using El2Core.Utils;
using El2Core.ViewModelBase;
using GongSolutions.Wpf.DragDrop;
using Lieferliste_WPF.Interfaces;
using Lieferliste_WPF.Utilities;
using Lieferliste_WPF.ViewModels;
using Lieferliste_WPF.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Prism.Events;
using Prism.Ioc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Lieferliste_WPF.Planning
{ 
    public interface IPlanMachineFactory
    {
        IContainerProvider Container {  get; }
        IApplicationCommands ApplicationCommands { get; }
        IEventAggregator EventAggregator { get; }
        IUserSettingsService SettingsService { get; }
    }
    internal class PlanMachineFactory : IPlanMachineFactory
    {
        public IContainerProvider Container { get; }

        public IApplicationCommands ApplicationCommands { get; }

        public IEventAggregator EventAggregator { get; }

        public IUserSettingsService SettingsService { get; }

        public PlanMachineFactory(IContainerProvider container, IApplicationCommands applicationCommands,
            IEventAggregator eventAggregator, IUserSettingsService settingsService)
        {
            this.Container = container;
            this.ApplicationCommands = applicationCommands;
            this.EventAggregator = eventAggregator;
            this.SettingsService = settingsService;
        }
        public PlanMachine CreatePlanMachine(int Rid)
        {
            return new PlanMachine(Rid, Container, ApplicationCommands, EventAggregator, SettingsService);
        }
    }
    public interface IPlanMachine
    {
        public int Rid { get; }
    }
    internal class PlanMachine : DependencyObject, IPlanMachine, IDropTarget, IViewModel
    {

        #region Constructors
  
        public PlanMachine(int Rid, IContainerProvider container, IApplicationCommands applicationCommands,
            IEventAggregator eventAggregator, IUserSettingsService settingsService)
        {
            _container = container;
            _dbCtx = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
            _rId = Rid;
            _applicationCommands = applicationCommands;
            _eventAggregator = eventAggregator;
            _settingsService = settingsService;
            Initialize();
            LoadData();
            _eventAggregator = eventAggregator;
        }

        #endregion

        public bool Vis
        {
            get { return (bool)GetValue(VisProperty); }
            set { SetValue(VisProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Vis.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VisProperty =
            DependencyProperty.Register("Vis", typeof(bool), typeof(PlanMachine), new PropertyMetadata(true));

        public ICommand? SetMarkerCommand { get; private set; }
        public ICommand? OpenMachineCommand { get; private set; }
        public ICommand? MachinePrintCommand {  get; private set; }

        private readonly int _rId;
        private string _title;
        public string Title => _title;
        public bool HasChange => _dbCtx.ChangeTracker.HasChanges();
        public int Rid => _rId;
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? InventNo { get; private set; }
        public WorkArea? WorkArea { get; set; }
        public List<int> CostUnits { get; set; } = [];
        public Dictionary<User, bool> Employees { get; set; } = [];
        protected MachinePlanViewModel? Owner { get; }

        public ObservableCollection<Vorgang>? Processes { get; set; }

        public ICollectionView ProcessesCV { get { return ProcessesCVSource.View; } }
        private DB_COS_LIEFERLISTE_SQLContext _dbCtx;
        private IContainerProvider _container;
        private IEventAggregator _eventAggregator;
        private IApplicationCommands? _applicationCommands;
        private IUserSettingsService _settingsService;
        public IApplicationCommands? ApplicationCommands
        {
            get { return _applicationCommands; }
            set
            {
                if (_applicationCommands != value)
                {
                    _applicationCommands = value;
                }
            }
        }

        internal CollectionViewSource ProcessesCVSource { get; set; } = new CollectionViewSource();


        private void LoadData()
        {
            var res = _dbCtx.Ressources
                .Include(x => x.WorkArea)
                .Include(x => x.WorkSaps)
                .Include(x => x.RessourceCostUnits)
                .Include(x => x.Vorgangs)
                .ThenInclude(x => x.AidNavigation)
                .ThenInclude(x => x.MaterialNavigation)
                .Include(x => x.RessourceUsers)
                .ThenInclude(x => x.Us)
                .First(x => x.RessourceId == Rid);

            CostUnits.AddRange(res.RessourceCostUnits.Select(x => x.CostId));
            Processes.AddRange(res.Vorgangs.Where(x => x.SysStatus.Contains("RÜCK")==false));
            WorkArea = res.WorkArea;
            Name = res.RessName;
            _title = res.Inventarnummer ?? string.Empty;
            Vis = res.Visability;
            Description = res.Info;
            InventNo = res.Inventarnummer;
            var empl = _dbCtx.Users
                .Include(x => x.UserCosts)
                .ToList();
            for (int i = 0; i < CostUnits?.Count; i++)
            {
                foreach ( var emp in _dbCtx.Users.Where(x => x.UserCosts.Any(y => y.CostId == CostUnits[i])
                            && x.UserWorkAreas.Any(z => z.WorkAreaId == WorkArea.WorkAreaId)))
                {
                    if(Employees.Keys.Contains(emp) == false)
                   Employees.Add(emp, res.RessourceUsers.Any(x => x.UsId == emp.UserIdent));                   
                }
            }
            
        }
        private void Initialize()
        {
            
            SetMarkerCommand = new ActionCommand(OnSetMarkerExecuted, OnSetMarkerCanExecute);
            OpenMachineCommand = new ActionCommand(OnOpenMachineExecuted, OnOpenMachineCanExecute);
            MachinePrintCommand = new ActionCommand(OnMachinePrintExecuted, OnMachinePrintCanExecute);
            Processes = new ObservableCollection<Vorgang>();
            ProcessesCVSource.Source = Processes;
            ProcessesCV.SortDescriptions.Add(new SortDescription("Spos", ListSortDirection.Ascending));
            ProcessesCV.Filter = f => !((Vorgang)f).SysStatus?.Contains("RÜCK") ?? false;

            var live = ProcessesCV as ICollectionViewLiveShaping;
            if (live != null)
            {
                live.IsLiveSorting = false;
                live.LiveFilteringProperties.Add("SysStatus");
                live.IsLiveFiltering = true;
            }
            _eventAggregator.GetEvent<MessageVorgangChanged>().Subscribe(MessageReceived);

        }

        private void MessageReceived(Vorgang vorgang)
        {
            var pr = Processes?.FirstOrDefault(x => x.VorgangId == vorgang.VorgangId);
            if (pr != null)
            {
                pr.SysStatus = vorgang.SysStatus;
                pr.QuantityMiss = vorgang.QuantityMiss;
                pr.QuantityScrap = vorgang.QuantityScrap;
                pr.QuantityRework = vorgang.QuantityRework;
                pr.QuantityYield = vorgang.QuantityYield;
                pr.BemT = vorgang.BemT;

            }
        }

        private bool OnMachinePrintCanExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission(Permissions.MachPrint);
        }

        private void OnMachinePrintExecuted(object obj)
        {
            try
            {
                Printing.DoPrintPreview(Printing.CreateFlowDocument(obj));
            }
            catch (System.Exception e)
            {
                MessageBox.Show(e.Message, "MachinePrint", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static bool OnSetMarkerCanExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission(Permissions.SETMARK);
        }

        private void OnSetMarkerExecuted(object obj)
        {
            try
            {
                if (obj == null) return;
                var values = (object[])obj;
                var name = (string)values[0];
                var desc = (Vorgang)values[1];

                if (name == "DelBullet") desc.Bullet = Brushes.White.ToString();
                if (name == "Bullet1") desc.Bullet = Brushes.Red.ToString();
                if (name == "Bullet2") desc.Bullet = Brushes.Green.ToString();
                if (name == "Bullet3") desc.Bullet = Brushes.Yellow.ToString();
                if (name == "Bullet4") desc.Bullet = Brushes.Blue.ToString();

                ProcessesCV.Refresh();
            }
            catch (System.Exception e)
            {
                MessageBox.Show(e.Message, "SetMarker", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        private bool OnOpenMachineCanExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission(Permissions.OpenMach);
        }

        private void OnOpenMachineExecuted(object obj)
        {
            try
            {
                var ma = new MachineView();
                ma.DataContext = this;
                ma.Closed += MachineClosed;
                ma.Show();
            }
            catch (Exception e)
            {

                MessageBox.Show(e.Message, "Error OpenMachine", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MachineClosed(object? sender, EventArgs e)
        {
            if(_dbCtx.ChangeTracker.HasChanges())
            {
                if(!SaveQuestion())
                {
                    var canged = _dbCtx.ChangeTracker.Entries()
                        .Where(x => x.State == EntityState.Modified).ToList();
                    foreach(var c in canged)
                    {
                        c.State = EntityState.Unchanged;
                    }
                }                  
            }
        }

        public void Drop(IDropInfo dropInfo)
        {

            try
            {
                var vrg = (Vorgang)dropInfo.Data;
                var s = dropInfo.DragInfo.SourceCollection as ListCollectionView;
                var t = dropInfo.TargetCollection as ListCollectionView;
                if (s.CanRemove) s.Remove(vrg);
                var v = dropInfo.InsertIndex;
                vrg.Rid = _rId;

                if (v > t?.Count)
                {
                    ((IList)t.SourceCollection).Add(vrg);
                }
                else
                {
                    Debug.Assert(t != null, nameof(t) + " != null");
                    ((IList)t.SourceCollection).Insert(v, vrg);
                }
                var p = t.SourceCollection as Collection<Vorgang>;
 
                for (var i = 0; i < p.Count; i++)
                {
                    p[i].Spos = i;
                    var vv = _dbCtx.Vorgangs.First(x => x.VorgangId == p[i].VorgangId);
                    vv.Spos = i;
                    vv.Rid = _rId;
                }
                t.Refresh();
            }
            catch (Exception e)
            {
                string str = string.Format(e.Message + "\n" + e.InnerException);
                MessageBox.Show(str, "ERROR", MessageBoxButton.OK);
            }
        }

        public void DragOver(IDropInfo dropInfo)
        {
            if (PermissionsProvider.GetInstance().GetUserPermission(Permissions.MachDrop))
            {
                if (dropInfo.Data is Vorgang)
                {
                    dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                    dropInfo.Effects = DragDropEffects.Move;
                } 
            }
        }

        void IViewModel.Closing()
        {
            if (_dbCtx.ChangeTracker.HasChanges())
            {
                SaveQuestion();
            }
        }
        private bool SaveQuestion()
        {
            if (!_settingsService.IsSaveMessage)
            {
                _dbCtx.SaveChangesAsync();
                return true;
            }
            else
            {
                var result = MessageBox.Show(string.Format("Sollen die Änderungen in {0} gespeichert werden?", _title),
                    _title, MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    _dbCtx.SaveChangesAsync();
                    return true;
                } else return false;
            }
        }
    }
}
