using El2Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuleReport.ViewModels
{
    internal class SelectionWorkAreaViewModel
    {
        public SelectionWorkAreaViewModel(IContainerProvider containerProvider)
        {
            container = containerProvider;
            LoadData();
        }
        IContainerProvider container;
        public List<WorkRegion> WorkAreas { get; } = [];

        private void LoadData()
        {
            using var db = container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
            var areas = db.WorkAreas
                .Include(x => x.Ressources)
                .ToList();

            foreach (var area in areas)
            {
                List<Machine> list = [];
                foreach(var m in area.Ressources)
                {
                    list.Add(new(m.RessourceId, m.Inventarnummer, m.RessName));
                }
                var w = new WorkRegion(area.Bereich, list);
                WorkAreas.Add(w);
            }
        }
        public struct WorkRegion
        {
            public WorkRegion(string Name, List<Machine> Machines)
            {
                this.Name = Name;
                this.Machines = Machines;
            }
            public bool IsExpand { get; set; } = false;
            public bool IsChecked { get; set; } = true;
            public string Name { get; }
            public List<Machine> Machines { get; }
        }
        public struct Machine
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
            public bool IsChecked { get; set; } = true;
        }
    }
}
