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
        private List<Vorgang>? _orderList;
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
            var list = new List<Vorgang>();
            foreach (var o in ord)
            {
  
                Vorgang vrg;
                var ind = o.Vorgangs.OrderBy(x => x.Vnr).Select((x, i) => new { vrg = x, Index = i }).First(x => x.vrg.Vnr >= vnr)?.Index;
                if (ind != null)
                {
                    list.Add(o.Vorgangs.ElementAt(ind.Value - 1));
                    list.Add(o.Vorgangs.ElementAt(ind.Value));
                    list.Add(o.Vorgangs.ElementAt(ind.Value + 1));
                }
            }
                _orderList = new List<Vorgang>(list);
            OrderList = CollectionViewSource.GetDefaultView(_orderList);
            OrderList.GroupDescriptions.Add(new PropertyGroupDescription("Aid"));
        }
    }
}
