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
            FilterDate = date;   
 
            YieldSum = 0;
            ReworkSum = 0;
            ScrapSum = 0;

            Materials.Refresh();
        }

        private void OnFilterWorkAreaReceived((int, bool) tuple)
        {

            if (tuple.Item2)
                FilterRids.Add(tuple.Item1);
            else
                FilterRids.Remove(tuple.Item1);

            YieldSum = 0;
            ReworkSum = 0;
            ScrapSum = 0;

            Materials.Refresh();
        }

        IContainerProvider container;
        IEventAggregator ea;
        private List<ReportMaterial> _Materials = [];
        public ICollectionView Materials { get; private set; }
        private HashSet<int> FilterRids = [];
        private DateTime? FilterDate;
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
                .Include(x => x.Responses)
                .Include(x => x.AidNavigation.DummyMatNavigation)
                .Include(x => x.AidNavigation.MaterialNavigation)
                .Where(x => x.Rid != null)               
                .ToList();

            foreach (var result in res)
            {                           
                    string? ttnr = string.Empty;
                    string? descript = string.Empty;
 
                    if(result.AidNavigation.Material != null)
                    {
                        ttnr = result.AidNavigation.Material.ToString();
                        descript = result.AidNavigation.MaterialNavigation?.Bezeichng;
                    }
                    else if(result.AidNavigation.DummyMat != null) 
                    {
                        ttnr = result.AidNavigation.DummyMat.ToString();
                        descript = result.AidNavigation.DummyMatNavigation?.Mattext;
                    }
                foreach (var item in result.Responses)
                {

                    if (ttnr != null)
                    {

                        var m = new ReportMaterial(ttnr,
                            descript,
                            result.Aid,
                            result.VorgangId,
                            result.Vnr,
                            result.Rid,
                            item.Yield,
                            item.Scrap,
                            item.Rework,
                            item.Timestamp);

         
 
                            _Materials.Add(m);
                        
                    }
                }
            }

            Materials = CollectionViewSource.GetDefaultView(_Materials);
            Materials.GroupDescriptions.Add(new PropertyGroupDescription("TTNR"));
            Materials.Filter += OnFilterPredicate;
        }

        private bool OnFilterPredicate(object obj)
        {
            bool ret = false;
            if (obj is ReportMaterial m)
            {
                ret = FilterRids.Any(x => x == m.Rid) && m.Date_Time.Date == FilterDate;
                if (ret)
                {
                    YieldSum += m.Yield;
                    ScrapSum += m.Scrap;
                    ReworkSum += m.Rework;
                }
            }
            return ret;
        }
        public record ReportMaterial(string TTNR, string? Description, string Order, string VID, int ProcessNr, int? Rid, int Yield, int Scrap, int Rework, DateTime Date_Time);

    }
}
