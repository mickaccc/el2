using DocumentFormat.OpenXml.InkML;
using El2Core.Models;
using El2Core.Utils;
using El2Core.ViewModelBase;
using GongSolutions.Wpf.DragDrop;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Prism.Ioc;
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
        public string Title { get; } = "Schichtplan";
        private RelayCommand? _ShiftPlanSelectionChangedCommand;
        public RelayCommand ShiftPlanSelectionChangedCommand => _ShiftPlanSelectionChangedCommand ??= new RelayCommand(OnPlanSelected);
        public ICommand SaveAllCommand { get; private set; }
        public ICommand DeleteCommand { get; private set;}
        public ICommand SaveNewCommand { get; private set; }
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
        public ShiftPlan? SelectedPlan { get; set; }
        public List<ShiftPlan> ShiftPlans { get; set; }
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
                ShiftPlans = shift.ToList();
                ShiftWeeks = [];
                foreach (var item in shift)
                {
                    List<ShiftDay> shiftDays = new();
                    Byte[] bytes;
                    bytes = item.Sun;
                    shiftDays.Add(new ShiftDay(0, new BitArray(bytes)));
                    bytes = item.Mon;
                    shiftDays.Add(new ShiftDay(1, new BitArray(bytes)));
                    bytes = item.Tue;
                    shiftDays.Add(new ShiftDay(2, new BitArray(bytes)));
                    bytes = item.Wed;
                    shiftDays.Add(new ShiftDay(3, new BitArray(bytes)));
                    bytes = item.Thu;
                    shiftDays.Add(new ShiftDay(4, new BitArray(bytes)));
                    bytes = item.Fre;
                    shiftDays.Add(new ShiftDay(5, new BitArray(bytes)));
                    bytes = item.Sat;
                    shiftDays.Add(new ShiftDay(6, new BitArray(bytes)));

                    ShiftWeeks.Add(item.Id, shiftDays);
                }
                ShiftWeek = ShiftWeeks[ShiftPlans.First().Id];
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

        private void OnPlanSelected(object obj)
        {
            ShiftWeek = ShiftWeeks[SelectedPlan.Id];

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
            item.DefinitionChanged();
               
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

            bo.AsSpan().Slice(810, 990 - 810).Fill(true);
            bo.AsSpan().Slice(1000, 1150 - 1000).Fill(true);
            bo.AsSpan().Slice(1170, 1320 - 1170).Fill(true);

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
                CoverName = "Spätschicht",
                CoverMask = bytes
            };
            using var db = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
            db.ShiftCovers.Add(scc);
            db.SaveChanges();
        }
    }
}
