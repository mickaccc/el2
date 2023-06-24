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
using System.Windows.Forms.Automation;
using System.Windows.Input;
using Lieferliste_WPF.Commands;

namespace Lieferliste_WPF.ViewModels
{
    /// <summary>
    /// Class for the main window's view-model.
    /// </summary>
    public sealed class MainWindowViewModel : ViewModelBase, INotifyPropertyChanged
    {
        public ICommand OpenMachinePlanCommand { get; private set; }
        public ICommand OpenLieferlisteCommand { get; private set; }
        public ICommand OpenSettingsCommand { get; private set; }
        private ObservableCollection<TabItem> _tabTitles;
        private List<Page> _windowTitles;

        public MainWindowViewModel()
        {
            TabTitles = new ObservableCollection<TabItem>();
            WindowTitles = new List<Page>();
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
            if (!TabTitles.Any(x => x.Header.ToString() == contentTitle.Planning) &&
                !WindowTitles.Any(x => x.Title == contentTitle.Planning))
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
        }

        private bool OnOpenMachinePlanCanExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission("MP00");
        }

        private void OnOpenLieferlisteExecuted(object obj)
        {
            if (!TabTitles.Any(x => x.Header.ToString() == contentTitle.Deliverylist))
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
        }

        private bool OnOpenLieferlisteCanExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission("LIE00");
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
        public List<Page> WindowTitles
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
        public event PropertyChangedEventHandler PropertyChanged;

    }

}


