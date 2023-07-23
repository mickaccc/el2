using Lieferliste_WPF.ViewModels.Base;
using Lieferliste_WPF.Utilities;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.Specialized;
using System.Windows.Controls;
using System.Windows.Input;
using Lieferliste_WPF.Commands;
using GongSolutions.Wpf.DragDrop;
using Lieferliste_WPF.Data.Models;
using System.Collections;
using System.Windows.Data;

namespace Lieferliste_WPF.ViewModels
{
    /// <summary>
    /// Class for the main window's view-model.
    /// </summary>
    public sealed class MainWindowViewModel : ViewModelBase, IDropTarget
    {
        public ICommand OpenMachinePlanCommand { get; private set; }
        public ICommand OpenLieferlisteCommand { get; private set; }
        public ICommand OpenSettingsCommand { get; private set; }
        private ObservableCollection<TabItem> _tabTitles;
        private List<TabItem> _windowTitles;

        public MainWindowViewModel()
        {
            TabTitles = new ObservableCollection<TabItem>();
            WindowTitles = new List<TabItem>();
            OpenLieferlisteCommand = new ActionCommand(OnOpenLieferlisteExecuted, OnOpenLieferlisteCanExecute);
            OpenMachinePlanCommand = new ActionCommand(OnOpenMachinePlanExecuted, OnOpenMachinePlanCanExecute);
            OpenSettingsCommand = new ActionCommand(OnOpenSettingsExecuted, OnOpenSettingsCanExecute); 
        }
        public enum Location
        {
            TAB,
            WINDOW
        }
        private struct contentTitle
        {
            public const string Settings = "Einstellungen";
            public const string Deliverylist = "Lieferliste";
            public const string Planning = "Einplanung";
        }
        private void OnOpenSettingsExecuted(object obj)
        {
            if (!TabTitles.Any(x => x.Header.ToString() == contentTitle.Settings))
            {
                TabItem tabItem = new TabItem
                {
                    Content = new View.Settings(),
                    Header = contentTitle.Settings,
                    Tag = Location.TAB,
                    IsSelected = true
                };

                TabTitles.Add(tabItem);
            }
        }

        private bool OnOpenSettingsCanExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission("SET00");
        }

        private void OnOpenMachinePlanExecuted(object obj)
        {
                TabItem tabItem = new TabItem
                {
                    Content = new View.MachinePlan(),
                    Header = contentTitle.Planning,
                    Tag = Location.TAB,
                    IsSelected = true
                    
                };

                TabTitles.Add(tabItem);           
        }

        private bool OnOpenMachinePlanCanExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission("MP00") &&
                !TabTitles.Any(x => x.Header.ToString() == contentTitle.Planning) &&
                !WindowTitles.Any(x => x.Header.ToString() == contentTitle.Planning);
        }

        private void OnOpenLieferlisteExecuted(object obj)
        {
            TabItem tabItem = new TabItem
            {
                Content = new View.Lieferliste(),
                Header = contentTitle.Deliverylist,
                Tag = Location.TAB,
                IsSelected = true
            };
                
            TabTitles.Add(tabItem);           
        }

        private bool OnOpenLieferlisteCanExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission("LIE00") &&
                !TabTitles.Any(x => x.Header.ToString() == contentTitle.Deliverylist);
        }

        public ObservableCollection<TabItem> TabTitles
        {
            get { return _tabTitles; }
            set
            {
                _tabTitles = value;
                OnPropertyChanged("TabTitles");
            }
        }
        public List<TabItem> WindowTitles
        {
            get { return _windowTitles; }
            set
            {
                _windowTitles = value;
                OnPropertyChanged("WindowTitles");
            }
        }

   
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
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
                    int newI = dropInfo.InsertIndex - 1;
                    int oldI = dropInfo.DragInfo.SourceIndex;
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
                    Window wnd = tb.FindName("tabable") as Window;
                    if (wnd != null)
                    {
                        var o = wnd.Owner.OwnedWindows.SyncRoot;
                        
                        wnd.Close();
                    }
                }
            }
        }
        public bool CheckChanges()
        {
            return Dbctx.ChangeTracker.HasChanges();
        }
        public void SaveChanges()
        {
            Dbctx.SaveChanges();
        }
        public event PropertyChangedEventHandler PropertyChanged;

    }

}


