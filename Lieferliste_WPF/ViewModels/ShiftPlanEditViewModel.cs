using DocumentFormat.OpenXml.InkML;
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
        public Dictionary<int, List<ShiftDay>> ShiftWeeks { get; set; }
        public List<ShiftPlanDb> ShiftPlans { get; set; }
        public List<string> Shifts { get; set; }
        public ShiftPlanEditViewModel(IContainerProvider container)
        {
            _container = container;
            LoadData();
        }

        private void LoadData()
        {
            using var db = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
            var shift = db.ShiftPlanDbs.AsNoTracking().Where(x => x.ShiftName != "keine");
            ShiftPlans = shift.ToList();
            ShiftWeeks = [];
            foreach (var item in shift)
            {
                List<ShiftDay> shiftDays = new();
                for (int i = 0; i < 7; i++)
                {
                    shiftDays.Add(new ShiftDay(i, GetDefinition(item, i)));
                }
                ShiftWeeks.Add(item.Planid, shiftDays);
            }
        }
        private string GetDefinition(ShiftPlanDb shiftPlanDb, int index)
        {
            switch (index)
            {
                case 0: return shiftPlanDb.Su;
                case 1: return shiftPlanDb.Mo;
                case 2: return shiftPlanDb.Tu;
                case 3: return shiftPlanDb.We;
                case 4: return shiftPlanDb.Th;
                case 5: return shiftPlanDb.Fr;
                case 6: return shiftPlanDb.Sa;
            }
            return "0,0";
        }
    }
    public class ShiftDay
    {
        public ShiftDay(int id, string definition)
        {
            Id = id;
            Definition = definition;
            ShiftName = DateTimeFormatInfo.CurrentInfo.GetDayName((DayOfWeek)id);
        }
        public readonly int Id;
        public readonly string ShiftName;
        public readonly string Definition;
    }
    
}
