using CompositeCommands.Core;
using El2Core.Constants;
using El2Core.Models;
using El2Core.Utils;
using El2Core.ViewModelBase;
using GongSolutions.Wpf.DragDrop;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Prism.Ioc;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
        private readonly IContainerExtension _container;
        private readonly IDialogService _dialogService;
        public MainWindowViewModel(IRegionManager regionManager,
            IContainerExtension container,
            IApplicationCommands applicationCommands,
            IDialogService dialogService)
        {
            _regionmanager = regionManager;
            _container = container;
            _applicationCommands = applicationCommands;
            _dialogService = dialogService;

            RegisterMe();
            SetTimer();
            TabCloseCommand = new ActionCommand(OnTabCloseExecuted, OnTabCloseCanExecute);
            CloseCommand = new ActionCommand(OnCloseExecuted, OnCloseCanExecute);
            _applicationCommands.CloseCommand.RegisterCommand(CloseCommand);
            ExplorerCommand = new ActionCommand(OnOpenExplorerExecuted, OnOpenExplorerCanExecute);
            _applicationCommands.ExplorerCommand.RegisterCommand(ExplorerCommand);
            OpenOrderCommand = new ActionCommand(OnOpenOrderExecuted, OnOpenOrderCanExecute);
            _applicationCommands.OpenOrderCommand.RegisterCommand(OpenOrderCommand);

            OpenLieferlisteCommand = new ActionCommand(OnOpenLieferlisteExecuted, OnOpenLieferlisteCanExecute);
            OpenMachinePlanCommand = new ActionCommand(OnOpenMachinePlanExecuted, OnOpenMachinePlanCanExecute);
            OpenUserMgmtCommand = new ActionCommand(OnOpenUserMgmtExecuted, OnOpenUserMgmtCanExecute);
            OpenRoleMgmtCommand = new ActionCommand(OnOpenRoleMgmtExecuted, OnOpenRoleMgmtCanExecute);
            OpenMachineMgmtCommand = new ActionCommand(OnOpenMachineMgmtExecuted, OnOpenMachineMgmtCanExecute);
            OpenSettingsCommand = new ActionCommand(OnOpenSettingsExecuted, OnOpenSettingsCanExecute);
            OpenArchiveCommand = new ActionCommand(OnOpenArchiveExecuted, OnOpenArchiveCanExecute);

        }

        private bool OnOpenArchiveCanExecute(object arg)
        {
            return true;
        }

        private void OnOpenArchiveExecuted(object obj)
        {
            _regionmanager.RequestNavigate(RegionNames.MainContentRegion, new Uri("Archive", UriKind.Relative));
        }

        #region Commands
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
                String exp = Properties.Settings.Default.ExplorerPath;
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


                if (!Directory.Exists(Properties.Settings.Default.ExplorerRoot))
                {
                    MessageBox.Show($"Der Hauptpfad '{Properties.Settings.Default.ExplorerRoot}'\nwurde nicht gefunden!"
                        , "Error", MessageBoxButton.OK);
                }
                else
                {
                    var p = Path.Combine(@Properties.Settings.Default.ExplorerRoot, sb.ToString());
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


