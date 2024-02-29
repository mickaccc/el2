using CompositeCommands.Core;
using El2Core.Constants;
using El2Core.Models;
using El2Core.Services;
using El2Core.Utils;
using El2Core.ViewModelBase;
using Lieferliste_WPF.Planning;
using Lieferliste_WPF.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Prism.Events;
using Prism.Ioc;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Printing;
using System.Text;
using System.Text.RegularExpressions;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Lieferliste_WPF.ViewModels
{
    /// <summary>
    /// Class for the main window's view-model.
    /// </summary>
    [System.Runtime.Versioning.SupportedOSPlatform("windows10.0")]
    public class MainWindowViewModel : ViewModelBase
    {

        public ICommand OpenMachinePlanCommand { get; private set; }
        public ICommand OpenLieferlisteCommand { get; private set; }
        public ICommand OpenSettingsCommand { get; private set; }
        public ICommand OpenUserMgmtCommand { get; private set; }
        public ICommand OpenRoleMgmtCommand { get; private set; }
        public ICommand OpenMachineMgmtCommand { get; private set; }
        public ICommand TabCloseCommand { get; private set; }
        public ICommand CloseCommand { get; private set; }
        public ICommand OpenArchiveCommand { get; private set; }
        public ICommand OpenWorkAreaCommand { get; private set; }
        public ICommand OpenProjectCombineCommand { get; private set; }
        public ICommand OpenMeasuringCommand { get; private set; }

        private IApplicationCommands _applicationCommands;
        public IApplicationCommands ApplicationCommands
        {
            get { return _applicationCommands; }
            set
            {
                if (_applicationCommands != value)
                {
                    _applicationCommands = value;
                    NotifyPropertyChanged(() => ApplicationCommands);
                }
            }
        }
        public ICommand ExplorerCommand { get; }
        public ICommand OpenOrderCommand { get; }
        public ICommand ArchivateCommand { get; }
        public ICommand OpenProjectOverViewCommand { get; }
        public ICommand MachinePrintCommand { get; }
        private NotifyTaskCompletion<int>? _onlineTask;
        public NotifyTaskCompletion<int>? OnlineTask
        {
            get { return _onlineTask; }
            set
            {
                if (_onlineTask != value)
                {
                    _onlineTask = value;
                    NotifyPropertyChanged(() => OnlineTask);
                }
            }
        }
        private string? _msg;
        public string? Msg
        {
            get { return _msg; }
            private set
            {
                if (value != _msg)
                {
                    _msg = value;
                    NotifyPropertyChanged(() => Msg);
                }
            }
        }
        private static int _onlines;
        private static System.Timers.Timer? _timer;
        private IRegionManager _regionmanager;
        private static readonly object _lock = new();
        private readonly IContainerExtension _container;
        private readonly IDialogService _dialogService;
        private readonly IEventAggregator _ea;
        private readonly IUserSettingsService _settingsService;
        public MainWindowViewModel(IRegionManager regionManager,
            IContainerExtension container,
            IApplicationCommands applicationCommands,
            IDialogService dialogService,
            IEventAggregator ea,
            IUserSettingsService settingsService)
        {
            _regionmanager = regionManager;
            _container = container;
            _applicationCommands = applicationCommands;
            _dialogService = dialogService;
            _ea = ea;
            _settingsService = settingsService;

            RegisterMe();
            SetTimer();
            SetMsgTimer();
            TabCloseCommand = new ActionCommand(OnTabCloseExecuted, OnTabCloseCanExecute);
            CloseCommand = new ActionCommand(OnCloseExecuted, OnCloseCanExecute);
            _applicationCommands.CloseCommand.RegisterCommand(CloseCommand);
            ExplorerCommand = new ActionCommand(OnOpenExplorerExecuted, OnOpenExplorerCanExecute);
            _applicationCommands.ExplorerCommand.RegisterCommand(ExplorerCommand);
            OpenOrderCommand = new ActionCommand(OnOpenOrderExecuted, OnOpenOrderCanExecute);
            _applicationCommands.OpenOrderCommand.RegisterCommand(OpenOrderCommand);
            ArchivateCommand = new ActionCommand(OnArchivateExecuted, OnArchivateCanExecute);
            _applicationCommands.ArchivateCommand.RegisterCommand(ArchivateCommand);
            OpenProjectOverViewCommand = new ActionCommand(OnOpenProjectOverViewExecuted, OnOpenProjectOverViewCanExecute);
            _applicationCommands.OpenProjectOverViewCommand.RegisterCommand(OpenProjectOverViewCommand);
            MachinePrintCommand = new ActionCommand(OnMachinePrintExecuted, OnMachinePrintCanExecute);
            _applicationCommands.MachinePrintCommand.RegisterCommand(MachinePrintCommand);

            OpenLieferlisteCommand = new ActionCommand(OnOpenLieferlisteExecuted, OnOpenLieferlisteCanExecute);
            OpenMachinePlanCommand = new ActionCommand(OnOpenMachinePlanExecuted, OnOpenMachinePlanCanExecute);
            OpenUserMgmtCommand = new ActionCommand(OnOpenUserMgmtExecuted, OnOpenUserMgmtCanExecute);
            OpenRoleMgmtCommand = new ActionCommand(OnOpenRoleMgmtExecuted, OnOpenRoleMgmtCanExecute);
            OpenMachineMgmtCommand = new ActionCommand(OnOpenMachineMgmtExecuted, OnOpenMachineMgmtCanExecute);
            OpenSettingsCommand = new ActionCommand(OnOpenSettingsExecuted, OnOpenSettingsCanExecute);
            OpenArchiveCommand = new ActionCommand(OnOpenArchiveExecuted, OnOpenArchiveCanExecute);
            OpenWorkAreaCommand = new ActionCommand(OnOpenWorkAreaExecuted, OnOpenWorkAreaCanExecute);
            OpenMeasuringCommand = new ActionCommand(OnOpenMeasuringExecuted, OnOpenMeasuringCanExecute);
            OpenProjectCombineCommand = new ActionCommand(OnOpenProjectCombineExecuted, OnOpenProjectCombineCanExecute);

            
            //DbOperations();
        }


        #region Commands
        private bool OnOpenProjectCombineCanExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission(Permissions.OpenProjectCombine);
        }

        private void OnOpenProjectCombineExecuted(object obj)
        {
            _regionmanager.RequestNavigate(RegionNames.MainContentRegion, new Uri("ProjectEdit", UriKind.Relative));
        }

        private bool OnOpenProjectOverViewCanExecute(object arg)
        {
            bool accept = false;
            if (arg is Vorgang)
            {
                accept = true;
            }
            else if (arg is string s)
            {
                if (s.StartsWith("DS")) accept = true;
            }
            return accept && PermissionsProvider.GetInstance().GetUserPermission(Permissions.OpenProject);
        }
        private void OnOpenProjectOverViewExecuted(object obj)
        {
            string param = string.Empty;
            if (obj is Vorgang vrg)
            {
                param = vrg.AidNavigation.ProId ??= string.Empty;
            }
            else
            {
                if (obj is string s)
                {
                    param = s;
                }
            }
            if (param.IsNullOrEmpty()) { return; }

            var par = new DialogParameters();
            par.Add("projectNo", param);
            _dialogService.Show("Projects", par, null);

        }


        private bool OnOpenMeasuringCanExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission(Permissions.OpenMeasure);
        }

        private void OnOpenMeasuringExecuted(object obj)
        {
            _regionmanager.RequestNavigate(RegionNames.MainContentRegion, new Uri("MeasuringRoom", UriKind.Relative));
        }
        private bool OnOpenWorkAreaCanExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission(Permissions.OpenWorkArea);
        }

        private void OnOpenWorkAreaExecuted(object obj)
        {
            _regionmanager.RequestNavigate(RegionNames.MainContentRegion, new Uri("ShowWorkArea", UriKind.Relative));
        }

        private bool OnOpenArchiveCanExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission(Permissions.Archiv);
        }

        private void OnOpenArchiveExecuted(object obj)
        {
            _regionmanager.RequestNavigate(RegionNames.MainContentRegion, new Uri("Archive", UriKind.Relative));
        }

        private void OnArchivateExecuted(object obj)
        {
            if (obj is object[] onr)
            {
                using var db = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
                var v = db.OrderRbs.First(x => x.Aid == (string)onr[0]);
                v.Abgeschlossen = true;
                foreach (var item in v.Vorgangs.Where(x => x.Visability == false))
                {
                    item.Visability = true;
                }
                db.SaveChangesAsync();
                _ea.GetEvent<MessageOrderArchivated>().Publish(v);
            }
        }

        private bool OnArchivateCanExecute(object arg)
        {
            try
            {
                if (arg is object[] onr)
                {
                    if (onr[1] is Boolean f)
                    {
                        return PermissionsProvider.GetInstance().GetUserPermission(Permissions.Archivate) &&
                            (f || Keyboard.IsKeyDown(Key.LeftAlt));
                    }
                }
                return false;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "ArchiveCan", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
        private void OnCloseExecuted(object obj)
        {
            if (obj == null)
            {
                using (var Dbctx = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>())
                {
                    Dbctx.ChangeTracker.DetectChanges();
                    if (Dbctx.ChangeTracker.HasChanges())
                    {
                        var r = MessageBox.Show("Sollen die Änderungen noch in\n die Datenbank gespeichert werden?",
                            "MS SQL Datenbank", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes);
                        if (r == MessageBoxResult.Yes) Dbctx.SaveChanges();
                    }

                    var del = Dbctx.InMemoryOnlines.Where(x => UserInfo.User.UserIdent.Equals(x.Userid)
                     && UserInfo.PC == x.PcId);
                    Dbctx.InMemoryMsgs.Where(x => x.OnlId.Equals(del.First().OnlId)).ExecuteDelete();
                    del.ExecuteDelete();
                }
            }
            else
            {
                _regionmanager.Regions[RegionNames.MainContentRegion].Remove(obj);
            }
        }

        private bool OnCloseCanExecute(object arg)
        {
            return true;
        }

        private bool OnTabCloseCanExecute(object arg)
        {
            return true;
        }

        private void OnTabCloseExecuted(object obj)
        {
            if (obj is FrameworkElement f)
            {
                var vm = f.DataContext as IViewModel;
                vm?.Closing();
            }
            _regionmanager.Regions[RegionNames.MainContentRegion].Remove(obj);
        }
        private static bool OnOpenOrderCanExecute(object arg)
        {
            if (arg is string s)
                if (s.StartsWith("DS"))
                    return false;
            return PermissionsProvider.GetInstance().GetUserPermission(Permissions.Order);
        }
        private void OnOpenOrderExecuted(object parameter)
        {
            var aid = string.Empty;
            if (parameter is Vorgang y) aid = y.Aid;
            else if (parameter is string) aid = (string)parameter;

            if (string.IsNullOrEmpty(aid) == false)
            {
                using (var db = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>())
                {
                    var vrgs = db.Vorgangs
                        .Include(x => x.AidNavigation)
                        .Include(x => x.AidNavigation.DummyMatNavigation)
                        .Include(x => x.AidNavigation.MaterialNavigation)
                        .Include(x => x.AidNavigation.Pro)
                        .Include(x => x.RidNavigation)
                        .Include(x => x.ArbPlSapNavigation)
                        .ThenInclude(x => x.Ressource)
                        .ThenInclude(X => X.WorkArea)
                        .Where(x => x.Aid == aid)
                        .ToList();



                    if (vrgs != null)
                    {
                        var par = new DialogParameters();
                        par.Add("vrgList", vrgs);
                        _dialogService.Show("Order", par, null);
                    }
                }
            }
        }
        private void OnOpenMachineMgmtExecuted(object selectedItem)
        {
            _regionmanager.RequestNavigate(RegionNames.MainContentRegion, new Uri("MachineEdit", UriKind.Relative));
        }

        private bool OnOpenMachineMgmtCanExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission(Permissions.MachEdit);
        }

        private void OnOpenRoleMgmtExecuted(object obj)
        {
            _regionmanager.RequestNavigate(RegionNames.MainContentRegion, new Uri("RoleEdit", UriKind.Relative));
        }

        private bool OnOpenRoleMgmtCanExecute(object arg)
        {

            return PermissionsProvider.GetInstance().GetUserPermission(Permissions.RoleEdit);
        }

        private void OnOpenUserMgmtExecuted(object obj)
        {
            _regionmanager.RequestNavigate(RegionNames.MainContentRegion, new Uri("UserEdit", UriKind.Relative));
        }

        private bool OnOpenUserMgmtCanExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission(Permissions.OpenUserEdit);
        }


        private void OnOpenSettingsExecuted(object obj)
        {

            _regionmanager.RequestNavigate(RegionNames.MainContentRegion, new Uri("UserSettings", UriKind.Relative));

        }

        private bool OnOpenSettingsCanExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission(Permissions.UserSett);
        }

        private void OnOpenMachinePlanExecuted(object obj)
        {
            _regionmanager.RequestNavigate(RegionNames.MainContentRegion, new Uri("MachinePlan", UriKind.Relative));
        }

        private bool OnOpenMachinePlanCanExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission(Permissions.MachPlan);
        }

        private bool OnOpenLieferlisteCanExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission(Permissions.Liefer);
        }

        private void OnOpenLieferlisteExecuted(object obj)
        {
            _regionmanager.RequestNavigate(RegionNames.MainContentRegion, new Uri("Liefer", UriKind.RelativeOrAbsolute));
        }
        private bool OnOpenExplorerCanExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission(Permissions.OpenExpl);
        }
        private void OnOpenExplorerExecuted(object obj)
        {
            Dictionary<string, object>? dic;
            if (obj is OrderViewModel o)
            {
                dic = new Dictionary<string, object>();
                dic.Add("aid", o.Aid);
                dic.Add("ttnr", o.Material ?? string.Empty);
            }
            else if (obj is Vorgang v)
            {
                dic = new Dictionary<string, object>();
                dic.Add("aid", v.Aid);
                dic.Add("ttnr", v.AidNavigation.Material ?? string.Empty);
            }
            else dic = obj as Dictionary<string, object>;
            if (dic != null)

            {
                StringBuilder sb = new();
                String exp = _settingsService.ExplorerPath;
                string[] pa = exp.Split(',');

                Regex reg2 = new(@"(?<=\[)(.*?)(?=\])");


                for (int i = 0; i < pa.Length; i += 2)
                {
                    StringBuilder nsb = new StringBuilder();
                    if (!pa[i].IsNullOrEmpty())
                    {
                        Regex reg3 = new(pa[i]);
                        object? val;

                        if (dic.TryGetValue(reg2.Match(pa[i + 1]).ToString().ToLower(), out val))
                        {
                            if (val is string s)
                            {
                                s = s.Trim();
                                Match match2 = reg3.Match(s);
                                foreach (Group ma in match2.Groups.Values)
                                {
                                    if (ma.Value != s)
                                        nsb.Append(ma.Value).Append(Path.DirectorySeparatorChar);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (dic.TryGetValue(reg2.Match(pa[i + 1]).ToString().ToLower(), out object? val))
                            nsb.Append(val).Append(Path.DirectorySeparatorChar);
                    }
                    sb.Append(nsb.ToString());
                }


                if (!Directory.Exists(_settingsService.ExplorerRoot))
                {
                    MessageBox.Show($"Der Hauptpfad '{_settingsService.ExplorerRoot}'\nwurde nicht gefunden!"
                        , "Error", MessageBoxButton.OK);
                }
                else
                {
                    var p = Path.Combine(@_settingsService.ExplorerRoot, sb.ToString());
                    Process.Start("explorer.exe", @p);
                }
            }
        }
        private bool OnMachinePrintCanExecute(object arg)
        {
            if (arg is PlanMachine plan)
            {
                int wo = plan.WorkArea?.WorkAreaId ?? 0;
                return PermissionsProvider.GetInstance().GetRelativeUserPermission(Permissions.MachPrint, wo);
            }
            return false;
        }

        private void OnMachinePrintExecuted(object obj)
        {
            try
            {
                var print = new PrintDialog();
                var ticket = new PrintTicket();
                ticket.PageMediaSize = new PageMediaSize(PageMediaSizeName.ISOA4);
                ticket.PageOrientation = PageOrientation.Landscape;
                print.PrintTicket = ticket;
                Printing.DoPrintPreview(obj, print);
            }
            catch (System.Exception e)
            {
                MessageBox.Show(e.Message, "MachinePrint", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion
        private void SetTimer()
        {
            // Create a timer with a 30 seconds interval.
            _timer = new System.Timers.Timer(30000);
            // Hook up the Elapsed event for the timer. 
            _timer.Elapsed += OnTimedEvent;
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }
        public int Onlines
        {
            get { return _onlines; }
            private set
            {
                if (_onlines != value)
                {
                    _onlines = value;
                    NotifyPropertyChanged(() => Onlines);
                }
            }
        }
        private void OnTimedEvent(object? sender, ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.InvokeAsync(new Action(() =>
            {
                using var db = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
                Onlines = db.InMemoryOnlines.Count();
            }), System.Windows.Threading.DispatcherPriority.Background);
        }
        private void SetMsgTimer()
        {
            // Create a timer with a 60 seconds interval.
            _timer = new System.Timers.Timer(60000);
            // Hook up the Elapsed event for the timer. 
            _timer.Elapsed += OnMsgTimedEvent;
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }
        private async void OnMsgTimedEvent(object? sender, ElapsedEventArgs e)
        {
            List<string?> msgListV = [];
            List<string?> msgListO = [];
            try
            {
                using var db = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
                {
                    var m = await db.InMemoryMsgs.AsNoTracking()
                        .Include(x => x.Onl)
                        .Where(x => x.Onl.PcId == UserInfo.PC && x.Onl.Userid == UserInfo.User.UserIdent)
                        .ToListAsync();
                    if (m.Count > 0)
                    {
                        msgListV.AddRange(m.Where(x => x.TableName == "Vorgang").Select(x => x.PrimaryKey).ToList());
                        msgListO.AddRange(m.Where(x => x.TableName == "OrderRB").Select(x => x.PrimaryKey).ToList());
                        int IdMin = m.Min(x => x.MsgId), IdMax = m.Max(x => x.MsgId);

                        await db.Database.ExecuteSqlRawAsync(@"DELETE FROM InMemoryMsg WHERE MsgId>={0} AND MsgId<={1}", IdMin, IdMax);
                    }
                }
                Msg = string.Format("{0}-{1}", DateTime.Now.TimeOfDay, msgListO.Count + msgListV.Count);

                if (msgListV.Count > 0)
                    _ea.GetEvent<MessageVorgangChanged>().Publish(msgListV);
                if (msgListO.Count > 0)
                    _ea.GetEvent<MessageOrderChanged>().Publish(msgListO);

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "MsgTimer", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("AsyncUsage", "AsyncFixer03:Fire-and-forget async-void methods or delegates",
            Justification = "<Pending>")]
        private async void RegisterMe()
        {
            try
            {
                using var db = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
                await using var transaction = await db.Database.BeginTransactionAsync();
                var onl = db.InMemoryOnlines.FirstOrDefault(x => x.Userid == UserInfo.User.UserIdent && x.PcId == UserInfo.PC);
                if (onl != null)
                {
                    db.Database.ExecuteSqlRaw("DELETE dbo.InMemoryMsg WHERE OnlId=@p0", onl.OnlId);
                    db.Database.ExecuteSqlRaw("DELETE dbo.InMemoryOnline WHERE OnlId=@p0", onl.OnlId);
                }
                db.Database.ExecuteSqlRaw(@"INSERT INTO dbo.InMemoryOnline(Userid,PcId,Login) VALUES({0},{1},{2})",
                    UserInfo.User.UserIdent,
                    UserInfo.PC ?? string.Empty,
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                await transaction.CommitAsync();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "RegisterMe", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void DbOperations()
        {
            using (var db = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>())
            {
                var ord = db.Vorgangs
                    .Where(x => x.BemM != null || x.BemMa != null || x.BemT != null)
                    .ToList();
                
                foreach(var o in ord)
                { char a;
                    //foreach (var m in o.BemM) { a = Convert.ToChar(m); var b = (char)134; var c = Convert.ToChar(134); }
                    //foreach (var m in o.BemT) a = (char)m;
                    //foreach (var m in o.BemMa) a = (char)m;
                    o.BemM = o.BemM?.Replace(';', (char)29);
                    o.BemT = o.BemT?.Replace(';', (char)29);
                    o.BemMa = o.BemMa?.Replace(';', (char)29);
                }
                db.SaveChanges();
            }
        }

    }
}


