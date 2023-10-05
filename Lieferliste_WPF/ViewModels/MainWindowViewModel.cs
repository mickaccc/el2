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
        public NotifyTaskCompletion<TabItem?> LieferTask { get; private set; }
        private delegate void LieferTaskCompletedEventHandler();
        private Dispatcher Dispatcher { get; set; }
        private static int _onlines;
        private static System.Timers.Timer _timer;
        private double _progressValue;
        private bool _progressIsBusy;
        
        private ObservableCollection<TabItem> _tabTitles;
        private List<TabItem> _windowTitles;

        public MainWindowViewModel()
        {
            Dispatcher = Dispatcher.CurrentDispatcher;
            TabTitles = new ObservableCollection<TabItem>();
            WindowTitles = new List<TabItem>();
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
            if (obj is TabItem o)
            {
                var t = TabTitles.FirstOrDefault(x => x.Content == o.Content);
                if (t != null)
                    TabTitles.Remove(t);
            }
        }

        private void OnOpenMachineMgmtExecuted(object obj)
        {
            TabItem tabItem = new()
            {
                Content = new MachineEdit(),
                Header = new TabHeader() { HeaderText = ContentTitle.MachineEdit },
                Tag = ContentTitle.MachineEdit,
                IsSelected = true
            };

            TabTitles.Add(tabItem);
        }

        private bool OnOpenMachineMgmtCanExecute(object arg)
        {          
            return PermissionsProvider.GetInstance().GetUserPermission("MA00") &&
                TabTitles.All(x => x.Tag.ToString() != ContentTitle.MachineEdit) &&
                WindowTitles.All(x => x.Tag.ToString() != ContentTitle.MachineEdit);
        }

        private void OnOpenRoleMgmtExecuted(object obj)
        {
            TabItem tabItem = new()
            {
                Content = new RoleEdit(),
                Header = new TabHeader() { HeaderText = ContentTitle.RoleEdit },
                Tag = ContentTitle.RoleEdit,
                IsSelected = true
            };

            TabTitles.Add(tabItem);
        }

        private bool OnOpenRoleMgmtCanExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission("RM00") &&
                TabTitles.All(x => x.Tag.ToString() != ContentTitle.RoleEdit) &&
                WindowTitles.All(x => x.Tag.ToString() != ContentTitle.RoleEdit);
        }

        private void OnOpenUserMgmtExecuted(object obj)
        {
            TabItem tabItem = new()
            {
                Content = new UserEdit(),
                Header = new TabHeader() { HeaderText = ContentTitle.UserEdit },
                Tag = ContentTitle.UserEdit,
                IsSelected = true
            };

            TabTitles.Add(tabItem);

        }

        private bool OnOpenUserMgmtCanExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission("UM00") &&
                !TabTitles.Any(x => x.Tag.ToString() == ContentTitle.UserEdit) &&
                !WindowTitles.Any(x => x.Tag.ToString() == ContentTitle.UserEdit);
        }


        private void OnOpenSettingsExecuted(object obj)
        {
            if (!TabTitles.Any(x => x.Header.ToString() == ContentTitle.Settings))
            {
                TabItem tabItem = new()
                {
                    Content = new Settings(),
                    Header = new TabHeader() { HeaderText = ContentTitle.Settings },
                    Tag = ContentTitle.Settings,
                    IsSelected = true
                };

                TabTitles.Add(tabItem);
            }
        }

        private bool OnOpenSettingsCanExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission("SET00") &&
                TabTitles.All(x => x.Tag.ToString() != ContentTitle.Settings) &&
                WindowTitles.All(x => x.Tag.ToString() != ContentTitle.Settings);
        }

        private void OnOpenMachinePlanExecuted(object obj)
        {
            TabItem tabItem = new()
            {
                Content = new View.MachinePlan(),
                Header = new TabHeader() { HeaderText = ContentTitle.Planning },
                Tag = ContentTitle.Planning,
                IsSelected = true

            };

            TabTitles.Add(tabItem);
        }

        private bool OnOpenMachinePlanCanExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission("MP00") &&
                !TabTitles.Any(x => x.Tag.ToString() == ContentTitle.Planning) &&
                !WindowTitles.Any(x => x.Tag.ToString() == ContentTitle.Planning);
        }

        private bool OnOpenLieferlisteCanExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission("LIE00") &&
                TabTitles.All(x => x.Tag.ToString() != ContentTitle.Deliverylist) &&
                WindowTitles.All(x => x.Tag.ToString() != ContentTitle.Deliverylist);

        }
        private void OnOpenLieferlisteExecuted(object obj)
        {
            
            //LieferTask = new NotifyTaskCompletion<TabItem?>(CreateTabItemAsync());
            Dispatcher.BeginInvoke( DispatcherPriority.Normal, new LieferTaskCompletedEventHandler(OnComplete));
        }

        private void OnComplete()
        {
            var ll = new Lieferliste();
            TabItem? tabItem = null;
            tabItem = new TabItem
            {
                Content = ll,
                Header = new TabHeader() { HeaderText = ContentTitle.Deliverylist },
                Tag = ContentTitle.Deliverylist,
                IsSelected = true
            };
            TabTitles.Add(tabItem);
        }


        #endregion
        private void SetTimer()
        {
            // Create a timer with a two second interval.
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

        public ObservableCollection<TabItem> TabTitles
        {
            get { return _tabTitles; }
            set
            {
                _tabTitles = value;
                NotifyPropertyChanged(() => TabTitles);
            }
        }
        public List<TabItem> WindowTitles
        {
            get { return _windowTitles; }
            set
            {
                _windowTitles = value;
                NotifyPropertyChanged(() => WindowTitles);
            }
        }

        public double ProgressValue { get { return _progressValue; } set { _progressValue = value; } }
        public bool ProgressIsBusy { get { return _progressIsBusy; }
            private set
            {
                _progressIsBusy = value;
                NotifyPropertyChanged(() => ProgressIsBusy);
            } }

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
            if(dropInfo.Data is TabItem tb)
            {
                if(TabTitles.Contains(tb))
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
                    TabTitles.Add(tb);
                    WindowTitles.Remove(tb);
                    if (tb.FindName("tabable") is Window wnd)
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

