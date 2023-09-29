using Lieferliste_WPF.Data;
using Lieferliste_WPF.Data.Models;
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

        private TblAuftrag? _order;
        private ConcurrentObservableCollection<Vorgang> _vorgangs = new();
        public TblAuftrag? Order { get; private set; }
        public ICollectionView VorgangCV { get; private set; }

        #region Constructor
        public OrderViewModel() {} 
        #endregion
        public void LoadData(string AID)
        {
            _order = Dbctx.TblAuftrags
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
