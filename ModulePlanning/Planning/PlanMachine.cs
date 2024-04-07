using CompositeCommands.Core;
using El2Core.Constants;
using El2Core.Models;
using El2Core.Services;
using El2Core.Utils;
using El2Core.ViewModelBase;
using GongSolutions.Wpf.DragDrop;
using Microsoft.EntityFrameworkCore;
using ModulePlanning.ViewModels;
using Prism.Events;
using Prism.Ioc;
using Prism.Services.Dialogs;
using System;
using System.Collections;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;



namespace ModulePlanning.Planning
{
    public interface IPlanMachineFactory
    {
        IContainerProvider Container { get; }
        IApplicationCommands ApplicationCommands { get; }
        IEventAggregator EventAggregator { get; }
        IUserSettingsService SettingsService { get; }
        IDialogService DialogService { get; }
    }
    public class PlanMachineFactory : IPlanMachineFactory
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

            DataObject dt = new();
            
        }
        public PlanMachine CreatePlanMachine(int Rid, List<Vorgang> processes)
        {
            return new PlanMachine(Rid, processes, Container, ApplicationCommands, EventAggregator, SettingsService, DialogService);
        }
    }
    public interface IPlanMachine
    {
        public int Rid { get; }
    }
    [System.Runtime.Versioning.SupportedOSPlatform("windows10.0")]
    public class PlanMachine : ViewModelBase, IPlanMachine, IDropTarget, IViewModel
    {

        #region Constructors

        public PlanMachine(int Rid, List<Vorgang> processes, IContainerProvider container, IApplicationCommands applicationCommands,
            IEventAggregator eventAggregator, IUserSettingsService settingsService, IDialogService dialogService)
        {
            _container = container;
            _rId = Rid;
            _applicationCommands = applicationCommands;
            _eventAggregator = eventAggregator;
            _settingsService = settingsService;
            _dialogService = dialogService;
            Initialize();
            Processes.AddRange(processes.OrderBy(x => x.Spos));
            LoadData();
            CalculateEndTime();
            ProcessesCV.Refresh();
        }

        #endregion

        public bool Vis { get; private set; }

        public ICommand? SetMarkerCommand { get; private set; }
        public ICommand? OpenMachineCommand { get; private set; }
        public ICommand? MachinePrintCommand { get; private set; }
        public ICommand? HistoryCommand { get; private set; }
        public ICommand? FastCopyCommand { get; private set; }
        public ICommand? CorrectionCommand { get; private set; }
        private static readonly object _lock = new();
        private readonly int _rId;
        private string _title;
        public string Title => _title;
        public bool HasChange => _shifts.Any(x => x.IsChanged);
        public int Rid => _rId;
        private string? _name;
        public string? Name { get { return _name; }
            set { if(_name != value)
                {
                    _name = value;
                    NotifyPropertyChanged(() => Name);
                }
            }
        }
        private string? _description;
        public string? Description { get { return _description; }
            set { if(_description != value)
                {
                    _description = value;
                    NotifyPropertyChanged(() => Description);
                }
            }
        }
        private bool _isAdmin;
        public bool IsAdmin
        {
            get { return _isAdmin; }
            set
            {
                if(_isAdmin != value)
                {
                    _isAdmin = value;
                    NotifyPropertyChanged(() => IsAdmin);
                }
            }
        }
        public string? InventNo { get; private set; }
        public WorkArea? WorkArea { get; set; }
        public List<int> CostUnits { get; set; } = [];
        private ObservableCollection<ShiftStruct> _shifts = [];
        public ICollectionView ShiftsView { get; private set; }

        public ObservableCollection<Vorgang>? Processes { get; set; }
        public ICollectionView ProcessesCV { get { return ProcessesCVSource.View; } }

        private IContainerProvider _container;
        private IEventAggregator _eventAggregator;
        private IApplicationCommands? _applicationCommands;
        private IUserSettingsService _settingsService;
        private readonly IDialogService _dialogService;
        private Vorgang _scrollItem;

        public Vorgang ScrollItem
        {
            get
            {
                return _scrollItem;
            }
            set
            {
                if (value != _scrollItem)
                {
                    _scrollItem = value;
                    NotifyPropertyChanged(() => ScrollItem);
                }
            }
        }

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
            using var db = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
            var res = db.Ressources
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
            WorkArea = res.WorkArea;
            Name = res.RessName;
            _title = res.Inventarnummer ?? string.Empty;
            Vis = res.Visability;
            Description = res.Info;
            InventNo = res.Inventarnummer;
            var empl = db.Users
                .Include(x => x.UserCosts)
                .ToList();


            //for (int i = 0; i < CostUnits?.Count; i++)
            //{
            //    foreach (var emp in db.Users.Where(x => x.UserCosts.Any(y => y.CostId == CostUnits[i])
            //                && x.UserWorkAreas.Any(z => z.WorkAreaId == WorkArea.WorkAreaId)))
            //    {

            //        if (_shifts.All(x => x.User != emp))
            //            _shifts.Add(new UserStruct(emp, res.RessourceUsers.Any(x => x.UsId == emp.UserIdent)));
            //    }
            //}
            var sh = db.WorkShifts
                .Include(x => x.RessourceWorkshifts)
                .ToFrozenSet();
            foreach(var e in sh)
            {
                _shifts.Add(new ShiftStruct(e, e.RessourceWorkshifts?.Any(x => x.Rid == Rid) ?? false));
            }
            ShiftsView = CollectionViewSource.GetDefaultView(_shifts);
        }
        private void Initialize()
        {

            SetMarkerCommand = new ActionCommand(OnSetMarkerExecuted, OnSetMarkerCanExecute);
            OpenMachineCommand = new ActionCommand(OnOpenMachineExecuted, OnOpenMachineCanExecute);
            HistoryCommand = new ActionCommand(OnHistoryExecuted, OnHistoryCanExecute);
            FastCopyCommand = new ActionCommand(OnFastCopyExecuted, OnFastCopyCanExecute);
            CorrectionCommand = new ActionCommand(OnCorrectionExecuted, OnCorrectionCanExecute);
            Processes = new ObservableCollection<Vorgang>();
            ProcessesCVSource.Source = Processes;
            ProcessesCVSource.SortDescriptions.Add(new SortDescription("Spos", ListSortDirection.Ascending));
            ProcessesCVSource.IsLiveSortingRequested = true;
            //ProcessesCV.Filter = f => !((Vorgang)f).SysStatus?.Contains("RÜCK") ?? false;

            //var live = ProcessesCV as ICollectionViewLiveShaping;
            //if (live != null)
            //{
            //    live.IsLiveSorting = false;
            //    live.LiveFilteringProperties.Add("SysStatus");
            //    live.IsLiveFiltering = true;
            //}
            _eventAggregator.GetEvent<MessageVorgangChanged>().Subscribe(MessageReceived);
            _eventAggregator.GetEvent<SearchTextFilter>().Subscribe(MessageSearchFilterReceived);
            IsAdmin = PermissionsProvider.GetInstance().GetUserPermission(Permissions.AdminFunc);
        }


        private void CalculateEndTime()
        {
            using var processService = _container.Resolve<ProcessStripeService>();
            DateTime start = DateTime.Now;
            foreach(var p in Processes)
            {
                TimeSpan length;
                var end = processService.GetProcessLength(p, start, out length);
                if (length.TotalMinutes == 0) p.Extends = "---";
                    else p.Extends = string.Format("({0}){1:N2}h \n{2}",p.QuantityMissNeo, length.TotalHours, end.ToString("dd.MM.yy - HH:mm"));
                start = end;
            }
        }

        private void MessageSearchFilterReceived(string obj)
        {
            var ind = Processes?.LastOrDefault(x => x.Aid.Equals(obj));
            if (ind == null) ind = Processes?.LastOrDefault(x => x.AidNavigation?.Material?.Equals(obj) ?? false);
            if (ind == null) ind = Processes?.LastOrDefault(x => x.AidNavigation?.MaterialNavigation?.Bezeichng?.Equals(obj) ?? false);
            if (ind != null) ScrollItem = ind;
        }

        private void MessageReceived(List<string?> vorgangIdList)
        {
            try
            {
                Task.Run(() =>
                {
                    using var db = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
                    lock (_lock)
                    {
                        foreach (string? id in vorgangIdList.Where(x => x != null))
                        {
                            var pr = Processes?.FirstOrDefault(x => x.VorgangId == id);
                            if (pr != null)
                            {
                                db.Entry<Vorgang>(pr).Reload();
                                
                                if ((pr.SysStatus?.Contains("RÜCK") ?? false) || pr.Rid == null)
                                {
                                    Application.Current.Dispatcher.Invoke(new Action(() => Processes?.Remove(pr)));                                  
                                }
                                pr.RunPropertyChanged();
                            }
                            else if (db.Vorgangs.Find(id)?.Rid == Rid)
                            {

                                    var vo = db.Vorgangs.Find(id);

                                if (vo?.SysStatus?.Contains("RÜCK") == false)
                                {
                                    Application.Current.Dispatcher.Invoke(new Action(() => Processes?.Add(vo)));  
                                }                            
                            }
                        }                      
                    }
                });        
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("{0}\n{1}", ex.Message, ex.InnerException), "MsgReceivedPlanMachine", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private bool OnCorrectionCanExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission(Permissions.Correction);
        }

        private void OnCorrectionExecuted(object obj)
        {
            if(obj is Vorgang vrg)
            {
                var par = new DialogParameters();
                par.Add("correction", vrg);
                _dialogService.ShowDialog("CorrectionDialog", par, CorrectionCallback);
            }
        }

        private void CorrectionCallback(IDialogResult result)
        {
            if (result.Result == ButtonResult.OK)
            {
                var vrg = result.Parameters.GetValue<Vorgang>("correction");
                var corr = result.Parameters.GetValue<double?>("correct");
                var v = Processes.First(x => x.VorgangId == vrg.VorgangId);
                v.Correction = (float?)corr;
                v.RunPropertyChanged();
                
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
                using var db = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
                var matInfo = db.OrderRbs.AsNoTracking()
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
                if(bem != null) bemt = bem.Split((char)29);
                if (bemt.Length > 1)
                {
                    var pr = Processes?.First(x => x.VorgangId == vid);
                    {
                        pr.BemT = String.Format("[{0}-{1}]{2}{3}",
                        UserInfo.User.UserIdent, DateTime.Now.ToShortDateString(), (char)29, bemt[1]);
                        pr.RunPropertyChanged();
                    }
                }
            }
        }
        private bool OnFastCopyCanExecute(object arg)
        {
            return arg is Vorgang && PermissionsProvider.GetInstance().GetUserPermission(Permissions.FastCopy);
        }

        private void OnFastCopyExecuted(object obj)
        {
            try
            {
                lock (_lock)
                    {
                    if (obj is Vorgang v)
                    {
                        var m = v.AidNavigation.Quantity.ToString();
                        var a = v.Aid.Trim();
                        var vnr = v.Vnr.ToString();
                        var mat = v.AidNavigation.Material?.Trim();
                        var bez = v.AidNavigation.MaterialNavigation?.Bezeichng?.Trim();

                        OnFastCopyExecuted(m);
                        OnFastCopyExecuted(bez ?? a);
                        OnFastCopyExecuted(mat ?? "DUMMY");
                        OnFastCopyExecuted(vnr);
                        OnFastCopyExecuted(a);
                    }
                    if (obj is string s)
                    { setTextToClipboard(s); }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format("{0}\n{1}", e.Message, e.InnerException), "FastCopy", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void setTextToClipboard(string text)
        {
            System.Windows.Clipboard.SetText(text);
            Task.Delay(250).Wait();          
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
                par.Add("PlanMachine", this);
                ListCollectionView lv = ProcessesCV as ListCollectionView;
                if(lv.IsEditingItem) lv.CommitEdit();
                if(lv.IsAddingNew) lv.CommitNew();
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
            }
        }

        //private void MachineClosed(object? sender, EventArgs e)
        //{
        //    if (_dbCtx.ChangeTracker.HasChanges() || _employees.Any(x => x.IsChanged))
        //    {
        //        if (!SaveQuestion())
        //        {
        //            var canged = _dbCtx.ChangeTracker.Entries()
        //                .Where(x => x.State == EntityState.Modified).ToList();
        //            foreach (var c in canged)
        //            {
        //                c.State = EntityState.Unchanged;
        //            }
        //        }
        //    }
        //}
        private void InsertItems(Vorgang Item, ListCollectionView Source, ListCollectionView Target, int Index, bool sorting)
        {
 
            Item.Rid = _rId;
            if (Source.CanRemove) Source.Remove(Item);

            if (Index > Target.Count)
            {
                ((IList)Target.SourceCollection).Add(Item);
            }
            else
            {
                ((IList)Target.SourceCollection).Insert(Index, Item);
            }

            var p = Target.SourceCollection as Collection<Vorgang>;
 
 
            for (int i=0; i < p.Count; ++i)
            {
                p[i].Spos = i;
   
                //var vv = _dbCtx.Vorgangs.First(x => x.VorgangId == p[i].VorgangId);
                //vv.Spos = i;
                //vv.Rid = _rId;
            }
            Target.MoveCurrentTo(Item);
        }
        public void Drop(IDropInfo dropInfo)
        {

            try
            {
                var s = dropInfo.DragInfo.SourceCollection as ListCollectionView;
                var t = dropInfo.TargetCollection as ListCollectionView;
                
                var v = dropInfo.InsertIndex;
                if (s != null && t != null)
                {
                    if (dropInfo.Data is List<dynamic> vrgList)
                    {
                        foreach (var vrg in vrgList)
                        {
                            InsertItems(vrg, s, t, v, false);
                        }
                    }
                    else if (dropInfo.Data is Vorgang vrg)
                    {
                        InsertItems(vrg, s, t, v, dropInfo.IsSameDragDropContextAsSource);
                    }
                    ListCollectionView lv = ProcessesCV as ListCollectionView;
                    if (lv.IsAddingNew) { lv.CommitNew(); }
                    if (lv.IsEditingItem) { lv.CommitEdit(); }
                    ProcessesCV.Refresh();                  
                }    
            }
            catch (Exception e)
            {
                string str = string.Format(e.Message + "\n" + e.InnerException);
                MessageBox.Show(str, "ERROR", MessageBoxButton.OK);
            }
        }

        public void DragOver(IDropInfo dropInfo)
        {
            if (PermissionsProvider.GetInstance().GetRelativeUserPermission(Permissions.MachDrop, WorkArea?.WorkAreaId ?? 0))
            {
                if (dropInfo.Data is Vorgang || dropInfo.Data is List<dynamic>)
                {
                    dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                    dropInfo.Effects = DragDropEffects.Move;
                }               
            }
        }

        void IViewModel.Closing()
        {
 
        }

        internal void SaveAll()
        {
            using var db = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
            foreach (var s in _shifts.Where(x => x.IsChanged))
            {
                
                RessourceWorkshift ws = new RessourceWorkshift() { Rid = Rid, Sid = s.Shift.Sid };
                if (s.IsCheck)
                    db.RessourceWorkshifts.Add(ws);
                else db.RessourceWorkshifts.Remove(ws);
                s.IsChanged = false;
            }
            db.SaveChanges();
            
        }

        public class ShiftStruct : ViewModelBase
        {
            public ShiftStruct(WorkShift shf, bool isc)
            {
                this.Shift = shf;
                this._isCheck = isc;
            }
            private bool _isCheck;
            public bool IsChanged { get; set; }
            public WorkShift Shift { get; }
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
