using Lieferliste_WPF.Working;
using Lieferliste_WPF.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Lieferliste_WPF.Data.Models
{
    public class Process
    {

        public event PropertyChangedEventHandler PropertyChanged;
        public string OrderNumber { get; set; }
        public string ExecutionNumber { get; set; }
        public string VID { get; set; }
        public string Material { get; set; }
        public string MaterialDescription { get; set; }
        public string ExecutionShortText { get; set; }
        public bool isArchivated { get; set; }
        private string _CommentMei;
        public string CommentMei
        {
            get { return _CommentMei; }
            set { _CommentMei = value; }
        }
        private string _CommentTL;
        public string CommentTL
        {
            get { return _CommentTL; }
            set { _CommentTL = value; }
        }
        private string _CommentMA;
        public string CommentMA
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
        public DateUtils.CalendarWeek deadKW { get; set; }
        public DateTime LastEnd { get; set; }
        private bool _isHighPrio;
        public bool isHighPrio
        {
            get { return _isHighPrio; }
            set { _isHighPrio = value; }
        }
        private string _CommentHighPrio;
        public string CommentHighPrio
        {
            get { return _CommentHighPrio; }
            set { _CommentHighPrio = value; }
        }
        private string _LieferTermin;
        public string LieferTermin
        {
            get { return _LieferTermin; }
            set { _LieferTermin = value; }
        }
        public string PlanTermin { get; set; }
        public string WorkSpace { get; set; }
        public string WorkPlaceSAP { get; set; }
        public string WorkPlace { get; set; }
        public bool isReady { get; set; }
        public string SysState { get; set; }
        private string _marker;
        public string marker
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

        public Process(string OrderNumber)
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

            return OrderNumber.Equals(p.OrderNumber) && ExecutionNumber.Equals(p.ExecutionNumber);
        }
        public bool Equals(Process process)
        {
            if (process == null) return false;

            return OrderNumber.Equals(process.OrderNumber) && ExecutionNumber.Equals(process.ExecutionNumber);
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
