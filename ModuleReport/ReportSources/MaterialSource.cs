﻿using El2Core.Models;
using El2Core.Utils;
using El2Core.ViewModelBase;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace ModuleReport.ReportSources
{
    interface IMaterialSource
    {

        ObservableCollection<ReportMaterial> Materials { get; }

    }
    internal class MaterialSource : ViewModelBase, IMaterialSource
    {
        public MaterialSource(IContainerProvider containerProvider, IEventAggregator eventAggregator)
        {
            container = containerProvider;
            ea = eventAggregator;
            ea.GetEvent<MessageReportChangeSource>().Subscribe(OnSourceChange);
            _ = LoadDefaultDataAsync();
        }

        private void OnSourceChange(int obj)
        {
            if (obj == 0) _ = LoadDefaultDataAsync();
            if (obj == 1) _ = LoadSapDataAsync();
        }

        IContainerProvider container;
        IEventAggregator ea;
        public ObservableCollection<ReportMaterial> Materials { get; private set; } = [];
        public long MatCount
        {
            get {  return Materials.Count; }
        }
        private List<Vorgang> defaultVrg;
        private List<Vorgang> sapVrg;

        private async Task<List<Vorgang>> TakeDefaults()
        {
            using var db = container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();

            var res = await db.Vorgangs.AsNoTracking()
                .Include(x => x.Responses)
                .Include(x => x.AidNavigation.DummyMatNavigation)
                .Include(x => x.AidNavigation.MaterialNavigation)
                .Include(x => x.RidNavigation)
                .Where(x => x.Rid != null)
                .ToListAsync();
            return res;
        }
        private async Task<List<Vorgang>> TakeSaps()
        {
            using var db = container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();

            var vorg = await db.Vorgangs.AsNoTracking()
                .Include(x => x.AidNavigation.MaterialNavigation)
                .Include(x => x.AidNavigation.DummyMatNavigation)
                .Include(x => x.ArbPlSapNavigation.Ressource.WorkArea)
                .Include(x => x.Responses)
                .Where(x => x.Responses.Count > 0
                    && x.ArbPlSapNavigation.Ressource.WorkAreaId != 0
                    && x.SpaetEnd.Value.Year == DateTime.Now.Month)
                .ToListAsync();
            return vorg;
        }
        private async Task LoadDefaultDataAsync()
        {
            Materials.Clear();
            List<ReportMaterial> temp = [];
            defaultVrg ??= await TakeDefaults();
            foreach (var result in defaultVrg)
            {
                string? ttnr = null;
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

                        temp.Add(m);

                    }
                }
            }
            Materials.AddRange(temp);
        }
        public async Task LoadSapDataAsync()
        {
            Materials.Clear();

            sapVrg ??= await TakeSaps();

            var result = await Task.Run(() =>
            {
                List<ReportMaterial> temp = [];
                foreach (var result in sapVrg)
                {
                    string? ttnr = null;
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
                                result.ArbPlSapNavigation?.RessourceId,
                                item.Yield,
                                item.Scrap,
                                item.Rework,
                                item.Timestamp,
                                string.Format("{0}\n{1}", result.ArbPlSapNavigation?.Ressource?.RessName, result.ArbPlSapNavigation?.Ressource?.Inventarnummer));

                            temp.Add(m);

                        }
                    }
                }
                return temp;
            });
            Materials.AddRange(result);
        }

    }
    public record ReportMaterial(string TTNR, string? Description, string Order, string VID, int ProcessNr, int? Rid, int Yield, int Scrap, int Rework, DateTime Date_Time, string MachName);

}
