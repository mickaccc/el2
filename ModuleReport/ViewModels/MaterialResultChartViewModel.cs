using El2Core.Utils;
using El2Core.ViewModelBase;
using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.VisualBasic.Devices;
using ModuleReport.ReportSources;
using Prism.Events;
using System.Windows.Forms;

namespace ModuleReport.ViewModels
{
    internal class MaterialResultChartViewModel : ViewModelBase
    {
        public MaterialResultChartViewModel(IMaterialSource materialSource, IEventAggregator eventAggregator)
        {
            Materials = materialSource.Materials;
            ea = eventAggregator;
            ea.GetEvent<MessageReportFilterDateChanged>().Subscribe(OnDateChanged);
            ea.GetEvent<MessageReportFilterWorkAreaChanged>().Subscribe(OnWorkAreaChanged);
            SeriesCollection = new SeriesCollection
            {
                new ColumnSeries { Title = "Gut", Values = new ChartValues<int>() },
                new ColumnSeries { Title = "Ausschuss", Values = new ChartValues<int>() },
                new ColumnSeries { Title = "Nacharbeit", Values = new ChartValues<int>() }
            };
            Formatter = value => value.ToString("N");
        }

        IEventAggregator ea;
        private List<ReportMaterial> Materials;
        public SeriesCollection SeriesCollection { get; set; }
        private string[] labels = [];
        public string[] Labels
        {
            get { return labels; }
            set 
            {
                if (labels != value)
                {
                    labels = value;
                    NotifyPropertyChanged(() => Labels);
                }
            }
        }
        private HashSet<int> FilterRids = [];
        private DateTime Date = DateTime.Now.Date;
        public Func<int, string> Formatter { get; set; }

        private void OnWorkAreaChanged((int, bool) tuple)
        {
            if(tuple.Item2)
                FilterRids.Add(tuple.Item1);
            else FilterRids.Remove(tuple.Item1);

            RefreshData();
        }

        private void OnDateChanged(DateTime time)
        {
            Date = time.Date;
            RefreshData();
        }
        private void RefreshData()
        {
            SeriesCollection[0].Values.Clear();
            SeriesCollection[1].Values.Clear();
            SeriesCollection[2].Values.Clear();
            HashSet<string> keys = [];
            int yield = 0, scrap = 0, rework = 0;
            foreach (var item in Materials.Where(x => x.Date_Time.Date == Date).GroupBy(x => x.Rid))
            {
                if (FilterRids.Any(x => x == item.Key))
                {

                    foreach (var mat in item)
                    {
                        yield += mat.Yield;
                        scrap += mat.Scrap;
                        rework += mat.Rework;
                        keys.Add(mat.MachName);
                    }
                    SeriesCollection[0].Values.Add(yield);
                    SeriesCollection[1].Values.Add(scrap);
                    SeriesCollection[2].Values.Add(rework);
                    yield = 0; scrap = 0; rework = 0;
                }
            }

            Labels = keys.ToArray();
            
        }
    }
}
