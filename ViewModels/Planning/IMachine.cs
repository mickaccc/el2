using Lieferliste_WPF.Entities;
using Lieferliste_WPF.Utilities;
using System;

namespace Lieferliste_WPF.Planning
{
    internal interface IMachine
    {
        String MachineName { get; set; }
        int RID { get; set; }
        bool isFilling { get; set; }
        bool isSelected { get; set; }
        ObservableLinkedList<RessZuteilView> ProcessesLine { get; }
        void addKappa(DateTime thisDate, Stripe thisStripe);
        double? addOrder(Process thisOrder);
    }
}
