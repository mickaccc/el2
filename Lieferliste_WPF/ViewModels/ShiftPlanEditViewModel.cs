using El2Core.Models;
using El2Core.ViewModelBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Prism.Ioc;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Windows.Graphics.DirectX;


namespace Lieferliste_WPF.ViewModels
{
    internal class ShiftPlanEditViewModel : ViewModelBase
    {
        private IContainerProvider _container;
        public string Title { get; } = "Schichtplan";
        public List<Tuple<string, string>> ShiftWeek { get; set; }
        public List<ShiftPlanDb> ShiftPlans { get; set; }
        public List<string> Shifts { get; set; }
        public bool[,] ShiftPlan { get; }
        public ShiftPlanEditViewModel(IContainerProvider container)
        {
            _container = container;
            LoadData();
                  
        }

        private void LoadData()
        {
            using var db = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
            var shift = db.ShiftPlanDbs.AsNoTracking();
            ShiftPlans = shift.ToList();
            ShiftWeek = [];
            var item = shift.First();
            {
                string dt;
                dt = DateTimeFormatInfo.CurrentInfo.GetDayName(0);
                ShiftWeek.Add(Tuple.Create(dt, item.Su));
                dt = DateTimeFormatInfo.CurrentInfo.GetDayName((DayOfWeek)1);
                ShiftWeek.Add(Tuple.Create(dt, item.Mo));
                dt = DateTimeFormatInfo.CurrentInfo.GetDayName((DayOfWeek)2);
                ShiftWeek.Add(Tuple.Create(dt, item.Tu));
                dt = DateTimeFormatInfo.CurrentInfo.GetDayName((DayOfWeek)3);
                ShiftWeek.Add(Tuple.Create(dt, item.We));
                dt = DateTimeFormatInfo.CurrentInfo.GetDayName((DayOfWeek)4);
                ShiftWeek.Add(Tuple.Create(dt, item.Th));
                dt = DateTimeFormatInfo.CurrentInfo.GetDayName((DayOfWeek)5);
                ShiftWeek.Add(Tuple.Create(dt, item.Fr));
                dt = DateTimeFormatInfo.CurrentInfo.GetDayName((DayOfWeek)6);
                ShiftWeek.Add(Tuple.Create(dt, item.Sa));
            }
        }

    }
}
