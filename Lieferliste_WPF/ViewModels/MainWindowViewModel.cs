﻿using CompositeCommands.Core;
using El2Core.Constants;
using El2Core.Models;
using El2Core.Services;
using El2Core.Utils;
using El2Core.ViewModelBase;
using GongSolutions.Wpf.DragDrop;
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
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Lieferliste_WPF.ViewModels
{
    /// <summary>
    /// Class for the main window's view-model.
    /// </summary>
    [System.Runtime.Versioning.SupportedOSPlatform("windows7.0")]
    public class MainWindowViewModel : ViewModelBase, IDropTarget
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
            return PermissionsProvider.GetInstance().GetUserPermission(Permissions.OpenProject);
        }
        private void OnOpenProjectOverViewExecuted(object obj)
        {
            var para = obj as Vorgang;
            if (para != null) {
                var pro = para.AidNavigation.ProId;
                if(string.IsNullOrEmpty(pro)) { return; }
                var par = new DialogParameters();
                par.Add("projectNo", pro);
                _dialogService.Show("Projects", par, null);
            }
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
                db.SaveChanges();
                _ea.GetEvent<MessageOrderChanged>().Publish([v.Aid]);
            }
        }

        private bool OnArchivateCanExecute(object arg)
        {
            try
            {
                if (PermissionsProvider.GetInstance().GetUserPermission(Permissions.Archivate))
                {
                    if (arg is object[] onr)
                    {
                        if (onr[1] is Boolean f)
                        {
                            return f || (Keyboard.IsKeyDown(Key.LeftAlt));
                        }
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

                    var del = Dbctx.Onlines.Where(x => x.UserId.Equals(UserInfo.User.UserIdent)
                     && x.PcId.Equals(UserInfo.PC)).ExecuteDelete();
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
                if (vm != null) vm.Closing();
            }
            
            _regionmanager.Regions[RegionNames.MainContentRegion].Remove(obj);
        }
        private static bool OnOpenOrderCanExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission(Permissions.Order);
        }
        private void OnOpenOrderExecuted(object parameter)
        {
            if (parameter != null)
            {
                string? para;
                if (parameter is Vorgang y) para = y.Aid; else para = parameter.ToString();
                OrderRb? ord;
                using (var db = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>())
                {
                    ord = db.OrderRbs
                        .Include(x => x.MaterialNavigation)
                        .Include(x => x.DummyMatNavigation)
                        .Include(x => x.Vorgangs)
                        .ThenInclude(x => x.RidNavigation)
                        .FirstOrDefault(x => x.Aid == para);
                }
                if (ord != null)
                {
                    var par = new DialogParameters();
                    par.Add("vrgList", ord);
                    _dialogService.Show("Order", par, null);
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
            var Dbctx = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
            OnlineTask = new NotifyTaskCompletion<int>(Dbctx.Onlines.CountAsync());
            if (OnlineTask.IsCompleted) { Dbctx.Dispose(); }
        }
        private void SetMsgTimer()
        {
            // Create a timer with a 2 seconds interval.
            _timer = new System.Timers.Timer(15000);
            // Hook up the Elapsed event for the timer. 
            _timer.Elapsed += OnMsgTimedEvent;
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }
        private async void OnMsgTimedEvent(object? sender, ElapsedEventArgs e)
        {
            List<string> msgListV = [];
            List<string> msgListO = [];
                await Task.Run(() =>
                {
                    lock (_lock)
                    {
                        using var db = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
                        {
                            var m = db.Msgs.AsNoTracking()
                                .Include(x => x.Onl)
                                .Where(x => x.Onl.PcId == UserInfo.PC && x.Onl.UserId == UserInfo.User.UserIdent)
                                .ToListAsync();

                            foreach (var msg in m.Result)
                            {
                                if(msg.PrimaryKey != null && msg.TableName == "Vorgang")
                                    msgListV.Add(msg.PrimaryKey);

                                if (msg.PrimaryKey != null && msg.TableName == "OrderRB")
                                    msgListO.Add(msg.PrimaryKey);
                                db.Database.ExecuteSqlRaw(@"DELETE FROM msg WHERE id={0}", msg.Id);
                            }

                        }
                    }
                }, CancellationToken.None);
                if(msgListV.Count > 0) 
                _ea.GetEvent<MessageVorgangChanged>().Publish(msgListV);
            if (msgListO.Count > 0)
                _ea.GetEvent<MessageOrderChanged>().Publish(msgListO);

        }
        private struct ContentTitle
        {
            public const string Settings = "Einstellungen";
            public const string Deliverylist = "Lieferliste";
            public const string Planning = "Teamleiter Zuteilung";
            public const string RoleEdit = "Rollen Management";
            public const string MachineEdit = "Maschinen Management";
            public const string UserEdit = "User Managment";

        }

        public void DragOver(IDropInfo dropInfo)
        {
            if (dropInfo.Data is TabItem)
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                dropInfo.Effects = DragDropEffects.Move;
            }
        }

        public void Drop(IDropInfo dropInfo)
        {
            //if(dropInfo.Data is CodeTypeOfExpression( pg))
            //{
            //    if(TabTitles.Contains(pg))
            //    {
            //        var newI = dropInfo.InsertIndex - 1;
            //        var oldI = dropInfo.DragInfo.SourceIndex;
            //        if (newI > TabTitles.Count-1) newI = TabTitles.Count-1;
            //        if (newI < 0) newI = 0;
            //        if (newI != oldI)
            //        {
            //            TabTitles.Move(oldI, newI);
            //        }
            //    }
            //    else
            //    {
            //        TabTitles.Add(pg);
            //        //WindowTitles.Remove(pg);
            //        //if (pg.FindName("tabable") is Window wnd)
            //        //{
            //        //    var o = wnd.Owner.OwnedWindows.SyncRoot;

            //        //    wnd.Close();
            //        //}
            //    }
            //}
        }

        private void RegisterMe()
        {
            using (var db = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>())
            {
                db.Database.ExecuteSqlRaw(@"INSERT INTO dbo.Online(UserId,PcId,Login) VALUES({0},{1},{2})",
                    UserInfo.User.UserIdent,
                    UserInfo.PC ?? string.Empty,
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            }
        }
        private void DBOperation()
        {
            using (var db = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>())
            {
                var vorg = db.WorkSaps.Where(x => x.WorkSapId != null).Select(x => x.WorkSapId).ToList();
                var varb = vorg.Distinct();
                foreach (var v in varb)
                {
                    if (v.Length > 3)
                    {
                        string cost = v[..3];
                        string inv = v[3..];
                        int costid;
                        if (int.TryParse(cost, out costid))
                        {
                            var ress = db.Ressources.Where(x => x.Inventarnummer == inv);
                            var id = ress.FirstOrDefault().RessourceId;
                            foreach (var r in ress.Skip(1))
                            {
                                db.Ressources.Remove(r);
                            }
                            if (id != null)
                            {
                                var res = db.RessourceCostUnits.FirstOrDefault(x => x.CostId == costid && x.Rid == id);
                                if (res == null)
                                    db.RessourceCostUnits.Add(new RessourceCostUnit() { CostId = costid, Rid = id });
                            }
                            db.SaveChanges();
                        }
                    }
                }
            }
        }
    }
}


