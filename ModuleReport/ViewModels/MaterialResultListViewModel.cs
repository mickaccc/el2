using El2Core.Models;
using El2Core.Utils;
using El2Core.ViewModelBase;
using Microsoft.EntityFrameworkCore;
using ModuleReport.ReportSources;
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
using static ModuleReport.ViewModels.ReportMainViewModel;

namespace ModuleReport.ViewModels
{
    internal class MaterialResultListViewModel : ViewModelBase
    {
        public MaterialResultListViewModel(IMaterialSource materialSource, IEventAggregator eventAggregator)
        { 
            Materials = CollectionViewSource.GetDefaultView(materialSource.Materials);
            ea = eventAggregator;
            ea.GetEvent<MessageReportFilterWorkAreaChanged>().Subscribe(OnFilterWorkAreaReceived);
            ea.GetEvent<MessageReportFilterDateChanged>().Subscribe(OnFilterDateReceived);
            Materials.GroupDescriptions.Add(new PropertyGroupDescription("TTNR"));
            Materials.Filter += OnFilterPredicate;
        }
        public ICollectionView Materials { get; }
        IEventAggregator ea;
        private HashSet<int> FilterRids = [];
        private DateTime FilterDate = DateTime.Now.Date;
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

    }
}
