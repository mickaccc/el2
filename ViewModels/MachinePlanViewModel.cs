
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
        private ObservableCollection<Vorgang> Processes { get; set; }
        public ObservableCollection<Vorgang> Parking { get; set; }
        private List<dynamic> _processesAll = new();
        private readonly ICollectionView _ressCV;
        public ICollectionView ProcessCV { get { return ProcessViewSource.View; } }
        private readonly ICollectionView _parkingCV;
        static readonly DataContext _db = new();
        private string _masterFilterText = "";
        private string _searchFilterText = "";
        internal CollectionViewSource ProcessViewSource { get; private set; } = new();
 
        public MachinePlanViewModel() 
        {
            
            Machines = new List<PlanMachine>();
            SaveCommand = new ActionCommand(OnSaveExecuted, OnSaveCanExecute);

            LoadData();
            _ressCV = CollectionViewSource.GetDefaultView(Machines);
            _ressCV.Filter = f => (f as PlanMachine).WorkArea.WorkAreaId == WorkAreas.First().WorkAreaId;
            _ressCV.MoveCurrentToFirst();
            CollectionViewSource cvs = new();
            ProcessViewSource.Filter += ProcessCV_Filter;
            ProcessViewSource.Source = Processes;
                   
            _parkingCV = CollectionViewSource.GetDefaultView(Parking);
            _parkingCV.Filter = f => (f as Vorgang).ArbPlSapNavigation.Ressource.WorkAreaId == WorkAreas.First().WorkAreaId;
            _masterFilterText = WorkAreas.First().WorkAreaId.ToString();
            ProcessCV.Refresh();
         
        }



        private bool OnSaveCanExecute(object arg)
        {
            return _db.ChangeTracker.HasChanges();
            
        }

        private void OnSaveExecuted(object obj)
        {
            _db.SaveChanges();
        }

        private void LoadData()
        {
 

            var qp = _db.Vorgangs
                .Include(x => x.AidNavigation)
                .ThenInclude(x => x.MaterialNavigation)
                .Include(x => x.AidNavigation.DummyMatNavigation)
                .Include(x => x.RidNavigation)
                .ThenInclude(x => x.RessourceCostUnits)
                .Include(x => x.ArbPlSapNavigation)
                .Where(x => !x.AidNavigation.Fertig &&
                !x.SysStatus.Contains("RÜCK") && 
                !x.Text.ToUpper().Contains("AUFTRAG STARTEN"))
                .ToList();


      

            var work = _db.WorkAreas
                .Include(x => x.UserWorkAreas)
                .Where(x => x.UserWorkAreas.Any(x => x.UserId.Equals(AppStatic.User.UserIdent)));

            WorkAreas = new List<WorkArea>(work);

            if (AppStatic.User != null)
            {

                var re = _db.Ressources
                    .Include(x => x.WorkSaps)
                    .Include(x => x.RessourceCostUnits)
                    .Include(x => x.WorkArea)
                    .Where(x => x.WorkArea != null).ToList();

                var ress = re.Where(y => y.RessourceCostUnits.IntersectBy(AppStatic.User.UserCosts.Select(e => e.CostId), a => a.CostId).Any());
                    
               
                foreach (var q in ress)
                {

                        PlanMachine plm = new(q.RessourceId, q.RessName, q.Inventarnummer ,this)
                        {
                            WorkArea = q.WorkArea,
                            CostUnits = q.RessourceCostUnits.Select(x => x.CostId).ToArray(),
                            Description = q.Info,
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
                Processes = list.FindAll(x => x.Rid == null)
                    .ToObservableCollection();
                Parking = list.FindAll(x => x.Rid == -1)
                    .ToObservableCollection();
            }

        }


        //public bool DragProcess(String VID,int RID)
        //{
        //    if(_db.ChangeTracker.HasChanges()) { _db.SaveChanges(); }
            
        //    var pro = Processes.FirstOrDefault(x => x.VorgangId == VID);
        //    var proB = _processesAll.First(x => x.v.VorgangId == VID);
            

        //        PlanMachine? p_new = Machines.FirstOrDefault(x => x.RID == RID);
        //        if (pro != null) //new planning
        //        {
        //            RessourceVorgang rv_new = new() { Vid = VID, Rid = RID };
                    
        //            Processes.Remove(pro);
        //            _db.RessourceVorgangs.Add(rv_new);
        //            p_new.Processes.Add(pro);
                    
        //            _processCV.Refresh();
                    
        //            return true;
        //        }
        //        else if(proB.r.RessourceId == RID) //sort in same machine
        //        {

        //        }
        //        else if(proB.r.RessourceId == 999999) //parking
        //        {

        //        }
        //        else if(RID == 0 && proB.resv != null)//remove from machine
        //        {
        //            int r = proB.resv.Rid;
        //            PlanMachine plm = Machines.First(x => x.RID == proB.resv.Rid);
                    
        //            RessourceVorgang rv_old = _db.RessourceVorgangs.First(x => x.Vid == VID && x.Rid == r);
                    
        //            plm.Processes.Remove(proB);
        //            _db.RessourceVorgangs.Attach(rv_old);
        //            _db.RessourceVorgangs.Remove(rv_old);
                    
        //            Processes.Add(proB);
        //            _processCV.Refresh();
                    
        //            return true;
        //        }
        //        else if(proB.resv != null) //drag to another machine
        //        {
        //            if (proB.resv.Rid != RID && RID != 0 && RID != 999999)
        //            {
        //                int r = proB.resv.Rid;
        //                PlanMachine p_old = Machines.First(x => x.RID == r);
        //                RessourceVorgang rv_new = new() { Vid=VID, Rid=RID };
        //                RessourceVorgang rv_old = _db.RessourceVorgangs.First(x => x.Vid == VID && x.Rid == r);
        //                _db.RessourceVorgangs.Attach(rv_old);
        //                _db.RessourceVorgangs.Remove(rv_old);
        //                _db.RessourceVorgangs.Add(rv_new);
        //                p_old.Processes.Remove(proB);
        //                p_new.Processes.Add(proB);

        //                return true;
        //            }
        //        }
            
        //    return false;
        //}

        private void SelectionChange(object commandParameter)
        {
            if (commandParameter is SelectionChangedEventArgs sel)
            { 
                if (sel.AddedItems[0] is WorkArea wa)
                {                  
                    _ressCV.Filter = f => (f as PlanMachine).WorkArea.WorkAreaId == wa.WorkAreaId;
                    
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
            if (v.ArbPlSapNavigation.Ressource.WorkAreaId.ToString() == _masterFilterText)
            {
                e.Accepted = true;
                if (!string.IsNullOrWhiteSpace(_searchFilterText))
                {
                    if (v.Aid.ToUpper().Contains(_searchFilterText.ToUpper()) ||
                        v.AidNavigation.Material.ToUpper().Contains(_searchFilterText.ToUpper()) ||
                        v.AidNavigation.MaterialNavigation.Bezeichng.ToUpper().Contains(_searchFilterText.ToUpper()))
                    {
                        e.Accepted = true;
                    }
                    else e.Accepted = false;
                }
             
            } else { e.Accepted = false; }

        }
        public void DragOver(IDropInfo dropInfo)
        {
            if (dropInfo.Data as dynamic)
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                dropInfo.Effects = DragDropEffects.Move;
            }
        }

        internal void Exit()
        {
            _db.SaveChanges();
        }

        public void Drop(IDropInfo dropInfo)

        {
            string name = dropInfo.VisualTarget.GetValue(FrameworkElement.NameProperty) as string;
            if (dropInfo.Data is Vorgang vrg)
            {
                if (name == "parking")
                {
                    vrg.Rid = -1;
                }
                if (name == "UnPlaned")
                {
                    vrg.Rid = null;
                }
                ((IList)dropInfo.DragInfo.SourceCollection).Remove(vrg);
                ((IList)dropInfo.TargetCollection).Add(vrg);
            }
        }

    }
}
