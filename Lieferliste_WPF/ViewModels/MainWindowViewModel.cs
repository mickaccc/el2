﻿using GongSolutions.Wpf.DragDrop;
using Lieferliste_WPF.Commands;
using Lieferliste_WPF.Interfaces;
using El2UserControls;
using Lieferliste_WPF.Utilities;
using Lieferliste_WPF.View;
using Lieferliste_WPF.ViewModels.Base;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using El2Utilities.Utils;
using System.Collections.Generic;
using System.Linq;
using System;
using El2Utilities.Models;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Lieferliste_WPF.ViewModels
{
    /// <summary>
    /// Class for the main window's view-model.
    /// </summary>
    [System.Runtime.Versioning.SupportedOSPlatform("windows7.0")]
    public sealed class MainWindowViewModel : ViewModelBase, IDropTarget, IProgressbarInfo
    {
        public ICommand OpenMachinePlanCommand { get; private set; }
        public ICommand OpenLieferlisteCommand { get; private set; }
        public ICommand OpenSettingsCommand { get; private set; }
        public ICommand OpenUserMgmtCommand { get; private set; }
        public ICommand OpenRoleMgmtCommand { get; private set; }
        public ICommand OpenMachineMgmtCommand { get; private set; }
        public ICommand TabCloseCommand { get; private set; }
        public ICommand CloseCommand { get; private set; }
        public NotifyTaskCompletion<Page?> LieferTask { get; private set; }
        private delegate void LieferTaskCompletedEventHandler();
        private Dispatcher dispatcher = Application.Current.Dispatcher;
        private static int _onlines;
        private static System.Timers.Timer _timer;
        private double _progressValue;
        private bool _isLoading;
        
        private ObservableCollection<Page> _tabTitles;
        private List<Page> _windowTitles;

        public MainWindowViewModel()
        {

            TabTitles = new ObservableCollection<Page>();
            WindowTitles = new List<Page>();
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
        private static void OnCloseExecuted(object obj)
        {
            Dbctx.ChangeTracker.DetectChanges();
            if (Dbctx.ChangeTracker.HasChanges())
            {
                var r = MessageBox.Show("Sollen die Änderungen noch in\n die Datenbank gespeichert werden?",
                    "MS SQL Datenbank", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes);
                if (r == MessageBoxResult.Yes) Dbctx.SaveChanges();
            }

           var del = Dbctx.Onlines.Where(x => x.UserId.Equals(AppStatic.User.UserIdent)
            && x.PcId.Equals(AppStatic.PC)).ExecuteDelete();

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
            if (obj is Page o)
            {
                var t = TabTitles.FirstOrDefault(x => x.Content == o.Content);
                if (t != null)
                    TabTitles.Remove(t);
            }
        }

        private void OnOpenMachineMgmtExecuted(object obj)
        {
 
            var me = new MachineEdit()
            {
                Title = ContentTitle.MachineEdit,
                Tag = ContentTitle.MachineEdit
            };
            TabTitles.Add(me);
        }

        private bool OnOpenMachineMgmtCanExecute(object arg)
        {          
            return PermissionsProvider.GetInstance().GetUserPermission("MA00") &&
                TabTitles.All(x => typeof(MachineEdit).Equals(x)) &&
                WindowTitles.All(x => typeof(MachineEdit).Equals(x));
        }

        private void OnOpenRoleMgmtExecuted(object obj)
        {
  
            var re = new RoleEdit()
            {
                Title = ContentTitle.RoleEdit,
                Tag = ContentTitle.RoleEdit
            };
            TabTitles.Add(re);
        }

        private bool OnOpenRoleMgmtCanExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission("RM00") &&
                TabTitles.All(x => typeof(RoleEdit).Equals(x)) &&
                WindowTitles.All(x => typeof(RoleEdit).Equals(x));
        }

        private void OnOpenUserMgmtExecuted(object obj)
        {

            var ue = new UserEdit()
            {
                Title = ContentTitle.UserEdit,
                Tag = ContentTitle.UserEdit
            };
            TabTitles.Add(ue);

        }

        private bool OnOpenUserMgmtCanExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission("UM00") &&
                TabTitles.All(x => typeof(UserEdit).Equals(x)) &&
                WindowTitles.All(x => typeof(UserEdit).Equals(x));
        }


        private void OnOpenSettingsExecuted(object obj)
        {
 
            var sett = new Settings()
            {
                Title = ContentTitle.Settings
            };
                TabTitles.Add(sett);
            
        }

        private bool OnOpenSettingsCanExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission("SET00") &&
                TabTitles.All(x => typeof(Settings).Equals(x)) &&
                WindowTitles.All(x => typeof(Settings).Equals(x));
        }

        private void OnOpenMachinePlanExecuted(object obj)
        {

            var mp = new MachinePlan
            {
                Title = ContentTitle.Planning
            };
            TabTitles.Add(mp);
        }

        private bool OnOpenMachinePlanCanExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission("MP00") &&
                !TabTitles.Any(x => typeof(MachinePlan).Equals(x)) &&
                !WindowTitles.Any(x => typeof(MachinePlan).Equals(x));
        }

        private bool OnOpenLieferlisteCanExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission("LIE00") &&
                !TabTitles.Any(x => typeof(Lieferliste).Equals(x)) &&
                !WindowTitles.Any(x => typeof(Lieferliste).Equals(x));
        }
        private void OnOpenLieferlisteExecuted(object obj)
        {
           dispatcher.BeginInvoke((Action)(() =>
           {         
                var ll = new Lieferliste()
                {
                    Title = ContentTitle.Deliverylist
                };
                TabTitles.Add(ll);
           })); 
            
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

                if (LieferTask.IsSuccessfullyCompleted)
                {
                    TabTitles.Add(LieferTask.Result);
                }
            
        }

        private static async Task<Page?> OnLoadAsync(Page view)
        {

            IViewModel vm = (IViewModel)view.DataContext;
            vm.LoadDataAsnc();
            return (Page?)vm;
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
            
            Onlines = Dbctx.Onlines.Count();
        }

        public ObservableCollection<Page> TabTitles
        {
            get { return _tabTitles; }
            set
            {
                _tabTitles = value;
                NotifyPropertyChanged(() => TabTitles);
            }
        }
        public List<Page> WindowTitles
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
            public const string Planning = "Einplanung";
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
            if(dropInfo.Data is Page pg)
            {
                if(TabTitles.Contains(pg))
                {
                    var newI = dropInfo.InsertIndex - 1;
                    var oldI = dropInfo.DragInfo.SourceIndex;
                    if (newI > TabTitles.Count-1) newI = TabTitles.Count-1;
                    if (newI < 0) newI = 0;
                    if (newI != oldI)
                    {
                        TabTitles.Move(oldI, newI);
                    }
                }
                else
                {
                    TabTitles.Add(pg);
                    WindowTitles.Remove(pg);
                    if (pg.FindName("tabable") is Window wnd)
                    {
                        var o = wnd.Owner.OwnedWindows.SyncRoot;

                        wnd.Close();
                    }
                }
            }
        }

        private static void RegisterMe()
        {
            using DB_COS_LIEFERLISTE_SQLContext db = new();
            var onl = db.Onlines;
            onl.Add(new Online() { UserId = AppStatic.User.UserIdent, PcId = AppStatic.PC,Login = DateTime.Now });
            db.SaveChanges();
        }



        public void SetProgressIsBusy()
        {
            throw new NotImplementedException();
        }

    }

}


