using System;
using System.Collections.Generic;
using Lieferliste_WPF.Entities;
using System.Collections.ObjectModel;
using Lieferliste_WPF.ViewModels.Support;
using Lieferliste_WPF.ViewModels.Base;
using System.Data.Entity;

namespace Lieferliste_WPF.Planning
{
    internal interface IMachine
    {
        String MachineName { get; set; }
        int RID { get; set; }
        bool isFilling { get; set; }
        bool isSelected { get; set; }
        ObservableLinkedList<RessZuteilView> ProcessesLine { get; }
        void addKappa(DateTime thisDate,Stripe thisStripe);
        double? addOrder(Process thisOrder);
    }
}
