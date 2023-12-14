﻿using CompositeCommands.Core;
using El2Core.Constants;
using El2Core.Models;
using El2Core.Utils;
using GongSolutions.Wpf.DragDrop;
using Lieferliste_WPF.Utilities;
using Lieferliste_WPF.ViewModels;
using Lieferliste_WPF.Views;
using Prism.Events;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Lieferliste_WPF.Planning
{
    internal class PlanMachine : DependencyObject, IDropTarget
    {

        #region Constructors
        public PlanMachine() { Initialize(); }
        public PlanMachine(int RID, string name, string inventarNo)
        {
            Initialize();
            _rId = RID;
            Name = name;
            InventNo = inventarNo;
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

        public bool Vis
        {
            get { return (bool)GetValue(VisProperty); }
            set { SetValue(VisProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Vis.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VisProperty =
            DependencyProperty.Register("Vis", typeof(bool), typeof(PlanMachine), new PropertyMetadata(true));

        public ICommand? SetMarkerCommand { get; private set; }
        public ICommand? OpenMachineCommand { get; private set; }
        public ICommand? MachinePrintCommand {  get; private set; }

        private readonly int _rId;

        public int RID { get { return _rId; } }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? InventNo { get; private set; }
        public WorkArea? WorkArea { get; set; }
        public int[]? CostUnits { get; set; }
        protected MachinePlanViewModel? Owner { get; }

        public ObservableCollection<Vorgang>? Processes { get; set; }

        public ICollectionView ProcessesCV { get { return ProcessesCVSource.View; } }
        private IApplicationCommands? _applicationCommands;

        public IApplicationCommands? ApplicationCommands
        {
            get { return _applicationCommands; }
            set
            {
                if (_applicationCommands != value)
                {
                    _applicationCommands = value;
                }
            }
        }

        internal CollectionViewSource ProcessesCVSource { get; set; } = new CollectionViewSource();

        private void Initialize()
        {
            
            SetMarkerCommand = new ActionCommand(OnSetMarkerExecuted, OnSetMarkerCanExecute);
            OpenMachineCommand = new ActionCommand(OnOpenMachineExecuted, OnOpenMachineCanExecute);
            MachinePrintCommand = new ActionCommand(OnMachinePrintExecuted, OnMachinePrintCanExecute);
            Processes = new ObservableCollection<Vorgang>();
            ProcessesCVSource.Source = Processes;
            ProcessesCV.SortDescriptions.Add(new SortDescription("Spos", ListSortDirection.Ascending));
            ProcessesCV.Filter = f => !((Vorgang)f).SysStatus?.Contains("RÜCK") ?? false;
            
        }

        private void MessageReceived(Vorgang vorgang)
        {
            var pr = Processes?.FirstOrDefault(x => x.VorgangId == vorgang.VorgangId);
            if (pr != null)
            {
                pr.SysStatus = vorgang.SysStatus;
                pr.QuantityMiss = vorgang.QuantityMiss;
                pr.QuantityScrap = vorgang.QuantityScrap;
                pr.QuantityRework = vorgang.QuantityRework;
                pr.QuantityYield = vorgang.QuantityYield;
                pr.BemT = vorgang.BemT;

            }
        }

        private bool OnMachinePrintCanExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission(Permissions.MachPrint);
        }

        private void OnMachinePrintExecuted(object obj)
        {
            try
            {
                Printing.DoThePrint(Printing.CreateFlowDocument(obj));
            }
            catch (System.Exception e)
            {
                MessageBox.Show(e.Message, "MachinePrint", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static bool OnSetMarkerCanExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission(Permissions.SETMARK);
        }

        private void OnSetMarkerExecuted(object obj)
        {
            try
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
            catch (System.Exception e)
            {
                MessageBox.Show(e.Message, "SetMarker", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        private bool OnOpenMachineCanExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission(Permissions.OpenMach);
        }

        private void OnOpenMachineExecuted(object obj)
        {
            var ma = new MachineView();
            ma.DataContext = this;
            ma.Show();
        }
        public void Exit()
        {
            Owner?.Exit();
        }


        public void Drop(IDropInfo dropInfo)
        {

            try
            {
                var vrg = (Vorgang)dropInfo.Data;
                var s = dropInfo.DragInfo.SourceCollection as ListCollectionView;
                var t = dropInfo.TargetCollection as ListCollectionView;
                if (s.CanRemove) s.Remove(vrg);
                var v = dropInfo.InsertIndex;
                vrg.Rid = _rId;
                t?.SortDescriptions.RemoveAt(0);
                if (v > t?.Count)
                {
                    ((IList)t.SourceCollection).Add(vrg);
                }
                else
                {
                    Debug.Assert(t != null, nameof(t) + " != null");
                    ((IList)t.SourceCollection).Insert(v, vrg);
                }
                var p = t.SourceCollection as Collection<Vorgang>;
                
                for (var i = 0; i < p.Count; i++)
                {
                    p[i].Spos = i;
                }
                t?.SortDescriptions.Add(new SortDescription("Spos", ListSortDirection.Ascending));
            }
            catch (Exception e)
            {
                string str = string.Format(e.Message + "\n" + e.InnerException);
                MessageBox.Show(str, "ERROR", MessageBoxButton.OK);
            }
        }

        public void DragOver(IDropInfo dropInfo)
        {
            if (PermissionsProvider.GetInstance().GetUserPermission(Permissions.MachDrop))
            {
                if (dropInfo.Data is Vorgang)
                {
                    dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                    dropInfo.Effects = DragDropEffects.Move;
                } 
            }
        }
    }
}
