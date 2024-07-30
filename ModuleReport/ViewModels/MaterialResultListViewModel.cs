using El2Core.Models;
using El2Core.Utils;
using El2Core.ViewModelBase;
using Microsoft.EntityFrameworkCore;
using Prism.Events;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using static ModuleReport.ViewModels.MaterialResultListViewModel;

namespace ModuleReport.ViewModels
{
    internal class MaterialResultListViewModel : ViewModelBase
    {
        public MaterialResultListViewModel(IContainerProvider containerProvider, IEventAggregator eventAggregator)
        { 
            container = containerProvider;
            ea = eventAggregator;
            LoadData();
            ea.GetEvent<MessageReportFilterWorkAreaChanged>().Subscribe(OnFilterWorkAreaReceived);
            ea.GetEvent<MessageReportFilterDateChanged>().Subscribe(OnFilterDateReceived);
        }

        private void OnFilterDateReceived(DateTime date)
        {
            _YieldSum = 0;
            _ScrapSum = 0;
            _ReworkSum = 0;
            foreach (var item in _Materials)
            {
                if(item.IsActive = item.Responses.Any(x => x.Timestamp.Date.Equals(date.Date)))
                {
                    YieldSum += item.Responses.Sum(x => x.Yield);
                    ScrapSum += item.Responses.Sum(x => x.Scrap);
                    ReworkSum += item.Responses.Sum(x => x.Rework);
                }
            }
            Materials.Refresh();
        }

        private void OnFilterWorkAreaReceived((string, bool) tuple)
        {
            var m = _Materials.Where(x => x.InventNos.Any(y => y.Equals(tuple.Item1)));
            foreach (var item in m)
            {
                item.IsActive = tuple.Item2;
            }
            Materials.Refresh();
        }

        IContainerProvider container;
        IEventAggregator ea;
        private List<Mat> _Materials = [];
        public ICollectionView Materials { get; private set; }
        private int _YieldSum = 0;
        private int _ScrapSum = 0;
        private int _ReworkSum = 0;
        public int YieldSum
        {
            get { return _YieldSum; }
            set
            {
                _YieldSum = value;
                NotifyPropertyChanged(() => YieldSum);
            }
        }
        public int ReworkSum
        {
            get { return _ReworkSum; }
            set
            {
                _ReworkSum = value;
                NotifyPropertyChanged(() => ReworkSum);
            }
        }
        public int ScrapSum
        {
            get { return _ScrapSum; }
            set
            {
                _ScrapSum = value;
                NotifyPropertyChanged(() => ScrapSum);
            }
        }
        private void LoadData()
        {
            using var db = container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();

            var results = db.Vorgangs
                .Include(x => x.RidNavigation)
                .ThenInclude(x => x.WorkArea)
                .Include(x => x.Responses)
                .Include(x => x.AidNavigation.DummyMatNavigation)
                .Include(x => x.AidNavigation.MaterialNavigation)
                .Where(x => x.AidNavigation.Abgeschlossen == false)               
                .ToList();
            foreach (var result in results.Where(x => x.Rid != null))
            {
                if (UserInfo.User.AccountWorkAreas.Any(x => x.WorkAreaId == result.RidNavigation.WorkAreaId))
                {
                    var m = new Mat() { TTNR = result.AidNavigation.MaterialNavigation.Ttnr, Description = result.AidNavigation.MaterialNavigation.Bezeichng };
   
                    m.Responses = result.Responses.ToList();
                    m.InventNos.Add(result.RidNavigation.Inventarnummer);
                    m.IsActive = true;
                    _Materials.Add(m);
                }
            }
            foreach (var mats in _Materials.Where(x => x.Responses != null))
            {
                if (mats.Responses.Any(y => y.Timestamp.Date == DateTime.Today))
                {
                    YieldSum += mats.Responses.Sum(x => x.Yield);
                    ScrapSum += mats.Responses.Sum(x => x.Scrap);
                    ReworkSum += mats.Responses.Sum(x => x.Rework);
                }
            }
            Materials = CollectionViewSource.GetDefaultView(_Materials);
            Materials.Filter += OnFilterPredicate;
        }

        private bool OnFilterPredicate(object obj)
        {
            if (obj is Mat m)
            {
                return m.IsActive;
            }
            return false;
        }

        public class Mat
        {          
            public string TTNR { get; set; }
            public string? Description { get; set; }
            public List<Response> Responses { get; set; }
            public HashSet<string> InventNos { get; } = [];
            public bool IsActive { get; set; }
            public int YieldSum { get { return (Responses == null) ? 0 : Responses.Sum(x => x.Yield); } }
            public int ScrapSum { get {  return (Responses == null) ? 0 : Responses.Sum(x => x.Scrap); } }
            public int ReworkSum { get { return (Responses == null) ? 0 : Responses.Sum(x => x.Rework); } }

        }

        
    }
}
