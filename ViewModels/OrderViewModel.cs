using Lieferliste_WPF.Entities;
using Lieferliste_WPF.ViewModels.Support;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lieferliste_WPF.ViewModels
{
    class OrderViewModel : CrudVM
    {
        //DB_COS_LIEFERLISTE_SQLEntities ctx = new DB_COS_LIEFERLISTE_SQLEntities();
        public List<OrderList_Result> Processes { get; set; }
        public String Title { get; private set; }
        public String Material { get; private set; }
        public String MaterialDescription { get; private set; }
        public int? Quantity { get; private set; }
        public String sysStatus { get; private set; }
        static OrderViewModel _this = new OrderViewModel();
        protected OrderViewModel()
        {
            LoadData();
        }

        private void LoadData()
        {

            Processes = (from o in dbAlloc.OrderList(null) select o).ToList();
        }

        public static OrderViewModel This
        {
            get { return _this; }
        }

        public void ReLoad(String OrderNumber)
        {
            Processes = (from o in dbAlloc.OrderList(OrderNumber) select o).ToList();
            var t = Processes.FirstOrDefault();
            if (t != null)
            {
                //DataSetEL4.OrderListDataTable dataTable = DbManager.Instance().getOrderList(OrderNumber);
                //if (dataTable.Rows.Count > 0) Processes.Clear();

                //DataSetEL4.OrderListRow firstRow = dataTable.Rows[0] as DataSetEL4.OrderListRow;
                this.Material = t.Material;
                this.Title = OrderNumber;
                this.MaterialDescription = t.Teil;
                this.Quantity = t.Quantity;
                this.sysStatus = t.SysStatus;
            }
            //foreach (DataSetEL4.OrderListRow row in dataTable)
            //{
            //    Process pro = new Process(row.AID);
            //    pro.isFilling = true;
            //    pro.ExecutionNumber = String.Format("{0:d4}", row.VNR);
            //    pro.ExecutionShortText = row.Text;
            //    Material = row.Material;
            //    pro.Quantity_yield = row.Is_Quantity_yieldNull() ? 0 : row._Quantity_yield;
            //    pro.Quantity_scrap = row.Is_Quantity_scrapNull() ? 0 : row._Quantity_scrap;
            //    //pro.Quantity_rework = row.Is_Quantity_reworkNull() ? 0 : row._Quantity_rework;
            //    pro.Quantity_miss = row.Is_Quantity_missNull() ? 0 : row._Quantity_miss;

            //    pro.CommentMei = row.Bem_M;
            //    pro.CommentTL = row.Bem_T;
            //    pro.CommentMA = row.Bem_MA;

            //    //pro.isHighPrio = row.Dringend;
            //    //pro.CommentHighPrio = row.Bemerkung;

            //    pro.isReady = row.fertig;
            //    pro.isInVisible = row.ausgebl;
            //    pro.isPortfolioAvail = row.Mappe;

            //    //pro.marker = row.marker;
            //    pro.PlanTermin = row.Plantermin;
            //    pro.LieferTermin = row.LieferTermin;
            //    pro.Termin = row.IsTerminNull() ? (DateTime?)null : row.Termin;
            //    pro.LastEnd = row.SpaetEnd;

            //    //pro.WorkPlace = row.Arbeitsplatz;
            //    pro.WorkPlaceSAP = row.ARBID;
            //    pro.WorkSpace = row.Bereich;
            //    pro.isFilling = false;
            //    Processes.Add(pro);

            //}
            RaisePropertyChanged("Processes");
            RaisePropertyChanged("Title");
            RaisePropertyChanged("Material");
            RaisePropertyChanged("MaterialDescription");
            RaisePropertyChanged("Quantity");
            RaisePropertyChanged("sysStatus");

        }

    }
}
