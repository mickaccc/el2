using El2Core.Models;
using Microsoft.EntityFrameworkCore;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuleReport.ViewModels
{
    internal class MaterialResultListViewModel
    {
        public MaterialResultListViewModel(IContainerProvider containerProvider)
        { 
            container = containerProvider;
            LoadData();
        }
        IContainerProvider container;
        public List<Mat> Materials { get; private set; } = [];
        public int Sum { get; private set; } = 0;
        private void LoadData()
        {
            using var db = container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();

            var results = db.TblMaterials
                .Include(x => x.OrderRbs)
                .ThenInclude(x => x.Vorgangs)
                .ThenInclude(x => x.RidNavigation)
                .ToList();
            foreach (var result in results)
            {
                Materials.Add(new() { TTNR = result.Ttnr, Description = result.Bezeichng, Response = 2 });
                Sum += 2;
            }
        }
        public struct Mat
        {          
            public string TTNR { get; set; }
            public string? Description { get; set; }
            public int Response { get; set; }

        }

        
    }
}
