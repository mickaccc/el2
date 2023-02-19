using Lieferliste_WPF.Entities;
using Lieferliste_WPF.ViewModels;
using Lieferliste_WPF.Working;
using Lieferliste_WPF.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Lieferliste_WPF.Planning
{
    class MachineCreator
    {
        private IMachineFactory _machineFactory = null;

        public void setFactory(IMachineFactory factoryRef)
        {
            this._machineFactory = factoryRef;
        }

        public void fillMachines(MachineContainerViewModel machineContainer)
        {
            using (var ctx = new EntitiesAlloc())
            {
                var machines = ctx.tblRessource
                               .Join(ctx.tblArbeitsplatzZuteilung,
                               r => r.RID,
                               a => a.RID,
                               (r, a) => new { tblRessource = r, tblArbeitsplatzZuteilung = a })
                               .Where(result => result.tblArbeitsplatzZuteilung.BID == machineContainer.BID)
                               .ToList();

                foreach (var zut in machines)
                {
                    IMachine m = _machineFactory.createMachine();
                    m.MachineName = zut.tblRessource.RessName;
                    m.RID = (int)zut.tblRessource.RID;

                    m.isFilling = true;

                    DateUtils.CalendarWeek week = DateUtils.GetGermanCalendarWeek(DateTime.Now);
                    List<Stripe> pause = DbManager.Instance().getPauseList();
                    foreach (var kr in ctx.tblRessKappa.Where(x => x.RID == m.RID))
                    {
                        if (DateUtils.GetGermanCalendarWeek(kr.Datum).CompareTo(week) >= 0)
                        {
                            int start3 = (!kr.start3.HasValue) ? 0 : (int)kr.start3;
                            int end3 = (!kr.end3.HasValue) ? 0 : (int)kr.end3;
                            if (start3 > end3)
                            {
                                m.addKappa(kr.Datum.AddDays(-1), new ShiftThree(start3, ShiftThree.MaxMinute, kr.comment3));
                                m.addKappa(kr.Datum, new ShiftFour(ShiftFour.MinMinute, end3, kr.comment3));
                            }
                            else
                            {
                                m.addKappa(kr.Datum, new ShiftThree(start3, end3, kr.comment3));
                            }
                            foreach (StripePause p in pause)
                            {
                                m.addKappa(kr.Datum, p);
                            }
                            m.addKappa(kr.Datum, new ShiftOne((!kr.start1.HasValue) ? 0 : (int)kr.start1, (!kr.end1.HasValue) ? 0 : (int)kr.end1, kr.comment1));
                            m.addKappa(kr.Datum, new ShiftTwo((!kr.start2.HasValue) ? 0 : (int)kr.start2, (!kr.end2.HasValue) ? 0 : (int)kr.end2, kr.comment2));
                        }
                    }

                    //var query = from r in ctx.RessourceAllocations
                    //            where r.RID == m.RID
                    //            orderby r.SPOS
                    //            select r;

                    //foreach (var dr in query)
                    //{
                    //    Process ord = new Process(dr.AID.Trim());

                    //    ord.ExecutionNumber = String.Format("{0:D4}", dr.VNR);
                    //    ord.ExecutionShortText = dr.Text.Trim();
                    //    ord.deadKW = DateUtils.GetGermanCalendarWeek((DateTime)dr.SpaetEnd);
                    //    ord.LastEnd = (DateTime)dr.SpaetEnd;
                    //    ord.Material = dr.Material.Trim();
                    //    ord.MaterialDescription = dr.MaterialDescription.Trim();
                    //    ord.ProcessTime = (int)Math.Round(dr.BEAZE.GetValueOrDefault(0));
                    //    ord.ProcessCorrect = dr.korrect.GetValueOrDefault(0);
                    //    ord.Quantity = dr.Quantity.GetValueOrDefault(0);
                    //    ord.Quantity_miss = dr.Quantity_miss.GetValueOrDefault(0);
                    //    ord.ProcessRestTime = ord.ProcessTime / ord.Quantity * ord.Quantity_miss + ord.ProcessCorrect;
                    //    ord.CommentMei = dr.Bem_M;
                    //    ord.CommentTL = dr.Bem_T;
                    //    ord.CommentMA = dr.Bem_MA;
                    //    ord.VID = dr.VID;

                    //m.addOrder(ord);

                    //}
                    machineContainer.Machines.Add(new MachineViewModel { Machine = m, MachineContainerViewModel = machineContainer });
                    m.isFilling = false;
                }

                var orderpool = from r in ctx.RessZuteilView
                                where r.RID == null && !r.Text.Contains("Auftrag Starten") && r.BID == machineContainer.BID
                                orderby r.SpaetEnd
                                select r;


                foreach (var zutRow in orderpool)
                {
                    Process pro = new Process(zutRow.AID.Trim());

                    pro.Material = zutRow.Material.Trim();
                    pro.MaterialDescription = zutRow.MaterialDescription;
                    pro.ExecutionNumber = String.Format("{0:D4}", zutRow.VNR);
                    pro.ExecutionShortText = zutRow.Text;
                    pro.Quantity = zutRow.Quantity.GetValueOrDefault(0);
                    pro.Quantity_miss = zutRow.Quantity_miss.GetValueOrDefault(0);
                    pro.Quantity_yield = zutRow.Quantity_yield.GetValueOrDefault(0);
                    pro.ProcessTime = (int)Math.Round(zutRow.BEAZE.GetValueOrDefault(0));
                    pro.ProcessRestTime = (pro.Quantity > 0) ?
                        pro.ProcessTime / pro.Quantity * pro.Quantity_miss + pro.ProcessCorrect : 0;
                    pro.LastEnd = zutRow.SpaetEnd.GetValueOrDefault();
                    pro.deadKW = DateUtils.GetGermanCalendarWeek(pro.LastEnd);
                    pro.CommentMei = zutRow.Bem_M;
                    pro.CommentTL = zutRow.Bem_T;
                    pro.CommentMA = zutRow.Bem_MA;
                    pro.VID = zutRow.VID;

                    machineContainer.OrderPool.Add(pro);
                }

            }
        }

        public void fillParking(MachineContainer machineContainer)
        {
            EnumerableRowCollection<DataSetEL2.RessZuteilViewRow> r = DbManager.Instance().getRessZuteilView().Where(x => x.IsARBIDNull() != false);
            if (_machineFactory.GetType() == typeof(ParkMachineFactory))
            {
                DataSetEL2.RessZuteilViewRow row = r.SingleOrDefault(x => x.BID == machineContainer.BID && x.Type == 3);
                if (row != null)
                {
                    IMachine m = _machineFactory.createMachine();
                    m.isFilling = true;
                    m.MachineName = row.RessName;
                    m.RID = row.RID;
                }
            }
        }
    }
}
