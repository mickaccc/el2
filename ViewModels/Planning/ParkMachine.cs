using Lieferliste_WPF.Entities;
using System;

namespace Lieferliste_WPF.Planning
{
    class ParkMachine : IMachine
    {
        public string MachineName { get; set; }

        public int RID { get; set; }

        public bool isFilling { get; set; }

        public bool isSelected { get; set; }

        public System.Collections.ObjectModel.ObservableCollection<DayLine> KappaLine
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

        public ObservableLinkedList<RessZuteilView> ProcessesLine
        {
            get
            {
                throw new NotImplementedException();
            }

        }

        public void addKappa(DateTime thisDate, Entities.Stripe thisStripe)
        {
            throw new NotImplementedException();
        }

        public double? addOrder(Entities.Process thisOrder)
        {
            throw new NotImplementedException();
        }
    }
}
