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
        public int YieldSum { get; private set; } = 0;
        public int ReworkSum { get; private set; } = 0;
        public int ScrapSum { get; private set; } = 0;
        private void LoadData()
        {
            using var db = container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();

            var results = db.TblMaterials
                .Include(x => x.OrderRbs)
                .ThenInclude(x => x.Vorgangs)
                .ThenInclude(x => x.Responses)
                .ToList();
            foreach (var result in results)
            {
                var m = new Mat() { TTNR = result.Ttnr, Description = result.Bezeichng };
                foreach (var ord in result.OrderRbs)
                {
                    foreach(var vorg in ord.Vorgangs)
                    {
                        m.Responses = [.. vorg.Responses];
                    }
                }
                Materials.Add(m);
            }
            foreach (var mats in Materials.Where(x => x.Responses != null))
            {
                if (mats.Responses.Any(y => y.Timestamp.Date == DateTime.Today))
                {
                    YieldSum += mats.Responses.Sum(x => x.Yield);
                    ScrapSum += mats.Responses.Sum(x => x.Scrap);
                    ReworkSum += mats.Responses.Sum(x => x.Rework);
                }
            }
        }
        public struct Mat
        {          
            public string TTNR { get; set; }
            public string? Description { get; set; }
            public List<Response> Responses { get; set; }
            public int YieldSum { get { return (Responses == null) ? 0 : Responses.Sum(x => x.Yield); } }
            public int ScrapSum { get {  return (Responses == null) ? 0 : Responses.Sum(x => x.Scrap); } }
            public int ReworkSum { get { return (Responses == null) ? 0 : Responses.Sum(x => x.Rework); } }

        }

        
    }
}
