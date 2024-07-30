using El2Core.Models;
using El2Core.Utils;
using El2Core.ViewModelBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Prism.Events;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Input;

namespace ModuleReport.ViewModels
{
    internal class SelectionWorkAreaViewModel
    {
        public SelectionWorkAreaViewModel(IContainerProvider containerProvider, IEventAggregator eventAggregator)
        {
            container = containerProvider;
            ea = eventAggregator; 
            LoadData();
        }
        IContainerProvider container;
        IEventAggregator ea;
        public List<WorkRegion> WorkAreas { get; } = [];

        private void LoadData()
        {           
            using var db = container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
            var areas = db.WorkAreas
                .Include(x => x.Ressources)
                .ToList();

            foreach (var area in areas)
            {
                if (UserInfo.User.AccountWorkAreas.Any(x => x.WorkAreaId == area.WorkAreaId))
                {
                    List<Machine> list = [];
                    foreach (var m in area.Ressources)
                    {
                        Machine mach = new(m.RessourceId, m.Inventarnummer, m.RessName);
                        mach.PropertyChanged += Mach_PropertyChanged;
                        list.Add(mach);
                    }
                    var w = new WorkRegion(area.Bereich, list);
                    WorkAreas.Add(w); 
                }
            }
        }

        private void Mach_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is Machine machine)
                ea.GetEvent<MessageReportFilterWorkAreaChanged>().Publish((machine.InventNo, machine.IsChecked));
        }

        public class WorkRegion
        {
            public WorkRegion(string Name, List<Machine> Machines)
            {
                this.Name = Name;
                this.Machines = Machines;
            }
            public bool IsExpand { get; set; } = false;
            private bool isChecked = true;
            public bool IsChecked
            {
                get { return isChecked; }
                set
                {
                    isChecked = value;
                    ActivationChanged();                   
                }
            }
            public string Name { get; }
            public List<Machine> Machines { get; }

            private void ActivationChanged()
            {
                Machines.ForEach(x => x.IsChecked = isChecked);
            }
        }
        public class Machine : ViewModelBase
        {
            public Machine(int ressid, string inventno, string name)
            {
                RessId = ressid;
                Name = name;
                InventNo = inventno;
            }
            public int RessId { get; }
            public string Name { get; }
            public string InventNo { get; }
            private bool isChecked = true;
            public bool IsChecked
            {
                get { return isChecked; }
                set
                {
                    isChecked = value;
                    NotifyPropertyChanged(() => IsChecked);
                }
            }
        }
    }
}
