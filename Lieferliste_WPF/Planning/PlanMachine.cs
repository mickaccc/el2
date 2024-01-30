using BionicCode.Utilities.Net.Standard.Extensions;
using CompositeCommands.Core;
using El2Core.Constants;
using El2Core.Converters;
using El2Core.Models;
using El2Core.Services;
using El2Core.Utils;
using El2Core.ViewModelBase;
using GongSolutions.Wpf.DragDrop;
using Lieferliste_WPF.Utilities;
using Lieferliste_WPF.ViewModels;
using Lieferliste_WPF.Views;
using Microsoft.EntityFrameworkCore;
using Prism.Events;
using Prism.Ioc;
using Prism.Services.Dialogs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Printing;
using System.Runtime;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Lieferliste_WPF.Planning
{
    public interface IPlanMachineFactory
    {
        IContainerProvider Container { get; }
        IApplicationCommands ApplicationCommands { get; }
        IEventAggregator EventAggregator { get; }
        IUserSettingsService SettingsService { get; }
        IDialogService DialogService { get; }
    }
    internal class PlanMachineFactory : IPlanMachineFactory
    {
        public IContainerProvider Container { get; }

        public IApplicationCommands ApplicationCommands { get; }

        public IEventAggregator EventAggregator { get; }

        public IUserSettingsService SettingsService { get; }

        public IDialogService DialogService { get; }


        public PlanMachineFactory(IContainerProvider container, IApplicationCommands applicationCommands,
            IEventAggregator eventAggregator, IUserSettingsService settingsService, IDialogService dialogService)
        {
            this.Container = container;
            this.ApplicationCommands = applicationCommands;
            this.EventAggregator = eventAggregator;
            this.SettingsService = settingsService;
            this.DialogService = dialogService;
        }
        public PlanMachine CreatePlanMachine(int Rid)
        {
            return new PlanMachine(Rid, Container, ApplicationCommands, EventAggregator, SettingsService, DialogService);
        }
    }
    public interface IPlanMachine
    {
        public int Rid { get; }
    }
    internal class PlanMachine : ViewModelBase, IPlanMachine, IDropTarget, IViewModel
    {

        #region Constructors

        public PlanMachine(int Rid, IContainerProvider container, IApplicationCommands applicationCommands,
            IEventAggregator eventAggregator, IUserSettingsService settingsService, IDialogService dialogService)
        {
            _container = container;
            _dbCtx = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
            _rId = Rid;
            _applicationCommands = applicationCommands;
            _eventAggregator = eventAggregator;
            _settingsService = settingsService;
            _dialogService = dialogService;
            Initialize();
            LoadData();
            _eventAggregator = eventAggregator;
            ProcessesCV.Refresh();
        }

        #endregion

        public bool Vis { get; private set; }

        public ICommand? SetMarkerCommand { get; private set; }
        public ICommand? OpenMachineCommand { get; private set; }
        public ICommand? MachinePrintCommand { get; private set; }
        public ICommand? HistoryCommand { get; private set; }

        private readonly int _rId;
        private string _title;
        public string Title => _title;
        public bool HasChange => _dbCtx.ChangeTracker.HasChanges() || _employees.Any(x => x.IsChanged);
        public int Rid => _rId;
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? InventNo { get; private set; }
        public WorkArea? WorkArea { get; set; }
        public List<int> CostUnits { get; set; } = [];
        private ObservableCollection<UserStruct> _employees = [];
        public ICollectionView EmployeesView { get; private set; }
        protected MachinePlanViewModel? Owner { get; }

        public ObservableCollection<Vorgang>? Processes { get; set; }

        public ICollectionView ProcessesCV { get { return ProcessesCVSource.View; } }
        private DB_COS_LIEFERLISTE_SQLContext _dbCtx;
        private IContainerProvider _container;
        private IEventAggregator _eventAggregator;
        private IApplicationCommands? _applicationCommands;
        private IUserSettingsService _settingsService;
        private readonly IDialogService _dialogService;
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
            Processes.AddRange(res.Vorgangs.Where(x => x.SysStatus.Contains("RÜCK") == false));
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
                foreach (var emp in _dbCtx.Users.Where(x => x.UserCosts.Any(y => y.CostId == CostUnits[i])
                            && x.UserWorkAreas.Any(z => z.WorkAreaId == WorkArea.WorkAreaId)))
                {

                    if (_employees.All(x => x.User != emp))
                        _employees.Add(new UserStruct(emp, res.RessourceUsers.Any(x => x.UsId == emp.UserIdent)));
                }
            }
            EmployeesView = CollectionViewSource.GetDefaultView(_employees);
        }
        private void Initialize()
        {

            SetMarkerCommand = new ActionCommand(OnSetMarkerExecuted, OnSetMarkerCanExecute);
            OpenMachineCommand = new ActionCommand(OnOpenMachineExecuted, OnOpenMachineCanExecute);
            HistoryCommand = new ActionCommand(OnHistoryExecuted, OnHistoryCanExecute);
            Processes = new ObservableCollection<Vorgang>();
            ProcessesCVSource.Source = Processes;
            ProcessesCV.SortDescriptions.Add(new SortDescription("Spos", ListSortDirection.Ascending));
            ProcessesCV.Filter = f => !((Vorgang)f).SysStatus?.Contains("RÜCK") ?? false;

            //var live = ProcessesCV as ICollectionViewLiveShaping;
            //if (live != null)
            //{
            //    live.IsLiveSorting = false;
            //    live.LiveFilteringProperties.Add("SysStatus");
            //    live.IsLiveFiltering = true;
            //}
            _eventAggregator.GetEvent<MessageVorgangChanged>().Subscribe(MessageReceived);

        }

        private void MessageReceived(List<string?> vorgangIdList)
        {
            try
            {
                foreach (string? id in vorgangIdList.Where(x => x != null))
                {
                    var pr = Processes?.FirstOrDefault(x => x.VorgangId == id);
                    if (pr != null)
                    {

                        _dbCtx.ChangeTracker.Entries<Vorgang>().First(x => x.Entity.VorgangId == pr.VorgangId).Reload();
                        pr.RunPropertyChanged();
                        if (pr.SysStatus?.Contains("RÜCK") ?? false)
                        {
                            Processes?.Remove(pr);
                            _dbCtx.ChangeTracker.Entries<Vorgang>().First(x => x.Entity.VorgangId != pr.VorgangId).State = EntityState.Unchanged;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "MsgReceivedPlanMachine", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private bool OnHistoryCanExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission(Permissions.HistoryDialog);
        }

        private void OnHistoryExecuted(object obj)
        {
            if(obj is Vorgang vrg)
            {
                var matInfo = _dbCtx.OrderRbs.AsNoTracking()
                    .Include(x => x.MaterialNavigation)
                    .Include(x => x.DummyMatNavigation)
                    .Include(x => x.Vorgangs)
                    .Where(x => x.Material == vrg.AidNavigation.Material && x.Aid != vrg.Aid)
                    .ToList();

                if (matInfo != null)
                {
                    var par = new DialogParameters();
                    par.Add("orderList", matInfo);
                    par.Add("VNR", vrg.Vnr);
                    par.Add("VID", vrg.VorgangId);
                    _dialogService.Show("HistoryDialog", par, HistoryCallBack);
                }
            }
        }

        private void HistoryCallBack(IDialogResult result)
        {
            if (result.Result == ButtonResult.Yes)
            {
                string[] bemt = [];
                var vid = result.Parameters.GetValue<string>("VID");
                var bem = result.Parameters.GetValue<string>("Comment");
                if(bem != null) bemt = bem.Split(';');
                if (bemt.Length > 1)
                {
                    Processes.First(x => x.VorgangId == vid).BemT = String.Format("[{0}-{1}];{2}",
                        UserInfo.User.UserIdent, DateTime.Now.ToShortDateString(), bemt[1]);
                }
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

                if (desc != null)
                {
                    if (name == "DelBullet") desc.Bullet = Brushes.White.ToString();
                    if (name == "Bullet1") desc.Bullet = Brushes.Red.ToString();
                    if (name == "Bullet2") desc.Bullet = Brushes.Green.ToString();
                    if (name == "Bullet3") desc.Bullet = Brushes.Yellow.ToString();
                    if (name == "Bullet4") desc.Bullet = Brushes.Blue.ToString();

                    ProcessesCV.Refresh();
                }
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
                var par = new DialogParameters();
                par.Add("processList", this.Processes.ToList());
                par.Add("InvNo", this.InventNo);
                par.Add("Name", this.Name);
                par.Add("Description", this.Description);
                par.Add("CostUnits", this.CostUnits);
                _dialogService.Show("MachineView", par, MachineViewCallBack);
            }
            catch (Exception e)
            {

                MessageBox.Show(e.Message, "Error OpenMachine", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MachineViewCallBack(IDialogResult result)
        {
            if (result.Result == ButtonResult.Yes)
            {
                string[] bemt = [];
                var vid = result.Parameters.GetValue<string>("VID");
                var bem = result.Parameters.GetValue<string>("Comment");
                if (bem != null) bemt = bem.Split(';');
                if (bemt.Length > 1)
                {
                    Processes.First(x => x.VorgangId == vid).BemT = String.Format("[{0}-{1}];{2}",
                        UserInfo.User.UserIdent, DateTime.Now.ToShortDateString(), bemt[1]);
                }
            }
        }

        private void OnDateChanged(object obj)
        {
            if(obj is Vorgang vrg)
            {
                _dbCtx.Vorgangs.First(x => x.VorgangId == vrg.VorgangId).Termin = vrg.Termin;
            }
        }
        private void MachineClosed(object? sender, EventArgs e)
        {
            if (_dbCtx.ChangeTracker.HasChanges() || _employees.Any(x => x.IsChanged))
            {
                if (!SaveQuestion())
                {
                    var canged = _dbCtx.ChangeTracker.Entries()
                        .Where(x => x.State == EntityState.Modified).ToList();
                    foreach (var c in canged)
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
                    p[i].Spos = (p[i].SysStatus?.Contains("RÜCK") == true) ? 1000 : i;
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
            var emp = _employees.Any(x => x.IsChanged);
            if (_dbCtx.ChangeTracker.HasChanges() || emp)
            {
                SaveQuestion();
            }
        }
        private bool SaveQuestion()
        {
            foreach (var item in _employees.Where(x => x.IsChanged))
            {
                if (item.IsCheck)
                {
                    if (!_dbCtx.RessourceUsers.Any(x => x.UsId == item.User.UserIdent && x.Rid == this.Rid))
                        _dbCtx.RessourceUsers.Add(new RessourceUser() { Rid = this.Rid, UsId = item.User.UserIdent });
                }
                else
                {
                    var ru = _dbCtx.RessourceUsers.SingleOrDefault(x => x.UsId == item.User.UserIdent && x.Rid == this.Rid);
                    if (ru != null)
                        _dbCtx.RessourceUsers.Remove(ru);
                }
            }
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
                }
                else return false;
            }
        }
        public class UserStruct : ViewModelBase
        {
            public UserStruct(User usr, bool isc)
            {
                this.User = usr;
                this._isCheck = isc;
            }
            private bool _isCheck;
            public bool IsChanged { get; private set; }
            public User User { get; }
            public bool IsCheck
            {
                get { return _isCheck; }
                set
                {
                    if (_isCheck != value)
                    {
                        _isCheck = value;
                        IsChanged = !IsChanged;
                        NotifyPropertyChanged(() => IsCheck);
                        NotifyPropertyChanged(() => IsChanged);
                    }
                }
            }
        }
    }
}
