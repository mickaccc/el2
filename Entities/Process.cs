using Lieferliste_WPF.Working;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Lieferliste_WPF.Entities
{
    public class Process
    {

        public event PropertyChangedEventHandler PropertyChanged;
        public String OrderNumber { get; set; }
        public String ExecutionNumber { get; set; }
        public String VID { get; set; }
        public String Material { get; set; }
        public String MaterialDescription { get; set; }
        public String ExecutionShortText { get; set; }
        public bool isArchivated { get; set; }
        private string _CommentMei;
        public String CommentMei
        {
            get { return _CommentMei; }
            set { _CommentMei = value; }
        }
        private string _CommentTL;
        public String CommentTL
        {
            get { return _CommentTL; }
            set { _CommentTL = value; }
        }
        private string _CommentMA;
        public String CommentMA
        {
            get { return _CommentMA; }
            set { _CommentMA = value; }
        }
        public int Quantity { get; set; }
        public int Quantity_yield { get; set; }
        public int Quantity_scrap { get; set; }
        public int Quantity_rework { get; set; }
        public int Quantity_miss { get; set; }
        public int ProcessRestTime { get; set; }
        public int ProcessCorrect { get; set; }
        public int ProcessTime { get; set; }
        public Lieferliste_WPF.DateUtils.CalendarWeek deadKW { get; set; }
        public DateTime LastEnd { get; set; }
        private bool _isHighPrio;
        public bool isHighPrio
        {
            get { return _isHighPrio; }
            set { _isHighPrio = value; }
        }
        private string _CommentHighPrio;
        public String CommentHighPrio
        {
            get { return _CommentHighPrio; }
            set { _CommentHighPrio = value; }
        }
        private string _LieferTermin;
        public String LieferTermin
        {
            get { return _LieferTermin; }
            set { _LieferTermin = value; }
        }
        public String PlanTermin { get; set; }
        public String WorkSpace { get; set; }
        public String WorkPlaceSAP { get; set; }
        public String WorkPlace { get; set; }
        public bool isReady { get; set; }
        public String SysState { get; set; }
        private string _marker;
        public String marker
        {
            get { return _marker; }
            set { _marker = value; }
        }
        private DateTime? _termin;
        public DateTime? Termin
        {
            get { return _termin; }
            set { _termin = value; }
        }
        private bool _isInvisible;
        public bool isInVisible
        {
            get { return _isInvisible; }
            set { _isInvisible = value; }
        }
        private bool _isPortfolioAvail;
        public bool isPortfolioAvail
        {
            get { return _isPortfolioAvail; }
            set { _isPortfolioAvail = value; }
        }

        private List<ActionStripe> _actionStripes = new List<ActionStripe>();

        public Process(String OrderNumber)
        {
            this.OrderNumber = OrderNumber;
        }
        //public List<ActionStripe> ActionStripes
        //{
        //    get { return this._actionStripes; }
        //    set { this._actionStripes = value; }
        //}
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            Process p = obj as Process;
            if (p == null) return false;

            return this.OrderNumber.Equals(p.OrderNumber) && this.ExecutionNumber.Equals(p.ExecutionNumber);
        }
        public bool Equals(Process process)
        {
            if ((object)process == null) return false;

            return this.OrderNumber.Equals(process.OrderNumber) && this.ExecutionNumber.Equals(process.ExecutionNumber);
        }
        public override int GetHashCode()
        {
            return OrderNumber.GetHashCode() ^ ExecutionNumber.GetHashCode();
        }
        private void Changed(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));

        }
    }
}
