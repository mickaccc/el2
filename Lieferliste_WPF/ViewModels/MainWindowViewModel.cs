using El2Core.Models;
using GongSolutions.Wpf.DragDrop;
using Lieferliste_WPF.Commands;
using Lieferliste_WPF.Interfaces;
using Lieferliste_WPF.Utilities;
using Lieferliste_WPF.Views;
using Lieferliste_WPF.ViewModels.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Prism.Regions;
using El2Core.Utils;
using El2Core.ViewModelBase;
using ModuleRoleEdit.Views;
using Prism.Ioc;
using Unity.Injection;
using El2Core.Constants;

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

        private NotifyTaskCompletion<int> _onlineTask;
        public NotifyTaskCompletion<int> OnlineTask
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
        private ViewPresenter _selectedTab;
        public ViewPresenter SelectedTab
        {
            get { return _selectedTab; }
            set
            {
                if (!_selectedTab.Equals(value))
                {
                    _selectedTab = value;
                    NotifyPropertyChanged(() => SelectedTab);
                }
            }
        }
        private delegate void LieferTaskCompletedEventHandler();
        private Dispatcher dispatcher = Application.Current.Dispatcher;
        private static int _onlines;
        private static System.Timers.Timer _timer;
        private double _progressValue;
        private bool _isLoading;
        
        private ObservableCollection<ViewPresenter> _tabTitles;
        private List<ViewModelBase> _windowTitles;
        private IRegionManager _regionmanager;
        private IContainerExtension _container;
        private RoleEdit _RoleEditView;
        private MachineEdit _MachineEditView;
   
        public MainWindowViewModel(IRegionManager regionManager, IContainerExtension container)
        {
            _regionmanager = regionManager;
            _container = container;
            TabTitles = new ObservableCollection<ViewPresenter>();
            WindowTitles = new List<ViewModelBase>();
            OpenLieferlisteCommand = new ActionCommand(OnOpenLieferlisteExecuted, OnOpenLieferlisteCanExecute);
            OpenMachinePlanCommand = new ActionCommand(OnOpenMachinePlanExecuted, OnOpenMachinePlanCanExecute);
            OpenSettingsCommand = new ActionCommand(OnOpenSettingsExecuted, OnOpenSettingsCanExecute);
            OpenUserMgmtCommand = new ActionCommand(OnOpenUserMgmtExecuted, OnOpenUserMgmtCanExecute);
            OpenRoleMgmtCommand = new ActionCommand(OnOpenRoleMgmtExecuted, OnOpenRoleMgmtCanExecute);
            OpenMachineMgmtCommand = new ActionCommand(OnOpenMachineMgmtExecuted, OnOpenMachineMgmtCanExecute);

            TabCloseCommand = new ActionCommand(OnTabCloseExecuted, OnTabCloseCanExecute);
            CloseCommand = new ActionCommand(OnCloseExecuted, OnCloseCanExecute);

            RegisterMe();
            SetTimer();
        }
 
        #region Commands
        private void OnCloseExecuted(object obj)
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
            TabTitles.Remove(SelectedTab);
            SelectedTab = TabTitles.LastOrDefault();
        }
 
        private void OnOpenMachineMgmtExecuted(object selectedItem)
        {
            var machedit = _container.Resolve<MachineEdit>();
            _regionmanager.AddToRegion(RegionNames.MainContentRegion, machedit);
            _regionmanager.Regions[RegionNames.MainContentRegion].Activate(machedit);
        }

        private bool OnOpenMachineMgmtCanExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission("MA00");
        }

        private void OnOpenRoleMgmtExecuted(object obj)
        {

            _RoleEditView = _container.Resolve<RoleEdit>();
            _regionmanager.AddToRegion(RegionNames.MainContentRegion, _RoleEditView);
            _regionmanager.Regions[RegionNames.MainContentRegion].Activate(_RoleEditView);
        }

        private bool OnOpenRoleMgmtCanExecute(object arg)
        {

            return PermissionsProvider.GetInstance().GetUserPermission("RM00") &&
                _regionmanager.Regions[RegionNames.MainContentRegion].Views.All(x => x.GetType() != typeof(RoleEdit));
        }

        private void OnOpenUserMgmtExecuted(object obj)
        {
            var mgm = _container.Resolve(typeof(UserEdit));
            _regionmanager.AddToRegion(RegionNames.MainContentRegion, mgm);
            _regionmanager.Regions[RegionNames.MainContentRegion].Activate(mgm);
        }

        private bool OnOpenUserMgmtCanExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission("UM00");
        }


        private void OnOpenSettingsExecuted(object obj)
        {
            var sett = _container.Resolve<UserSettings>();
            _regionmanager.AddToRegion(RegionNames.MainContentRegion, sett);
            _regionmanager.Regions[RegionNames.MainContentRegion].Activate(sett);
        }

        private bool OnOpenSettingsCanExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission("SET00") &&
                _regionmanager.Regions.All(x => !x.Views.Matches(typeof(UserSettings)));
        }

        private void OnOpenMachinePlanExecuted(object obj)
        {
            var machPlan = _container.Resolve<MachinePlan>();
            _regionmanager.RegisterViewWithRegion(RegionNames.MainContentRegion,() => machPlan);
            _regionmanager.Regions[RegionNames.MainContentRegion].Activate(machPlan);
        }

        private bool OnOpenMachinePlanCanExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission("MP00") &&
                !_regionmanager.Regions.All(x => !x.Views.Matches(typeof(MachinePlan)));
        }

        private bool OnOpenLieferlisteCanExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission("LIE00");
        }
        private void OnOpenLieferlisteExecuted(object obj)
        {
            var ll = _container.Resolve<Liefer>();
            _regionmanager.AddToRegion(RegionNames.MainContentRegion, ll);
            _regionmanager.Regions[RegionNames.MainContentRegion].Activate(ll);
            //ViewPresenter present = new ViewPresenter();
            //present.ViewType = typeof(Liefer);
            //present.Key = "lief";
            //present.Title = "Lieferliste";
            
            //DisplayVM(present);
   

            //LieferTask = new NotifyTaskCompletion<Page?>(OnLoadAsync(ll));

            //var ll = Dispatcher.BeginInvoke( DispatcherPriority.Normal, new LieferTaskCompletedEventHandler(OnComplete));
            //if (ll.Result != null )
            //Lieferliste ll = new Lieferliste()
            //{
            //    Title = ContentTitle.Deliverylist,
            //    Tag = ContentTitle.Deliverylist
            //};
            //TaskComplete = ll.TaskCompletion;
            //TaskComplete.PropertyChanged += TaskChanged;

        }

        private void TaskChanged(object? sender, PropertyChangedEventArgs e)
        {

                //if (LieferTask.IsSuccessfullyCompleted)
                //{
                //    TabTitles.Add(LieferTask.Result);
                //}
            
        }


        #endregion
        private void SetTimer()
        {
            // Create a timer with a 1 minute interval.
            _timer = new System.Timers.Timer(60000);
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
                if(_onlines != value)
                {
                   _onlines = value;
                   NotifyPropertyChanged(() => Onlines);
                }
            }
        }
        private void OnTimedEvent(object? sender, ElapsedEventArgs e)
        {
            using (var Dbctx = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>())
                OnlineTask = new NotifyTaskCompletion<int>(Dbctx.Onlines.CountAsync());
        }

        public ObservableCollection<ViewPresenter> TabTitles
        {
            get { return _tabTitles; }
            set
            {
                _tabTitles = value;
                NotifyPropertyChanged(() => TabTitles);
            }
        }
        public List<ViewModelBase> WindowTitles
        {
            get { return _windowTitles; }
            set
            {
                _windowTitles = value;
                NotifyPropertyChanged(() => WindowTitles);
            }
        }

        public double ProgressValue { get { return _progressValue; } set { _progressValue = value; } }
        public bool IsLoading { get { return _isLoading; }
            private set
            {
                if (_isLoading != value)
                { 
                    _isLoading = value;
                    NotifyPropertyChanged(() => IsLoading);
                }
            }
        }

        public NotifyTaskCompletion<ObservableCollection<ViewModelBase>> TaskComplete { get; private set; }

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

        private  void RegisterMe()
        {
            //var ap = ServiceLocator.Current.GetInstance<AppStatic>();

            using (var db = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>())
            {
                db.Database.ExecuteSqlRaw(@"INSERT INTO dbo.Online(UserId,PcId,Login) VALUES({0},{1},{2})",
                    UserInfo.User.UserIdent,
                    UserInfo.PC ?? string.Empty,
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            }
        }

    }
    public struct ViewPresenter
    {
        public string Title;
        public string Key;
        public Type ViewType;
    }

}


