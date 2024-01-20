using El2Core.Models;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Lieferliste_WPF.Dialogs
{
    class HistoryDialogVM : IDialogAware
    {
        public string Title => "Historische Aufträge";
        public string? Material => _material;
        private string? _material;
        public string? MatDescription => _matDescription;
        private string? _matDescription;
        private List<OrderRb>? _orderList;
        public ICollectionView OrderList { get; private set; }
        
        public event Action<IDialogResult> RequestClose;

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

            _material = ord.First().Material;
            _matDescription = ord.First().MaterialNavigation?.Bezeichng;

            _orderList = new List<OrderRb>(ord.Where(x => x.Vorgangs.Any(x => x.Vnr == vnr)));
            OrderList = CollectionViewSource.GetDefaultView(_orderList);
        }
    }
}
