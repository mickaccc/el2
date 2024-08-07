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

        private void OnFilterDateReceived(List<DateTime> date)
        {
            YieldSum = 0;
            foreach (var item in _Materials)
            {
                foreach (var dt in date)
                {

                    item.DateRange = date;
                    YieldSum += item.GetYieldSum(dt);
                }
            }

            _ScrapSum = 0;
            _ReworkSum = 0;
   
            Materials.Refresh();
        }

        private void OnFilterWorkAreaReceived((int, bool) tuple)
        {
            foreach (var item in _Materials)
            {
                if (tuple.Item2)
                    item.FilterRids.Add(tuple.Item1);
                else
                    item.FilterRids.Remove(tuple.Item1);
            }
            Materials.Refresh();
        }

        IContainerProvider container;
        IEventAggregator ea;
        private List<ReportMaterial> _Materials = [];
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

            var res = db.Vorgangs.AsNoTracking()
                .Include(x => x.RidNavigation)
                .Include(x => x.Responses)
                .Include(x => x.AidNavigation.DummyMatNavigation)
                .Include(x => x.AidNavigation.MaterialNavigation)
                .Where(x => x.RidNavigation != null)               
                .ToList();

            foreach (var result in res.GroupBy(x => x.Aid))
            {              
                
                    string? ttnr = string.Empty;
                    string? descript = string.Empty;
 
                    if(result.FirstOrDefault()?.AidNavigation.Material != null)
                    {
                        ttnr = result.First().AidNavigation.Material.ToString();
                        descript = result.First().AidNavigation.MaterialNavigation?.Bezeichng;
                    }
                    else if(result.FirstOrDefault()?.AidNavigation.DummyMat != null) 
                    {
                        ttnr = result.First().AidNavigation.DummyMat.ToString();
                        descript = result.First().AidNavigation.DummyMatNavigation?.Mattext;
                    }
                    var rid = result.FirstOrDefault()?.Rid;
                    if (ttnr != null && rid != null)
                    {
                        if (_Materials.All(x => x.TTNR != ttnr))
                        {
                            var m = new ReportMaterial();

                            m.TTNR = ttnr;
                            m.Description = descript;
                            m.Vorgangs = [..result];
                            //m.DateRange.Add(DateTime.Today);
                            _Materials.Add(m);
                        }
                        else
                        {
                            var m = _Materials.Single(x => x.TTNR == ttnr);
                            m.FilterRids.Add((int)rid);
                        }
                    }
                
            }
            //foreach (var mats in _Materials)
            //{
            //    if (mats.Vorgangs == null) continue;
            //    foreach (var vrg in mats.Vorgangs.Where(x => x.Responses.Any()))
            //    {
            //        if (vrg.Responses.Any(y => y.Timestamp.Date == DateTime.Today))
            //        {
            //            YieldSum += vrg.Responses.Sum(x => x.Yield);
            //            ScrapSum += vrg.Responses.Sum(x => x.Scrap);
            //            ReworkSum += vrg.Responses.Sum(x => x.Rework);
            //        }
                    
            //    }
            //    YieldSum = mats.GetYieldSum(DateTime.Today);
            //}
            Materials = CollectionViewSource.GetDefaultView(_Materials);
            Materials.Filter += OnFilterPredicate;
        }

        private bool OnFilterPredicate(object obj)
        {
            if (obj is ReportMaterial m)
            {
                return m.IsVisible;
            }
            return false;
        }

        public class ReportMaterial
        {          
            public string TTNR { get; set; }
            public string? Description { get; set; }
            public DateTime Date_Time { get; set; }
            public List<DateTime> DateRange { get; set; } = [];
            public HashSet<int> FilterRids { get; set; } = [];
            public List<Vorgang> DisplayVorgangs { get; } = [];
            public List<Vorgang>? Vorgangs { get; set; }
            public bool IsVisible
            {
                get { return GetVisible(); }
            }
            public int GetYieldSum(DateTime date)
            {
                int r = 0;
                foreach (var vrg in DisplayVorgangs)
                {
                    r += vrg.Responses.Where(y => y.Timestamp.Date == date).Sum(x => x.Yield);
                }
                return r;
            }
            public int GetScrapSum(DateTime date)
            {
                int r = 0;
                foreach (var vrg in DisplayVorgangs)
                {
                    r += vrg.Responses.Where(y => y.Timestamp.Date == date).Sum(x => x.Scrap);
                }
                return r;
            }
            public int GetReworkSum(DateTime date)
            {
                int r = 0;
                foreach (var vrg in DisplayVorgangs)
                {
                    r += vrg.Responses.Where(y => y.Timestamp.Date == date).Sum(x => x.Rework);
                }
                return r;
            }
            private bool GetVisible()
            {
                if(FilterRids.Count == 0) return false;
                if(DateRange.Count == 0) return false;
                if(Vorgangs == null) return false;
                if(Vorgangs.Count == 0) return false;
                bool visible = false;
                DisplayVorgangs.Clear();
                foreach(var vrg in Vorgangs)
                {
                    if (FilterRids.Contains(vrg.Rid ?? 0)) { visible = true; break; }
                }
                if (visible)
                {
                    foreach(var date in DateRange)
                    {
                        foreach (var vrg in Vorgangs)
                        {
                            var responses = vrg.Responses.Where(x => x.Timestamp.Date == date.Date);
                            if (responses.Any())
                            {
                                Vorgang tempVorg = new();
                                tempVorg.Aid = vrg.Aid;
                                tempVorg.Vnr = vrg.Vnr;
                                tempVorg.VorgangId = vrg.VorgangId;
                                tempVorg.Responses = responses.ToList();
                                DisplayVorgangs.Add(tempVorg);
                            }
                        }                     
                    }
                }
                return DisplayVorgangs.Any();
            }
            public int YieldSum
            {
                get
                {
                    int r = 0;
                    foreach (var vrg in Vorgangs)
                    {
                        r += vrg.Responses.Sum(x => x.Yield);
                    }
                    return r;
                }
            }
            public int ScrapSum
            {
                    get
                {
                        int r = 0;
                        foreach (var vrg in Vorgangs)
                        {
                            r += vrg.Responses.Sum(x => x.Scrap);
                        }
                        return r;
                    }
                }
            public int ReworkSum
            {
                get
                {
                    int r = 0;
                    foreach (var vrg in Vorgangs)
                    {
                        r += vrg.Responses.Sum(x => x.Rework);
                    }
                    return r;
                }
            }

        }

        
    }
}
