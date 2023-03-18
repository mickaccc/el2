using Lieferliste_WPF.Data.Models;
using Lieferliste_WPF.Utilities;
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

        public void addKappa(DateTime thisDate, Stripe thisStripe)
        {
            throw new NotImplementedException();
        }

        public double? addOrder(Process thisOrder)
        {
            throw new NotImplementedException();
        }
    }
}
