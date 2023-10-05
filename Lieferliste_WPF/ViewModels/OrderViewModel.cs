
using El2Utilities.Models;
using Lieferliste_WPF.Utilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Lieferliste_WPF.ViewModels
{
    class OrderViewModel :Base.ViewModelBase
    {

        private OrderRb? _order;
        private ConcurrentObservableCollection<Vorgang> _vorgangs = new();
        public OrderRb? Order { get; private set; }
        public ICollectionView VorgangCV { get; private set; }

        #region Constructor
        public OrderViewModel() {} 
        #endregion
        public void LoadData(string AID)
        {
            _order = Dbctx.OrderRbs
                .Include(m => m.MaterialNavigation)
                .Include(d => d.DummyMatNavigation)
                .Include(v => v.Vorgangs)
                .Single(o => o.Aid  == AID);

            _vorgangs.AddRange(_order.Vorgangs);
            VorgangCV = CollectionViewSource.GetDefaultView(_vorgangs);
            VorgangCV.SortDescriptions.Add(new SortDescription("Vnr",ListSortDirection.Ascending));
            
            Order = _order;
        }
    }
}
