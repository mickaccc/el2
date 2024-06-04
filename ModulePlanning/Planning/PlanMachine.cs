using CompositeCommands.Core;
using El2Core.Constants;
using El2Core.Models;
using El2Core.Services;
using El2Core.Utils;
using El2Core.ViewModelBase;
using GongSolutions.Wpf.DragDrop;
using Microsoft.EntityFrameworkCore;
using ModulePlanning.Specials;
using Prism.Events;
using Prism.Ioc;
using Prism.Services.Dialogs;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Microsoft.Extensions.Logging;
using System.IO;
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
        public PlanMachine CreatePlanMachine(Ressource res)
        {
            return new PlanMachine(res, Container, ApplicationCommands, EventAggregator, SettingsService, DialogService);
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

        public PlanMachine(Ressource ressource, IContainerProvider container, IApplicationCommands applicationCommands,
            IEventAggregator eventAggregator, IUserSettingsService settingsService, IDialogService dialogService)
        {
            _container = container;
            var factory = container.Resolve<ILoggerFactory>();
            _logger = factory.CreateLogger<PlanMachine>();

            _rId = ressource.RessourceId;
            _applicationCommands = applicationCommands;
            _eventAggregator = eventAggregator;
            _settingsService = settingsService;
            _dialogService = dialogService;
  
            Setup = settingsService.PlanedSetup;
            Initialize();
            LoadData(ressource);
            if(SelectedRadioButton != 0)
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
        private ILogger _logger;
        private static readonly object _lock = new();
        private HashSet<string> ImChanged = [];
        private readonly int _rId;
        private string _title;
        public string Title => _title;
        public string Setup { get; }
        public bool HasChange => _shifts.Any(x => x.IsChanged);
        public int Rid => _rId;
        private string? _name;
        public string? Name { get { return _name; }
            set { if (_name != value)
                {
                    _name = value;
                    NotifyPropertyChanged(() => Name);
                }
            }
        }
        private string? _description;
        public string? Description { get { return _description; }
            set { if (_description != value)
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
                if (_isAdmin != value)
                {
                    _isAdmin = value;
                    NotifyPropertyChanged(() => IsAdmin);
                }
            }
        }
        public enum ShiftButtons
        {
            [Description("1 Schicht (5:00-13:309")]
            Shift1 = 1,
            [Description("1 Schicht (6:00-14:30)")]
            Shift2 = 2,
            [Description("2 Schicht")]
            Shift3 = 4,
            [Description("3 Schicht")]
            Shift4 = 6,
            [Description("keine")]
            None = 0
        }
        private Dictionary<int, string> _ShiftPlans = [];
        public Dictionary<int, string> ShiftPlans => _ShiftPlans;
        private int _SelectedRadioButton;
        public int SelectedRadioButton
        {
            get { return _SelectedRadioButton; }
            set
            {
                if (value != _SelectedRadioButton)
                {
                    _SelectedRadioButton = value;
                    using var db = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
                    db.Ressources.First(x => x.RessourceId == _rId).ShiftPlanId = _SelectedRadioButton;
                    db.SaveChanges();
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
        public bool EnableRowDetails { get; private set; }
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
        private void LoadData(Ressource res)
        { 
            CostUnits.AddRange(res.RessourceCostUnits.Select(x => x.CostId));
            WorkArea = res.WorkArea;
            Name = res.RessName;
            _SelectedRadioButton = res.ShiftPlanId ??=0;
            _title = res.Inventarnummer ?? string.Empty;
            Vis = res.Visability ??= false;
            Description = res.Info;
            InventNo = res.Inventarnummer;
            Processes = res.Vorgangs.ToObservableCollection();
            ProcessesCVSource.Source = Processes;
            ProcessesCVSource.SortDescriptions.Add(new SortDescription("SortPos", ListSortDirection.Ascending));
            ProcessesCVSource.IsLiveSortingRequested = true;
            ProcessesCVSource.LiveSortingProperties.Add("SortPos");
 
            var db = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();

            foreach(var s in db.ShiftPlanDbs.OrderBy(x => x.Planid))
            {
                _ShiftPlans.Add(s.Planid, s.ShiftName);
            }
        }

        private void Initialize()
        {

            SetMarkerCommand = new ActionCommand(OnSetMarkerExecuted, OnSetMarkerCanExecute);
            OpenMachineCommand = new ActionCommand(OnOpenMachineExecuted, OnOpenMachineCanExecute);
            HistoryCommand = new ActionCommand(OnHistoryExecuted, OnHistoryCanExecute);
            FastCopyCommand = new ActionCommand(OnFastCopyExecuted, OnFastCopyCanExecute);
            CorrectionCommand = new ActionCommand(OnCorrectionExecuted, OnCorrectionCanExecute);
            
            _eventAggregator.GetEvent<MessageVorgangChanged>().Subscribe(MessageReceived);
            _eventAggregator.GetEvent<SearchTextFilter>().Subscribe(MessageSearchFilterReceived);
            IsAdmin = PermissionsProvider.GetInstance().GetUserPermission(Permissions.AdminFunc);
            EnableRowDetails = _settingsService.IsRowDetails;
        }

        private void CalculateEndTime()
        {
            var s = new ShiftPlan(Rid, _container);
            DateTime start = DateTime.Now;
            foreach(var p in Processes.OrderBy(x => x.SortPos))
            {
                var dur = GetProcessDuration(p);

                var l = s.GetEndDateTime(dur, start);
                var diff = l.Subtract(start);

                if (diff.TotalMinutes == 0) p.Extends = "---";
                    else p.Extends = string.Format("({0}){1:N2}h \n{2}",p.QuantityMissNeo, diff.TotalHours, l.ToString("dd.MM.yy - HH:mm"));
                start = l;
            }
        }
        private double GetProcessDuration(Vorgang vorgang)
        {
            double duration = 0.0;
            var r = vorgang.Rstze == null ? 0.0 : (short)vorgang.Rstze; //Setup time
            var c = vorgang.Correction == null ? 0.0 : (short)vorgang.Correction; //correction time
            if (vorgang.AidNavigation.Quantity != null && vorgang.AidNavigation.Quantity != 0.0) //if have Total quantity
            {
                //calculation of the currently required time
                var procT = vorgang.Beaze == null ? 0.0 : (short)vorgang.Beaze;
                var quant = (short)vorgang.AidNavigation.Quantity;
                var miss = vorgang.QuantityMissNeo == null ? 0.0 : (short)vorgang.QuantityMissNeo;
                duration = procT / quant * miss + r + c;

            }
            return duration;
        }
                private void MessageSearchFilterReceived(string obj)
        {
            var ind = Processes?.LastOrDefault(x => x.Aid.Equals(obj));
            if (ind == null) ind = Processes?.LastOrDefault(x => x.AidNavigation?.Material?.Equals(obj) ?? false);
            if (ind == null) ind = Processes?.LastOrDefault(x => x.AidNavigation?.MaterialNavigation?.Bezeichng?.Equals(obj) ?? false);
            if (ind != null) ScrollItem = ind;
        }

        private void MessageReceived(List<(string, string)?> vorgangIdList)
        {
            try
            { 
                Task.Run(() =>
                {
                    using var db = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
                    lock (_lock)
                    {
                        foreach ((string, string)? idTuple in vorgangIdList)
                        {
                            if(idTuple != null)
                            {
                                var pr = Processes?.FirstOrDefault(x => x.VorgangId == idTuple.Value.Item2);
                                if (pr != null)
                                {
                                    var proc = db.Vorgangs.Single(x => x.VorgangId == idTuple.Value.Item2);
                                    var v = db.Vorgangs.Local;
                                    if ((proc.SysStatus?.Contains("RÜCK") ?? false) || proc.Rid == null)
                                    {
                                        proc.SortPos = "Z";
                                        Application.Current.Dispatcher.Invoke(new Action(() => Processes?.Remove(pr)));
                                    }
                                    if (pr.Equals(proc) == false) { ChangedValues(idTuple.Value.Item1, pr, proc); }
                                }
                                else if (db.Vorgangs.Find(idTuple.Value.Item2)?.Rid == Rid)
                                {
                                    var vo = db.Vorgangs.AsNoTracking()
                                        .Include(x => x.AidNavigation)
                                        .ThenInclude(x => x.MaterialNavigation)
                                        .Include(x => x.AidNavigation.DummyMatNavigation)
                                        .Include(x => x.RidNavigation)
                                        .First(x => x.VorgangId == idTuple.Value.Item2);

                                    if (vo?.SysStatus?.Contains("RÜCK") == false)
                                    {
                                        Application.Current.Dispatcher.Invoke(new Action(() => Processes?.Add(vo)));
                                    }
                                }
                            }
                        }                      
                    }
                });        
            }
            catch (Exception ex)
            {
                _logger.LogError("{message}", ex.ToString());
                MessageBox.Show(string.Format("{0}\n{1}", ex.Message, ex.InnerException), "MsgReceivedPlanMachine", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void ChangedValues(string invoker, Vorgang local, Vorgang remote)
        {
            if (invoker == "EL2")
            {
                bool changed = false;
                if (local.BemM != remote.BemM) { local.BemM = remote.BemM; changed = true; }
                if (local.BemMa != remote.BemMa) { local.BemMa = remote.BemMa; changed = true; }
                if (local.CommentMach != remote.CommentMach) { local.CommentMach = remote.CommentMach; changed = true; }
                if (local.BemT != remote.BemT) { local.BemT = remote.BemT; changed = true; }
                if (local.Bullet != remote.Bullet) { local.Bullet = remote.Bullet; changed = true; }
                if (local.Correction != remote.Correction) { local.Correction = remote.Correction; changed = true; }
                if (local.Rid != remote.Rid) { local.Rid = remote.Rid; changed = true; }
                if (local.SortPos != remote.SortPos) { local.SortPos = remote.SortPos; changed = true; }
                if (local.Termin != remote.Termin) { local.Termin = remote.Termin; changed = true; }

                if (changed) local.RunPropertyChanged();
            }
            else
            { 
                local.SpaetEnd = remote.SpaetEnd;
                local.SpaetStart = remote.SpaetStart; 
                local.QuantityScrap = remote.QuantityScrap;
                local.QuantityMiss = remote.QuantityMiss;
                local.QuantityMissNeo = remote.QuantityMissNeo;
                local.Aktuell = remote.Aktuell;
                local.QuantityYield = remote.QuantityYield;

                local.RunPropertyChanged();
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

                if (matInfo != null && matInfo.Count > 0)
                {
                    var par = new DialogParameters();
                    par.Add("orderList", matInfo);
                    par.Add("VNR", vrg.Vnr);
                    par.Add("VID", vrg.VorgangId);
                    _dialogService.Show("HistoryDialog", par, HistoryCallBack);
                }
                else
                {
                    MessageBox.Show("Keine Einträge vorhanden!","Information",MessageBoxButton.OK, MessageBoxImage.Information);
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
                _logger.LogError("{message}", e.ToString());
                MessageBox.Show(string.Format("{0}\n{1}", e.Message, e.InnerException), "FastCopy", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void setTextToClipboard(string text)
        {
            System.Windows.Clipboard.SetText(text);
            Task.Delay(500).Wait();          
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

                    desc.RunPropertyChanged();
                }
            }
            catch (System.Exception e)
            {
                _logger.LogError("{message}", e.ToString());
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
                _logger.LogError("{message}", e.ToString());
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

 
        private void InsertItems(Vorgang Item, ListCollectionView Source, ListCollectionView Target, int Index, bool sorting)
        {

            Item.Rid = _rId;
            List<Vorgang> lst = [];
            var p = Target.SourceCollection as Collection<Vorgang>;
            lst.AddRange(p.OrderBy(x => x.SortPos));
            int oldIndex = Target.IndexOf(Item);
            if (oldIndex == -1) // commes from outside
            {
                if (Index >= Target.Count)
                {
                    ((IList)Target.SourceCollection).Add(Item);
                    lst.Add(Item);
                }
                else
                {
                    ((IList)Target.SourceCollection).Insert(Index, Item);
                    lst.Insert(Index, Item);
                }
                Source.Remove(Item);
            }

            if(oldIndex != -1) //sorting inside
            {
                lst.RemoveAt(oldIndex);

                if (Index > oldIndex) Index--;
                // the actual index could have shifted due to the removal

                lst.Insert(Index, Item);
            }

            for (int i=0; i < lst.Count; i++)
            {
                var vrg = p.First(x => x.Equals(lst[i]));
                vrg.SortPos = string.Format("{0,4:0}_{1,3:0}", Rid.ToString("D3"), i.ToString("D3"));
            }
            Target.MoveCurrentTo(Item);
            if (Item.AidNavigation.Material != null && WorkArea.CreateFolder)
            {
                string[] oa = new[] {Item.AidNavigation.Material, Item.Aid, WorkArea.Bereich}; 
                var work = _container.Resolve<WorkareaDocumentInfo>();
                work.CreateDocumentInfos(oa);
                work.Collect();
            }
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
                    ListCollectionView lv = ProcessesCV as ListCollectionView;
                    if (lv.IsAddingNew) { lv.CommitNew(); }
                    if (lv.IsEditingItem) { lv.CommitEdit(); }
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
 
                    ProcessesCV.Refresh();
                    _eventAggregator.GetEvent<ContextPlanMachineChanged>().Publish(Rid);
                }               
            }
            catch (Exception e)
            {
                _logger.LogError("{message}", e.ToString());
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
