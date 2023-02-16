using Lieferliste_WPF.Entities;
using Lieferliste_WPF.Planning;
using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Messaging;
using Lieferliste_WPF.Messages;

namespace Lieferliste_WPF.ViewModels
{
    class MachineViewModel: Support.CrudVM
    {
        private IMachine _machine;
        public IMachine Machine
        {
            get { return _machine; }
            set
            {
                _machine = value;
                GetData();
                RaisePropertyChanged("Title");
            }
        }
 
        public ObservableCollection<DayLine> KappaLine { get; private set; }
        public ObservableLinkedList<Process> Processes { get; private set; }
        public MachineContainerViewModel MachineContainerViewModel { get; set; }
        public String Title { get { return Machine.MachineName; } }

        public Int32 RID { get { return Machine.RID; } }

        public MachineViewModel()
        {
            KappaLine = new ObservableCollection<DayLine>();
            Processes = new ObservableLinkedList<Process>();

            
        }

        private void LoadData()
    {
        DataSetTablesTableAdapters.tblRessKappaTableAdapter Kappa = DbManager.Instance().getRessKappa(RID);

    }
        protected override void GetData()
        {
            if (_machine != null)
            {
                using (var data = new EntitiesAlloc())

                {
                    var prop = (from p in data.RessZuteilView
                                where p.RID == RID
                                orderby p.SPOS
                                select p).ToList();
                    
                    foreach (RessZuteilView r in prop)
                    {
                        Process pro = new Process(r.AID);

                        pro.Material = r.Material.Trim();
                        pro.MaterialDescription = r.MaterialDescription;
                        pro.ExecutionNumber = String.Format("{0:D4}", r.VNR);
                        pro.ExecutionShortText = r.Text;
                        pro.Quantity = r.Quantity.GetValueOrDefault(0);
                        pro.Quantity_miss = r.Quantity_miss.GetValueOrDefault(0);
                        pro.Quantity_yield = r.Quantity_yield.GetValueOrDefault(0);
                        pro.ProcessTime = (int)Math.Round(r.BEAZE.GetValueOrDefault(0));
                        pro.ProcessRestTime = (pro.Quantity > 0) ?
                            pro.ProcessTime / pro.Quantity * pro.Quantity_miss + pro.ProcessCorrect : 0;
                        pro.LastEnd = r.SpaetEnd.GetValueOrDefault();
                        pro.deadKW = DateUtils.GetGermanCalendarWeek(pro.LastEnd);
                        pro.CommentMei = r.Bem_M;
                        pro.CommentTL = r.Bem_T;
                        pro.CommentMA = r.Bem_MA;
                        pro.VID = r.VID;

                        Processes.AddLast(pro);
                    }

                }
            }
        }

        internal bool addProcess(Process dragged)
        {
            bool retValue = false;
            double? ret = Machine.addOrder(dragged);
            if (ret != null)
            {
                using (var data = new EntitiesAlloc())
                {
                    var zut = (from p in data.RessZuteilView
                               where p.VID == dragged.VID
                               select p);

                    var maxn = data.tblRessourceVorgang.Where(x => x.RID == Machine.RID).Max(y => y.SPOS);
                    var max = (Int16) ((maxn == null) ? 1 : maxn +1);

                    data.tblRessourceVorgang.Add(new tblRessourceVorgang { RID = Machine.RID, VID=dragged.VID,SPOS=max });
                    retValue = data.SaveChanges() > 0;
                    Processes.AddLast(dragged);
                   
                }
              
            }
            else
            {
                System.Windows.MessageBox.Show(string.Format("Der Vorgang konnte nicht Kalkuliert werden!\n\n"
                    + "Auftrag/Vorgang: {0:S} / {1:D4}\n{2:S}\n\n"
                    + "freie Fertigungskapazität: {3:F}\n"
                    + "benötigte Fertigungskapazität: {4:F}\n"
                    + "offene Menge: {5:D}",dragged.OrderNumber,dragged.ExecutionNumber,
                    dragged.ExecutionShortText,0.2,dragged.ProcessRestTime,dragged.Quantity_miss) , "Kalkulationsfehler", System.Windows.MessageBoxButton.OK);

            }
            return retValue;

        }
        internal void moveProcess(Process dragged, Process target)
        {
            using (var zut = new EntitiesAlloc())
            {
                var zug = (from z in zut.tblRessourceVorgang
                           where z.RID == Machine.RID
                           select z);

                var node = Processes.Find(target);
                Processes.Remove(dragged);
                Processes.AddBefore(node, dragged);
                Int16 s = 1;
                foreach (Process p in Processes)
                {
                    zug.Single(x => x.VID == p.VID).SPOS = s;
                    s++;
                }
            }
        }
    }
}
