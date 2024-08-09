using El2Core.Models;
using Prism.Ioc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace ModuleReport.ReportSources
{
    interface IMaterialSource
    {

        List<ReportMaterial> Materials { get; }
        
    }
    internal class MaterialSource : IMaterialSource
    {
        public MaterialSource(IContainerProvider containerProvider)
        {
            container = containerProvider;
            LoadData();
        }
        IContainerProvider container;
        public List<ReportMaterial> Materials { get; private set; } = [];


        private void LoadData()
        {
            using var db = container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();

            var res = db.Vorgangs.AsNoTracking()
                .Include(x => x.Responses)
                .Include(x => x.AidNavigation.DummyMatNavigation)
                .Include(x => x.AidNavigation.MaterialNavigation)
                .Include(x => x.RidNavigation)
                .Where(x => x.Rid != null)
                .ToList();

            foreach (var result in res)
            {
                string? ttnr = string.Empty;
                string? descript = string.Empty;

                if (result.AidNavigation.Material != null)
                {
                    ttnr = result.AidNavigation.Material.ToString();
                    descript = result.AidNavigation.MaterialNavigation?.Bezeichng;
                }
                else if (result.AidNavigation.DummyMat != null)
                {
                    ttnr = result.AidNavigation.DummyMat.ToString();
                    descript = result.AidNavigation.DummyMatNavigation?.Mattext;
                }
                foreach (var item in result.Responses)
                {

                    if (ttnr != null)
                    {

                        var m = new ReportMaterial(ttnr,
                            descript,
                            result.Aid,
                            result.VorgangId,
                            result.Vnr,
                            result.Rid,
                            item.Yield,
                            item.Scrap,
                            item.Rework,
                            item.Timestamp,
                            string.Format("{0}\n{1}", result.RidNavigation.RessName, result.RidNavigation.Inventarnummer));
                            
                        Materials.Add(m);

                    }
                }
            }

        }

    }
        public record ReportMaterial(string TTNR, string? Description, string Order, string VID, int ProcessNr, int? Rid, int Yield, int Scrap, int Rework, DateTime Date_Time, string MachName);
    
}
