using Lieferliste_WPF.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Lieferliste_WPF.Working
{
    public class ActionStripe : Stripe
    {
        private string _name;
        private int _start;
        private int _end;

        public double TimeWitdh { get; set; }
        public int CalcLenght { get; set; }

        public override bool ValidateStartEnd(int value) { return true; }
        public override int Start
        {
            get { return _start; }
            set
            {
                _start = value;
            }
        }
        public override int End
        {
            get
            {
                return _end;
            }
            set
            {
                _end = value;
            }
        }
        public override int Type
        {
            get { return 10; }
        }
        public override string Comment
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        //public List<int> workingMinutes
        //{
        //    get { return _workingMinutes; }
        //    set
        //    {
        //        _workingMinutes = value;
        //        int startInd = _workingMinutes.IndexOf(_start);
        //        _end = 0;
        //        if (CalcLenght > 0)
        //        {
        //            if (_workingMinutes.Count >= (startInd + (int)CalcLenght))
        //            {
        //                _end = _workingMinutes[startInd + (int)CalcLenght - 1];
        //            }
        //        }
        //    }
        //}
        public int deadKW { get; set; }
        public DateTime Date { get; set; }
        public String TTNR { get; set; }
        public String VNR { get; set; }
        private static KnownColor[] _KnownColor = new KnownColor[15];

        [System.ComponentModel.DefaultValue(0)]
        public int ColorIndex { get; set; }
        private Dictionary<String, String> _fields = new Dictionary<String, String>();

        #region Constructors
        private ActionStripe() { }

        public ActionStripe(ActionStripe ac)
        {
            this.Date = ac.Date;
            this.deadKW = ac.deadKW;
            this.TimeWitdh = ac.TimeWitdh;
            this._name = ac.Name;
            this.TTNR = ac.TTNR;
            this.VNR = ac.VNR;
            this._start = ac.Start;
            this.ColorIndex = ac.ColorIndex;

        }
        public ActionStripe(String name, double TimeWidth, DateTime date, int deadKW)
        {
            this._name = name;
            this.TimeWitdh = TimeWidth;
            this.Date = date;
            this.deadKW = deadKW;

        }
        #endregion Constructors

        public string Name
        {
            get { return _name; }
            set
            {
                this._name = value;
            }
        }
        public override int TimeLenght
        {
            get
            {
                if (CalcLenght == 0) return 0;
                return _end - _start;
            }
        }
        public override string StripeColor
        {
            get
            {
                var r = Properties.Settings.Default.ColorPallette[ColorIndex];
                return Convert.ToString(r);
            }
        }


        public String ToolTip
        {
            get { return String.Format("{0} {1}\n{2}\nStart: {3:t}\nEnde: {4:t}", Name, VNR, TTNR, StartTime, EndTime); }
        }
        public Dictionary<String, String> Fields
        {
            get { return _fields; }

        }
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            ActionStripe ac = obj as ActionStripe;
            if (ac == null) return false;

            return this.Name.Equals(ac.Name);
        }
        public bool Equals(ActionStripe actionStripe)
        {
            if ((object)actionStripe == null) return false;
            return this.Name.Equals(actionStripe.Name);
        }
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

    }

}

