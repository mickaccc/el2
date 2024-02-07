using CompositeCommands.Core;
using El2Core.Constants;
using El2Core.Models;
using El2Core.Utils;
using GongSolutions.Wpf.DragDrop;
using Prism.Services.Dialogs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;
using Lieferliste_WPF.Planning;

namespace Lieferliste_WPF.Dialogs.ViewModels
{
    class MachineViewVM : IDialogAware, IDropTarget
    {
        public string Title => "Maschinen Details";
        public PlanMachine PlanMachine { get; private set; }
        public string InventNo { get; private set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int WorkAreaId { get; private set; }
        public List<int>? CostUnits { get; private set; }
        public ObservableCollection<Vorgang>? Processes { get; set; } = new();
        private List<Vorgang> ChangedVrgs { get; set; } = new();
        public ICollectionView? ProcessesCV { get; private set; }
        public event Action<IDialogResult> RequestClose;
        private IApplicationCommands _applicationCommands;
        public IApplicationCommands ApplicationCommands
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

        public MachineViewVM(IApplicationCommands applicationCommands) { _applicationCommands = applicationCommands; }
        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
           
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {

            PlanMachine = parameters.GetValue<PlanMachine>("PlanMachine");
            ProcessesCV = CollectionViewSource.GetDefaultView(PlanMachine.Processes);
            
        }
        public void Drop(IDropInfo dropInfo)
        {

            try
            {
                var vrg = (Vorgang)dropInfo.Data;
                var s = dropInfo.DragInfo.SourceCollection as ListCollectionView;
                var t = dropInfo.TargetCollection as ListCollectionView;
                var v = dropInfo.InsertIndex;
                if (s.CanRemove) s.Remove(vrg);

                Debug.Assert(t != null, nameof(t) + " != null");
                    ((IList)t.SourceCollection).Insert(v, vrg);
                
                var p = t.SourceCollection as Collection<Vorgang>;

                for (var i = 0; i < p.Count; i++)
                {
                    p[i].Spos = (p[i].SysStatus?.Contains("RÜCK") == true) ? 1000 : i;
                    var vv = PlanMachine.Processes?.First(x => x.VorgangId == p[i].VorgangId);
                    vv.Spos = i;
                }
                t.Refresh();
            }
            catch (Exception e)
            {
                string str = string.Format(e.Message + "\n" + e.InnerException);
                MessageBox.Show(str, "ERROR", MessageBoxButton.OK);
            }
        }

        public void DragOver(IDropInfo dropInfo)
        {
            if (PermissionsProvider.GetInstance().GetRelativeUserPermission(Permissions.MachDrop, WorkAreaId))
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
