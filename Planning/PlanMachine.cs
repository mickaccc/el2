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
        public WorkArea? WorkArea { get; set; }
        public int[] CostUnits { get; set; }
        public string[] ArbPlSAPs { get; set; }
        protected MachinePlanViewModel Owner { get; private set; }

        public ObservableCollection<Vorgang> Processes { get; set; }
        
        public ICollectionView ProcessesCV { get { return ProcessesCVSource.View; } }
        internal CollectionViewSource ProcessesCVSource { get; set; } = new CollectionViewSource();

        private void Initialize()
        {
            SetMarkerCommand = new ActionCommand(OnSetMarkerExecuted, OnSetMarkerCanExecute);


            Processes = new ObservableCollection<Vorgang>();
            ProcessesCVSource.Source = Processes;
            ProcessesCV.SortDescriptions.Add(new SortDescription("Spos", ListSortDirection.Ascending));
            
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

            ProcessesCV.Refresh();
  
        }
        public void Exit()
        {

            Owner.Exit();
        }


        public void Drop(IDropInfo dropInfo)
        {
            Vorgang vrg = (Vorgang)dropInfo.Data;
            ListCollectionView s = dropInfo.DragInfo.SourceCollection as ListCollectionView;
            ListCollectionView t = dropInfo.TargetCollection as ListCollectionView;
            if (s.CanRemove) s.Remove(vrg);
            int v = dropInfo.InsertIndex;
            vrg.Rid = _rId;
            if (v > t.Count)
            {
                ((IList)t.SourceCollection).Add(vrg);
            }
            else
            {
                ((IList)t.SourceCollection).Insert(v, vrg);
                
            }
            Collection<Vorgang> p = t.SourceCollection as Collection<Vorgang>;
            for(int i=0;i<p.Count;i++)
            {
                p[i].Spos = i;
            }
            t.Refresh();
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
