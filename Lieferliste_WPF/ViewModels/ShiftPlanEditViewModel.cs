using DocumentFormat.OpenXml.InkML;
using El2Core.Models;
using El2Core.Utils;
using El2Core.ViewModelBase;
using GongSolutions.Wpf.DragDrop;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Prism.Ioc;
using Prism.Services.Dialogs;
using System;
using System.Collections;
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
using WpfCustomControlLibrary;
using Rule = El2Core.Models.Rule;


namespace Lieferliste_WPF.ViewModels
{
    internal class ShiftPlanEditViewModel : ViewModelBase, IDropTarget
    {
        private IContainerProvider _container;
        private IDialogService _dialogService;
        public string Title { get; } = "Schichtplan";
        private RelayCommand? _ShiftPlanSelectionChangedCommand;
        public RelayCommand ShiftPlanSelectionChangedCommand => _ShiftPlanSelectionChangedCommand ??= new RelayCommand(OnPlanSelected);
        public ICommand SaveAllCommand { get; private set; }
        public ICommand DeleteCoverCommand { get; private set;}
        public ICommand SaveNewCommand { get; private set; }
        public ICommand DetailCoverCommand { get; private set; }
        public ICommand AddCoverCommand { get; private set; }
        public Dictionary<int, List<ShiftDay>> ShiftWeeks { get; set; }
        private ShiftWeek? _SelectedPlan;
        public ShiftWeek? SelectedPlan
        {
            get { return _SelectedPlan; }
            set
            {
                if (_SelectedPlan != value)
                {
                    _SelectedPlan = value;
                    NotifyPropertyChanged(() => SelectedPlan);
                }
            }
        }
        public List<ShiftWeek> ShiftWeekPlans { get; set; }
        public List<string> Shifts { get; set; }
        public ShiftPlanEditViewModel(IContainerProvider container, IDialogService dialogService)
        {
            _container = container;
            _dialogService = dialogService;
            SaveAllCommand = new ActionCommand(OnSaveAllExecuted, OnSaveAllCanExecute);
            SaveNewCommand = new ActionCommand(OnSaveNewExecuted, OnSaveNewCanExecute);
            DeleteCoverCommand = new ActionCommand(OnDeleteExecuted, OnDeleteCanExecuted);
            AddCoverCommand = new ActionCommand(OnAddExecuted, OnAddCanExecuted);
            DetailCoverCommand = new ActionCommand(OnDetailExecuted, OnDetailCanExecuted);
            LoadData();
            LoadCovers();
            
        }

        public bool IsRubberChecked { get; set; }
        private List<ShiftCover> _ShiftCovers = [];
        public ICollectionView ShiftCovers { get; private set; }
        private void LoadCovers()
        {
            using var db = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
            var shc = db.ShiftCovers;
 
            _ShiftCovers.AddRange(shc);
            ShiftCovers = CollectionViewSource.GetDefaultView(_ShiftCovers);

        }
        private void SaveShiftCover(ShiftCover shiftCover)
        {
            using var db = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
            var shc = db.ShiftCovers;

            if(shc.Contains(shiftCover))
            {

            }
            else
            {
                shc.Add(shiftCover);
            }
            db.SaveChanges();
        }
        private void LoadData()
        {
            using var db = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
            var shift = db.ShiftPlans.AsNoTracking().OrderBy(x => x.Id);
            if (shift.Any())
            {

                ShiftWeeks = [];
                ShiftWeekPlans = [];
                foreach (var item in shift)
                {
                    var week = new ShiftWeek();
                    week.Id = item.Id;
                    week.ShiftPlanName = item.PlanName;
                    week.Lock = item.Lock;
                    List<ShiftDay> shiftDays = new();
                    Byte[] bytes;
                    bytes = item.Sun;
                    week.ShiftWeekDays.Add(new ShiftDay(0, new BitArray(bytes)));
                    bytes = item.Mon;
                    week.ShiftWeekDays.Add(new ShiftDay(1, new BitArray(bytes)));
                    bytes = item.Tue;
                    week.ShiftWeekDays.Add(new ShiftDay(2, new BitArray(bytes)));
                    bytes = item.Wed;
                    week.ShiftWeekDays.Add(new ShiftDay(3, new BitArray(bytes)));
                    bytes = item.Thu;
                    week.ShiftWeekDays.Add(new ShiftDay(4, new BitArray(bytes)));
                    bytes = item.Fre;
                    week.ShiftWeekDays.Add(new ShiftDay(5, new BitArray(bytes)));
                    bytes = item.Sat;
                    week.ShiftWeekDays.Add(new ShiftDay(6, new BitArray(bytes)));

                    ShiftWeekPlans.Add(week);
                }
                SelectedPlan = ShiftWeekPlans.FirstOrDefault();
            }
        }
        private Byte[]? GetDefinition(ShiftPlan shiftPlanDb, int index)
        {
            switch (index)
            {
                case 0: return shiftPlanDb.Sun;
                case 1: return shiftPlanDb.Mon;
                case 2: return shiftPlanDb.Tue;
                case 3: return shiftPlanDb.Wed;
                case 4: return shiftPlanDb.Thu;
                case 5: return shiftPlanDb.Fre;
                case 6: return shiftPlanDb.Sat;
            }
            return null;
        }
        private bool OnDetailCanExecuted(object arg)
        {
            return true;
        }

        private void OnDetailExecuted(object obj)
        {
            if (obj is ShiftCover cover)
            {
                var par = new DialogParameters();
                par.Add("Cover", cover);

                _dialogService.Show("DetailCoverDialog", par, OnDetailCallBack);
            }
        }

        private void OnDetailCallBack(IDialogResult result)
        {
            if (result.Result == ButtonResult.OK)
            {
                var c = result.Parameters.GetValue<ShiftCover>("Cover");
                using var db = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
                if (string.IsNullOrEmpty(c.CoverName) == false &&
                        c.CoverMask != null)
                {
                    
                    if (c.Id == 0 )
                    {
                        db.ShiftCovers.Add(c);
                        _ShiftCovers.Add(c);
                        ShiftCovers.Refresh();
                    }
                    else
                    {
                        db.ShiftCovers.Update(c);
                        var cc = _ShiftCovers.Find(x => x.Id == c.Id);
                        if (cc != null)
                        {
                            cc.CoverMask = c.CoverMask;
                            cc.CoverName = c.CoverName;
                        }
                    }
                    db.SaveChanges();
                }
            }          
        }

        private bool OnAddCanExecuted(object arg)
        {
            return true;
        }

        private void OnAddExecuted(object obj)
        {
            _dialogService.Show("DetailCoverDialog", OnDetailCallBack);
        }
        private void OnPlanSelected(object obj)
        {
            

        }
        private bool OnDeleteCanExecuted(object arg)
        {
            if (arg is ShiftCover cover)
            {
                return !cover.Lock;
            }
            if(arg is ShiftPlan plan)
            {
                return !plan.Lock;
            }
            return false;
        }

        private void OnDeleteExecuted(object obj)
        {
            using var db = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
            if (obj is ShiftCover cover)
            {
                _ShiftCovers.Remove(cover);
                
                db.Remove(cover);
                db.SaveChanges();
                ShiftCovers.Refresh();
            }
            if(obj is ShiftWeek plan)
            {
                ShiftWeekPlans.Remove(plan);
                db.Remove(plan);
                db.SaveChanges();
            }
        }

        private bool OnSaveNewCanExecute(object arg)
        {
            return true;
        }

        private void OnSaveNewExecuted(object obj)
        {
            var name = 
        }

        private bool OnSaveAllCanExecute(object arg)
        {
            if(SelectedPlan != null)
                return !SelectedPlan.Lock;
            return false;
        }

        private void OnSaveAllExecuted(object obj)
        {
            if (obj is ShiftWeek shiftWeek)
            {
                using var db = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
                var planDb = db.ShiftPlans.SingleOrDefault(x => x.Id == shiftWeek.Id);
                if (planDb != null)
                {
                    planDb.Sun = shiftWeek.GetDayDefinition(0);
                    planDb.Mon = shiftWeek.GetDayDefinition(1);
                    planDb.Tue = shiftWeek.GetDayDefinition(2);
                    planDb.Wed = shiftWeek.GetDayDefinition(3);
                    planDb.Thu = shiftWeek.GetDayDefinition(4);
                    planDb.Fre = shiftWeek.GetDayDefinition(5);
                    planDb.Sat = shiftWeek.GetDayDefinition(6);
                    
                    db.SaveChanges();
                }
                else
                {
                    var plan = shiftWeek.GetNewShiftPlan();
                    db.ShiftPlans.Add(plan);
                    db.SaveChanges();
                }
            }
            
        }
        public void DragOver(IDropInfo dropInfo)
        {
            dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
            dropInfo.Effects = DragDropEffects.All;
        }

        public void Drop(IDropInfo dropInfo)
        {
            ShiftCover data = (ShiftCover)dropInfo.Data;
            ShiftDay? item = dropInfo.TargetItem as ShiftDay;

            BitArray cover = new BitArray(data.CoverMask);
            if (IsRubberChecked)
            {
                for(int i = 0;i < cover.Length;i++)
                {
                    if (cover[i]) item.Definition[i] = false;
                }
            }
            else { item.Definition.Or(cover); }
            var s = SelectedPlan;
            SelectedPlan = null;
            SelectedPlan = s;
            
        }
        public class ShiftDay(int id, BitArray definition) : ViewModelBase
        {
            public readonly int Id = id;
            public string WeekDayName { get; } = DateTimeFormatInfo.CurrentInfo.GetDayName((DayOfWeek)id);
            private BitArray _Definition = definition;
            public BitArray Definition
            {
                get { return _Definition; }
                set
                {
                    _Definition = value;
                    NotifyPropertyChanged(() => Definition);
                }
            }
            public void DefinitionChanged()
            {
                NotifyPropertyChanged(() => Definition);
            }
        }
        public class ShiftWeek
        {
            public int Id { get; set; }
            public string ShiftPlanName { get; set; }
            public bool Lock { get; set; } = false;
            public List<ShiftDay> ShiftWeekDays { get; set; } = [];

            public ShiftPlan GetNewShiftPlan()
            {
                ShiftPlan shiftPlan = new ShiftPlan();

                shiftPlan.PlanName = ShiftPlanName;
                //shiftPlan.Sun = ShiftWeekDays[0].Definition.CopyTo;

                return shiftPlan;
            }
            public Byte[] GetDayDefinition(int id)
            {
                byte[] bytes = new byte[1440];
                ShiftWeekDays.First(x => x.Id == id).Definition.CopyTo(bytes, 0);

                return bytes;
            }
        }
        void addshift()
        {
            bool[] bo = new bool[1440];
            bool[] nul = new bool[1440];
            bool[] sun = new bool[1440];
            BitArray nulBit = new BitArray(nul);
            byte[] nulByte = new byte[nulBit.Length];

            sun.AsSpan().Slice(1260, 1440 - 1260).Fill(true);
            BitArray sunBit = new BitArray(sun);
            byte[] sunByte = new byte[sunBit.Length];
            sunBit.CopyTo(sunByte, 0);

            //bo.AsSpan().Slice(0, 120).Fill(true);
            //bo.AsSpan().Slice(130, 300 - 130).Fill(true);
            //bo.AsSpan().Slice(1320, 1440 - 1320).Fill(true);

            //bo.AsSpan().Slice(810, 990 - 810).Fill(true);
            //bo.AsSpan().Slice(1000, 1150 - 1000).Fill(true);
            //bo.AsSpan().Slice(1170, 1320 - 1170).Fill(true);

            bo.AsSpan().Slice(120, 15).Fill(true);

            //bo.AsSpan().Slice(300, 510 - 300).Fill(true);
            //bo.AsSpan().Slice(520, 690 - 520).Fill(true);
            //bo.AsSpan().Slice(710, 810 - 710).Fill(true);
            BitArray arr = new BitArray(bo);
            byte[] bytes = new byte[arr.Length];

            arr.CopyTo(bytes, 0);
            var sc = new ShiftPlan()
            {
                PlanName = "3 Schicht",
                Sun = nulByte,
                Mon = bytes,
                Tue = bytes,
                Wed = bytes,
                Thu = bytes,
                Fre = bytes,
                Sat = nulByte,
            };
            var scc = new ShiftCover()
            {
                CoverName = "PausNacht",
                CoverMask = bytes
            };
            using var db = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
            db.ShiftCovers.Add(scc);
            db.SaveChanges();
        }
    }
}
