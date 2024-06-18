using DocumentFormat.OpenXml.InkML;
using El2Core.Models;
using El2Core.Utils;
using El2Core.ViewModelBase;
using GongSolutions.Wpf.DragDrop;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Prism.Ioc;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Windows.Graphics.DirectX;
using Rule = El2Core.Models.Rule;


namespace Lieferliste_WPF.ViewModels
{
    internal class ShiftPlanEditViewModel : ViewModelBase, IDropTarget
    {
        private IContainerProvider _container;
        public string Title { get; } = "Schichtplan";
        private RelayCommand? _ShiftPlanSelectionChangedCommand;
        public RelayCommand ShiftPlanSelectionChangedCommand => _ShiftPlanSelectionChangedCommand ??= new RelayCommand(OnPlanSelected);
        public ICommand SaveAllCommand;
        public ICommand DeleteCommand;
        public ICommand SaveNewCommand;
        public Dictionary<int, List<ShiftDay>> ShiftWeeks { get; set; }
        private List<ShiftDay> _ShiftWeek;
        public List<ShiftDay> ShiftWeek
        {
            get { return _ShiftWeek; }
            set
            {
                if (_ShiftWeek != value)
                {
                    _ShiftWeek = value;
                    NotifyPropertyChanged(() => ShiftWeek);
                }
            }
        }
        public ShiftPlanDb? SelectedPlan { get; set; }
        public List<ShiftPlanDb> ShiftPlans { get; set; }
        public List<string> Shifts { get; set; }
        public ShiftPlanEditViewModel(IContainerProvider container)
        {
            _container = container;
            SaveAllCommand = new ActionCommand(OnSaveAllExecuted, OnSaveAllCanExecute);
            SaveNewCommand = new ActionCommand(OnSaveNewExecuted, OnSaveNewCanExecute);
            DeleteCommand = new ActionCommand(OnDeleteExecuted, OnDeleteCanExecuted);
            LoadData();
            LoadCovers();
        }

        public bool IsRubberChecked;
        private List<ShiftCover> _ShiftCovers = [];
        public ICollectionView ShiftCovers { get; private set; }
        private void LoadCovers()
        {
            using var db = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
            var sh = RuleInfo.Rules.Keys.Where(x => x.Contains(typeof(ShiftCover).Name)).ToList();

            var shc = new ShiftCover("earlyshift", "Frühschicht", "300,500,510,690,710,810");
            _ShiftCovers.Add(shc);
            shc = new ShiftCover("lateshift", "Spätschicht", "810,990,1000,1150,1170,1320");
            _ShiftCovers.Add(shc);
            ShiftCovers = CollectionViewSource.GetDefaultView(_ShiftCovers);
        }
        private void SaveShiftCover(ShiftCover shiftCover)
        {
            Rule r;
            if (RuleInfo.Rules.Keys.Contains(shiftCover.CoverId)) { r = RuleInfo.Rules[shiftCover.CoverId]; }
            else { r = new Rule() { RuleName = shiftCover.CoverName, RuleValue = shiftCover.CoverId }; }
            //Globals.SaveRule(r);
        }
        private void LoadData()
        {
            using var db = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
            var shift = db.ShiftPlanDbs.AsNoTracking().Where(x => x.ShiftName != "keine").OrderBy(x => x.Planid);
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
            ShiftWeek = ShiftWeeks[ShiftPlans.First().Planid];

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

        private void OnPlanSelected(object obj)
        {
            ShiftWeek = ShiftWeeks[SelectedPlan.Planid];

        }
        private bool OnDeleteCanExecuted(object arg)
        {
            return true;
        }

        private void OnDeleteExecuted(object obj)
        {
            
        }

        private bool OnSaveNewCanExecute(object arg)
        {
            return true;
        }

        private void OnSaveNewExecuted(object obj)
        {
            
        }

        private bool OnSaveAllCanExecute(object arg)
        {
            return true;
        }

        private void OnSaveAllExecuted(object obj)
        {
            
        }
        public void DragOver(IDropInfo dropInfo)
        {
            
        }

        public void Drop(IDropInfo dropInfo)
        {
            
        }
        public struct ShiftCover
        {
            public ShiftCover(string coverId, string coverName, string coverDef)
            {
                CoverId = coverId;
                CoverName = coverName;
                CoverDef = coverDef;
            }
            public string CoverId;
            public string CoverName { get; }
            public string CoverDef { get; }
        }
        public class ShiftDay
        {
            public ShiftDay(int id, string definition)
            {
                Id = id;
                Definition = definition;
                WeekDayName = DateTimeFormatInfo.CurrentInfo.GetDayName((DayOfWeek)id);
            }
            public readonly int Id;
            public string WeekDayName { get; }
            public string Definition { get; }
        }
    }
}
