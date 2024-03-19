using Lieferliste_WPF.Utilities;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Lieferliste_WPF.Utilities;

namespace Lieferliste_WPF.ViewModels
{
    internal class HolidayEditViewModel
    {
        public HolidayEditViewModel(IContainerExtension container) 
        {
            _container = container;
            LoadData();
        }
        public ObservableCollection<CloseAndHolidayRule.HolidayRule> FixHolidays { get; set; } = [];
        public ObservableCollection<CloseAndHolidayRule.HolidayRule> VarHolidays { get; set; } = [];
        public ObservableCollection<Holiday> CloseHolidays { get; set; } = [];
        IContainerExtension _container;
        void LoadData()
        {
            FixHolidays.AddRange(CloseAndHolidayRule.FixHoliday);
            VarHolidays.AddRange(CloseAndHolidayRule.VariousHoliday);
            CloseHolidays.AddRange(CloseAndHolidayRule.CloseDay);
        }
    }
}
