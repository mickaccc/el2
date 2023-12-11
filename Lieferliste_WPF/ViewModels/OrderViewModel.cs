
using CompositeCommands.Core;
using El2Core.Models;
using El2Core.Utils;
using El2Core.ViewModelBase;
using Prism.Services.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;

namespace Lieferliste_WPF.ViewModels
{
    internal class OrderViewModel : ViewModelBase, IDialogAware
    {
        public OrderViewModel(IApplicationCommands applicationCommands)
        {
            _applicationCommands = applicationCommands;
            VorgangCV = CollectionViewSource.GetDefaultView(Vorgangs);
            SaveCommand = new ActionCommand(OnSaveExecuted, OnSaveCanExecute);
        }

        private bool OnSaveCanExecute(object arg)
        {
            return false;
        }

        private void OnSaveExecuted(object obj)
        {
            
        }

        private ObservableCollection<Vorgang> Vorgangs { get; } = new();

        public event Action<IDialogResult> RequestClose;
        private string _aid;
        public string Aid
        {
            get { return _aid; }
            set
            {
                if (_aid != value)
                {
                    _aid = value;
                    NotifyPropertyChanged(() => Aid);
                }
            }
        }
        private string? _material;
        public string? Material
        {
            get { return _material; }
            set
            {
                if (_material != value)
                {
                    _material = value;
                    NotifyPropertyChanged(() => Material);
                }
            }
        }
        private string? _bezeichnung;
        public string? Bezeichnung
        {
            get { return _bezeichnung; }
            set
            {
                if (_bezeichnung != value)
                {
                    _bezeichnung = value;
                    NotifyPropertyChanged(() => Bezeichnung);
                }
            }
        }
        private int? _quantity;

        public int? Quantity
        {
            get { return _quantity; }
            set
            {
                if (_quantity != value)
                {
                    _quantity = value;
                    NotifyPropertyChanged(() => Quantity);
                }
            }
        }
 
        private string? _pro;

        public string? Pro
        {
            get { return _pro; }
            set
            {
                if (value != _pro)
                {
                    _pro = value;
                    NotifyPropertyChanged(() => Pro);
                }
            }
        }
        private string? _proInfo;

        public string? ProInfo
        {
            get { return _proInfo; }
            set
            {
                if ((_proInfo != value))
                {
                    _proInfo = value;
                    NotifyPropertyChanged(() => ProInfo);
                }
            }
        }
        private string? _sysStatus;

        public string? SysStatus
        {
            get { return _sysStatus; }
            set
            {
                if (_sysStatus != value)
                {
                    _sysStatus = value;
                    NotifyPropertyChanged(() => SysStatus);
                }
            }
        }

        public ICollectionView VorgangCV { get; }
        private string _title = "Auftragsübersicht";
        public string Title
        {
            get { return _title; }
            set
            {
                if (_title != value)
                {
                    _title = value;
                    NotifyPropertyChanged(() => Title);
                }
            }
        }
        private IApplicationCommands _applicationCommands;
        public IApplicationCommands ApplicationCommands
        {
            get { return _applicationCommands; }
            set
            {
                if (_applicationCommands != value)
                    _applicationCommands = value;
                NotifyPropertyChanged(() => ApplicationCommands);
            }
        }
        public ICommand SaveCommand;
        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {

        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            var p = parameters.GetValue<OrderRb>("vrgList");

            foreach (var item in p.Vorgangs)
            {
                Vorgangs.Add(item);
            }

            Aid = p.Aid;
            Material = p.Material;
            Bezeichnung = p.MaterialNavigation?.Bezeichng;
            Quantity = p.Quantity;
            Pro = p.ProId;
            ProInfo = p.ProId;
            SysStatus = p.SysStatus;

            VorgangCV.Refresh();
        }
    }
}
