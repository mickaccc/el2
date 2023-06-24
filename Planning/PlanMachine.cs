using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;
using System.Data;
using Lieferliste_WPF.Data.Models;
using System.ComponentModel;

using Windows.UI.Composition;
using System.Windows.Media;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Lieferliste_WPF.Commands;
using System.Windows.Controls;
using WpfCustomControlLibrary;
using System.Collections.Immutable;
using Lieferliste_WPF.ViewModels;
using GongSolutions.Wpf.DragDrop;
using System.Collections;
using System.Windows.Interop;
using System.Diagnostics;
using Lieferliste_WPF.Data;

namespace Lieferliste_WPF.Planning
{
    internal class PlanMachine : DependencyObject, IDropTarget
    {


        #region Constructors
        public PlanMachine() { }
        public PlanMachine(int RID, string name, string inventarNo, MachinePlanViewModel owner)
        {
            Initialize();
            _rId = RID;
            Name = name;
            InventNo = inventarNo;
            Owner = owner;
        }


        public PlanMachine(int RID, string name, string inventarNo, WorkArea workArea, int[] costUnit)
        {
            Initialize();
            _rId = RID;
            Name = name;
            InventNo = inventarNo;
            WorkArea = workArea;
            CostUnits = costUnit;
        }
        #endregion


        public static readonly DependencyProperty PlanableProperty =
            DependencyProperty.Register("Planable"
                , typeof(bool)
                , typeof(PlanMachine));
        public static readonly DependencyProperty BulletsProperty =
            DependencyProperty.Register("Bullets"
                , typeof(ImmutableArray<Shape>)
                , typeof(PlanMachine));

        public bool Planable
        {
            get { return (bool)GetValue(PlanableProperty); }
            set { SetValue(PlanableProperty, value); }
        }
        public ImmutableArray<Shape> Bullets
        {
            get { return (ImmutableArray<Shape>)GetValue(BulletsProperty); }
            set { SetValue(BulletsProperty, value); }
        }
        public ICommand SetMarkerCommand { get; private set; }
        public ICommand ChangeProcessesCommand { get; private set; }
        private int _rId;

        public int RID { get { return _rId; } }
        public String Name { get; set; }
        public String Description { get; set; }
        public String InventNo { get; private set; }
        public WorkArea WorkArea { get; set; }
        public int[] CostUnits { get; set; }
        public string[] ArbPlSAPs { get; set; }
        protected MachinePlanViewModel Owner { get; private set; }

        public ObservableCollection<Vorgang> Processes { get; set; }
        
        private ICollectionView _processesCV;

        private void Initialize()
        {
            SetMarkerCommand = new ActionCommand(OnSetMarkerExecuted, OnSetMarkerCanExecute);

            
            Processes = new ObservableCollection<Vorgang>();
            _processesCV = CollectionViewSource.GetDefaultView(Processes);
            _processesCV.SortDescriptions.Add(new SortDescription("Spos", ListSortDirection.Ascending));
            
        }

        private bool OnSetMarkerCanExecute(object arg)
        {
            if(arg != null)
            {             
                var values = (object[])arg;
                if (values[1] is Vorgang)
                {
                    return true;
                } 
                else return false;
            }
            return false;
        }

        private void OnSetMarkerExecuted(object obj)
        {
            if (obj == null) return;
            var values = (object[])obj;
            var name = (string)values[0];
            var desc = (Vorgang)values[1];
            
            if (name == "DelBullet") desc.Bullet = Brushes.White.ToString();
            if (name == "Bullet1") desc.Bullet = Brushes.Red.ToString();
            if (name == "Bullet2") desc.Bullet = Brushes.Green.ToString();
            if (name == "Bullet3") desc.Bullet = Brushes.Yellow.ToString();
            if (name == "Bullet4") desc.Bullet = Brushes.Blue.ToString();

            _processesCV.Refresh();
  
        }
        public void Exit()
        {

            Owner.Exit();
        }


        public void Drop(IDropInfo dropInfo)
        {
            Vorgang vrg = (Vorgang)dropInfo.Data;
            
            ((IList)dropInfo.DragInfo.SourceCollection).Remove(vrg);
            int v = dropInfo.InsertIndex;
            vrg.Rid = _rId;
            if (v > ((IList)dropInfo.TargetCollection).Count)
            {
                ((IList)dropInfo.TargetCollection).Add(vrg);
            }
            else
            { 
                ((IList)dropInfo.TargetCollection).Insert(v, vrg);
            }
            for(int i = 0; i < Processes.Count; i++)
            {
                Processes[i].Spos = i;
            }

        }

        public void DragOver(IDropInfo dropInfo)
        {
            if (dropInfo.Data is Vorgang)
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                dropInfo.Effects = DragDropEffects.Move;
            }
        }
    }
}
