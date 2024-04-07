using CompositeCommands.Core;
using El2Core.Models;
using Prism.Commands;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ModulePlanning.Dialogs.ViewModels
{
    [System.Runtime.Versioning.SupportedOSPlatform("windows10.0")]
    public class HistoryDialogVM : IDialogAware
    {
        public HistoryDialogVM(IApplicationCommands applicationCommands) { _applicationCommands = applicationCommands; }
        public string Title => "Historische Aufträge";
        public string? Material => _material;

        private string? _vid;
        private string? _material;
        public string? MatDescription => _matDescription;
        private string? _matDescription;
        public int VrgNr => _vrgNr;
        private int _vrgNr;
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
        private IApplicationCommands? _applicationCommands;
        private List<Vorgang>? _orderList;
        public ICollectionView? OrderList { get; private set; }
        
        public event Action<IDialogResult>? RequestClose;
        private DelegateCommand<Vorgang?>? _closeDialogCommand;
        public DelegateCommand<Vorgang?> CloseDialogCommand =>
            _closeDialogCommand ?? (_closeDialogCommand = new DelegateCommand<Vorgang?>(CloseDialog));

        protected virtual void CloseDialog(Vorgang? parameter)
        {
            ButtonResult result = ButtonResult.None;
            IDialogParameters param = new DialogParameters();

            if (parameter == null)
                result = ButtonResult.Cancel;
            else if (parameter is Vorgang v)
            {
                result = ButtonResult.Yes;
                param.Add("Comment", v.BemT);
                param.Add("VID", _vid);
            }
            RaiseRequestClose(new DialogResult(result, param));
        }
        public virtual void RaiseRequestClose(IDialogResult dialogResult)
        {
            RequestClose?.Invoke(dialogResult);
        }
        public bool CanCloseDialog()
        {
            return true;
        }
        public void OnDialogClosed()
        {
 
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            var ord = parameters.GetValue<List<OrderRb>>("orderList");
            var vnr = parameters.GetValue<short>("VNR");
            _vid = parameters.GetValue<string>("VID");

            _material = ord.First().Material;
            _matDescription = ord.First().MaterialNavigation?.Bezeichng;
            _vrgNr = vnr;
            var list = new List<Vorgang>();
            foreach (var o in ord.OrderByDescending(x => x.Eckende))
            {
  
                for(int i = 0; i < o.Vorgangs.Count; i++)
                {
                    if (o.Vorgangs.ElementAt(i).Vnr >= vnr)
                    {
                        
                        var e = o.Vorgangs.ElementAtOrDefault(i - 1);
                        if (e != null) list.Add(e);
                        list.Add(o.Vorgangs.ElementAt(i));
                        e = o.Vorgangs.ElementAtOrDefault(i  + 1);
                        if (e != null) list.Add(e);
                        break;
                    }
                }
            }
            _orderList = new List<Vorgang>(list);
            OrderList = CollectionViewSource.GetDefaultView(_orderList);
            OrderList.GroupDescriptions.Add(new PropertyGroupDescription("Aid"));
        }
    }
}
