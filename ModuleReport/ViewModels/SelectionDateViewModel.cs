using El2Core.Utils;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuleReport.ViewModels
{
    class SelectionDateViewModel
    {
        public SelectionDateViewModel(IEventAggregator eventAggregator)
        {
            ea = eventAggregator;
            
        }
        IEventAggregator ea;
        RelayCommand? _DateChangedCommand;
        public DateTime SelectedDate { get; set; }
        public RelayCommand DateChangedCommand => _DateChangedCommand ??= new RelayCommand(OnDateChanged);

        private void OnDateChanged(object obj)
        {
            ea.GetEvent<MessageReportFilterDateChanged>().Publish(SelectedDate);
            
        }
    }
}
