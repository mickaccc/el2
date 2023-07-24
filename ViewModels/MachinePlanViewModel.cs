﻿
using GongSolutions.Wpf.DragDrop;
using Lieferliste_WPF.Commands;
using Lieferliste_WPF.Data;
using Lieferliste_WPF.Data.Models;
using Lieferliste_WPF.Planning;
using Lieferliste_WPF.Utilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Lieferliste_WPF.ViewModels
{
    
    internal class MachinePlanViewModel : Base.ViewModelBase, IDropTarget
    {      
        private RelayCommand selectionChangeCommand;
        private RelayCommand textSearchCommand;
        public ICommand SelectionChangeCommand => selectionChangeCommand ??= new RelayCommand(SelectionChange);
        public ICommand TextSearchCommand => textSearchCommand ??= new RelayCommand(OnTextSearch);

        public ICommand SaveCommand { get; private set; }
  
        public List<WorkArea> WorkAreas { get; private set; }
        public List<PlanMachine> Machines { get; private set; }
        private ObservableCollection<Vorgang> Priv_processes { get; set; }
        private ObservableCollection<Vorgang> Priv_parking { get; set; }
        
        private readonly ICollectionView _ressCV;
        public ICollectionView ProcessCV { get { return ProcessViewSource.View; } }
        public ICollectionView ParkingCV { get { return ParkingViewSource.View; } }
        
        private string _masterFilterText = "";
        private string _searchFilterText = "";
        internal CollectionViewSource ProcessViewSource { get; private set; } = new();
        internal CollectionViewSource ParkingViewSource { get; private set; } = new();
 
        public MachinePlanViewModel() 
        {
            
            Machines = new List<PlanMachine>();
            SaveCommand = new ActionCommand(OnSaveExecuted, OnSaveCanExecute);

            LoadData();
            _ressCV = CollectionViewSource.GetDefaultView(Machines);
            _ressCV.Filter = f => (f as PlanMachine)?.WorkArea?.WorkAreaId == WorkAreas?.First().WorkAreaId;
            _ressCV.MoveCurrentToFirst();           
            ProcessViewSource.Source = Priv_processes;
            ProcessViewSource.Filter += ProcessCV_Filter;

            ParkingViewSource.Source = Priv_parking;
            ParkingCV.Filter = f => (f as Vorgang)?.ArbPlSapNavigation?.Ressource?.WorkAreaId == WorkAreas?.First().WorkAreaId;
            _masterFilterText = WorkAreas.First().WorkAreaId.ToString();
            ProcessCV.Refresh();
        
        }

        private bool OnSaveCanExecute(object arg)
        {
            return Dbctx.ChangeTracker.HasChanges();
            
        }

        private void OnSaveExecuted(object obj)
        {
            Dbctx.SaveChanges();
        }

        private void LoadData()
        {
 
            var qp = Dbctx.Vorgangs
                .Include(x => x.AidNavigation)
                .ThenInclude(x => x.MaterialNavigation)
                .Include(x => x.AidNavigation.DummyMatNavigation)
                .Include(x => x.RidNavigation)
                .ThenInclude(x => x.RessourceCostUnits)
                .Where(x => !x.AidNavigation.Fertig &&
                !x.SysStatus.Contains("RÜCK") && 
                !x.Text.ToUpper().Contains("AUFTRAG STARTEN"))
                .ToList();


      

            var work = Dbctx.WorkAreas
                .Include(x => x.UserWorkAreas)
                .Where(x => x.UserWorkAreas.Any(x => x.UserId.Equals(AppStatic.User.UserIdent)));

            WorkAreas = new List<WorkArea>(work);

            if (AppStatic.User != null)
            {

                var re = Dbctx.Ressources
                    .Include(x => x.RessourceCostUnits)
                    .Include(x => x.WorkArea)
                    .Include(x => x.WorkSaps)
                    .Where(x => x.WorkArea != null).ToList();

                var ress = re.Where(y => y.RessourceCostUnits.IntersectBy(AppStatic.User.UserCosts.Select(e => e.CostId), a => a.CostId).Any());
                    
               
                foreach (var q in ress)
                {

                        PlanMachine plm = new(q.RessourceId, q.RessName, q.Inventarnummer ,this)
                        {
                            WorkArea = q.WorkArea,
                            CostUnits = q.RessourceCostUnits.Select(x => x.CostId).ToArray(),
                            Description = q.Info ?? String.Empty,
                            ArbPlSAPs = q.WorkSaps.Select(x => x.WorkSapId).ToArray()
                                                     
                        };

                            List<Vorgang> VrgList = qp.FindAll(x => x.Rid == q.RessourceId);
                            
                            foreach (var vrg in VrgList)
                            {
                                if (vrg.VorgangId.Length > 0)
                                    plm.Processes.Add(vrg);
                            }
                          
                    Machines.Add(plm);
                }
                List<Vorgang> list = new();
                foreach(var m in Machines)
                {
                    list.AddRange(qp.FindAll(x => m.ArbPlSAPs.Contains(x.ArbPlSap)));
                }
                Priv_processes = list.FindAll(x => x.Rid == null)
                    .ToObservableCollection();
                Priv_parking = list.FindAll(x => x.Rid == -1)
                    .ToObservableCollection();
            }

        }


        private void SelectionChange(object commandParameter)
        {
            if (commandParameter is SelectionChangedEventArgs sel)
            { 
                if (sel.AddedItems[0] is WorkArea wa)
                {                  
                    _ressCV.Filter = f => (f as PlanMachine)?.WorkArea?.WorkAreaId == wa.WorkAreaId;
                    
                    _masterFilterText = wa.WorkAreaId.ToString();
                    ProcessCV.Refresh();
                                                    
                }
            }
        }
        private void OnTextSearch(object commandParameter)
        {
            if (commandParameter is TextChangedEventArgs change)
            {
                TextBox tb = (TextBox)change.OriginalSource;

                _searchFilterText = tb.Text;
                ProcessCV.Refresh();
                                 
            }
        }
        private void ProcessCV_Filter(object sender, FilterEventArgs e)
        {
            Vorgang v = (Vorgang)e.Item;
            e.Accepted = false;
            if (v.ArbPlSapNavigation?.Ressource?.WorkAreaId?.ToString() == _masterFilterText)
            {
                e.Accepted = true;
                if (!string.IsNullOrWhiteSpace(_searchFilterText))
                {
                    _searchFilterText = _searchFilterText.ToUpper();
                    if (!(e.Accepted = v.Aid.ToUpper().Contains(_searchFilterText)))
                        if (!(e.Accepted = v.AidNavigation?.Material?.ToUpper().Contains(_searchFilterText) ?? false))
                            e.Accepted = v.AidNavigation?.MaterialNavigation?.Bezeichng?.ToUpper().Contains(_searchFilterText) ?? false;
                }
            }
        }
        public void DragOver(IDropInfo dropInfo)
        {
            if (dropInfo.Data is Vorgang)
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                dropInfo.Effects = DragDropEffects.Move;
            }
        }

        internal void Exit()
        {
            Dbctx.SaveChanges();
        }

        public void Drop(IDropInfo dropInfo)

        {
            string? name = dropInfo.VisualTarget.GetValue(FrameworkElement.NameProperty) as string;
            if (dropInfo.Data is Vorgang vrg)
            {
                if (name == "parking")
                {
                    vrg.Rid = -1;
                }
                else
                {
                    vrg.Rid = null;
                }
                ((ListCollectionView)dropInfo.DragInfo.SourceCollection).Remove(vrg);
                ((ListCollectionView)dropInfo.TargetCollection).AddNewItem(vrg);
                ((ListCollectionView)dropInfo.TargetCollection).CommitNew();
            }
        }

    }
}
