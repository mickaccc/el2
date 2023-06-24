using Lieferliste_WPF.ViewModels.Base;
using Lieferliste_WPF.Working;
using Lieferliste_WPF.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Lieferliste_WPF.ViewModels.Support;
using Lieferliste_WPF.Data.Models;
using Lieferliste_WPF.ViewModels;

namespace Lieferliste_WPF.Planning
{
    class InternalMachine : ViewModelBase, IMachine
    {
        #region Members
        delegate void load();

        private LinkedList<Process> _workingLine = new LinkedList<Process>();

        public MachineViewModel MachineViewModel { get; set; }
        public string MachineName { get; set; }
        public int RID { get; set; }
        public bool isFilling { get; set; }
        private bool _isSelected = false;
        public Collection<WorkingWeek> WorkingWeeks { get; set; }
        public IEnumerable<WorkingWeek> FullWorkingDays
        {
            get
            {
                DateUtils.CalendarWeek cw = DateUtils.GetGermanCalendarWeek(DateTime.Now.Date);
                return WorkingWeeks.Where(x => x.CalendarWeek.Equals(cw) || x.getWorkingDays.Any(y => y.ActionStripes.Count > 0));
            }
        }

        public ObservableLinkedList<Ressource> ProcessesLine
        { get; internal set; }

        public List<ActionStripe> getActionStripes(DateTime date)
        {
            List<ActionStripe> ret = new List<ActionStripe>();
            foreach (Process pro in _workingLine)
            {
                //ret.AddRange(pro.ActionStripes.Where(x => x.Date == date));
            }
            return ret;
        }

        public bool isSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
            }
        }
        #endregion Members

        public InternalMachine()
        {
            MachineViewModel = new MachineViewModel();
            WorkingWeeks = new ObservableCollection<WorkingWeek>();
            ProcessesLine = new ObservableLinkedList<Ressource>();

        }

        #region Methods

        public void addKappa(DateTime thisDate, Stripe thisStripe)
        {
            WorkingWeek wd;
            DateUtils.CalendarWeek cw = DateUtils.GetGermanCalendarWeek(thisDate);
            if (!WorkingWeeks.Any(x => x.CalendarWeek.Equals(cw)))
            {

                wd = new WorkingWeek(cw);
                wd.getWorkingDays.First(x => x.Date == thisDate).Kappa.Add(thisStripe);
                WorkingWeeks.Add(wd);
            }
            else
            {
                wd = WorkingWeeks.First(x => x.CalendarWeek.Equals(cw));
                wd.getWorkingDays.First(x => x.Date == thisDate).Kappa.Add(thisStripe);
            }
            if (!isFilling && _workingLine.Count > 0)
            {
                PlannerTools.Reorganize(ref _workingLine);
            }
        }

        public double? addOrder(Process thisOrder)
        {
            double? dt = PlannerTools.planningTest(thisOrder.ProcessRestTime, thisOrder.deadKW,
                WorkingWeeks);
            if (dt > 0)
            {
                PlannerTools.insertForce(thisOrder, WorkingWeeks);
                //ProcessesLine.AddLast(thisOrder);

            }
            return dt - thisOrder.ProcessRestTime;
        }
        public void moveOrder(Process target, Process source)
        {

            PlannerTools.moveForce(target, source, ref _workingLine);

            int index = _workingLine.TakeWhile(x => !x.Equals(source)).Count();
            for (int p = index; p < _workingLine.Count; p++)
            {
                Process pp = _workingLine.ElementAt(p);


            }
            foreach (Process p in _workingLine)
            {
                //foreach (ActionStripe ac in p.ActionStripes)
                //{
                //    System.Diagnostics.Debug.WriteLine(String.Format("ActionStripe target: {0}, {1}, {2}, {3:D}, {4}, {5}",
                //        ac.Name, ac.Start, ac.End, ac.Date, ac.CalcLenght, ac.ColorIndex));
                //}
            }
        }
        public void moveOrder(ActionStripe targetActionStripe, ActionStripe sourceActionStripe)
        {

            //Process target = _workingLine.First(x => x.ActionStripes.Contains(targetActionStripe));
            //Process source = _workingLine.First(x => x.ActionStripes.Contains(sourceActionStripe));

            //    moveOrder(target, source);

        }
        public bool Remove(Process order)
        {
            LinkedListNode<Process> node = _workingLine.Find(order);
            LinkedListNode<Process> preOrder = node.Previous;
            _workingLine.Remove(node);
            //DateTime dt = node.Value.ActionStripes.First().Date;
            //if (preOrder != null)
            //{
            //    dt = preOrder.Value.ActionStripes.Last().Date;
            //}
            bool ret = PlannerTools.Reorganize(ref _workingLine);

            return ret;

        }
        #endregion Methods

    }
}
