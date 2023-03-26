using Lieferliste_WPF.Data;
using Lieferliste_WPF.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lieferliste_WPF.ViewModels
{
    class OrderViewModel :Base.ViewModelBase
    {

        private IEnumerable<TblAuftrag>? _order;
        public IEnumerable<TblAuftrag>? Order { get; private set; }
        private readonly DataContext _db = new();

        #region Constructor
        public OrderViewModel() { } 
        #endregion
        public void LoadData(string AID)
        {
            _order = _db.TblAuftrags
                .Include(m => m.MaterialNavigation)
                .Include(d => d.DummyMatNavigation)
                .Include(v => v.TblVorgangs)
                .Where(o => o.Aid  == AID)
                .ToList();
            Order = _order;
        }
    }
}
