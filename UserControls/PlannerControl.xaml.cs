using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using Lieferliste_WPF.Entities;
using Lieferliste_WPF.Working;
using Lieferliste_WPF.Planning;
using System.ComponentModel;
using Lieferliste_WPF.ViewModels;

namespace Lieferliste_WPF.UserControls
{
    /// <summary>
    /// Interaction logic for PlannerControl.xaml
    /// </summary>
    public partial class PlannerControl : UserControl
    {
        public static readonly DependencyProperty ActionProperty = DependencyProperty.Register("ActionList", typeof(List<ActionStripe>), typeof(PlannerControl), new FrameworkPropertyMetadata(new List<ActionStripe>())
        {
            BindsTwoWayByDefault = false,
            AffectsRender = true,
            AffectsArrange = true,
            AffectsParentArrange = true,
            DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
        });

        public List<ActionStripe> ActionList
        {
            get { return (List<ActionStripe>)this.GetValue(ActionProperty); }
            set { this.SetValue(ActionProperty, value); }
        }
        public static readonly DependencyProperty DayProperty = DependencyProperty.Register("DayList", typeof(List<Stripe>), typeof(PlannerControl), new FrameworkPropertyMetadata(new List<Stripe>())
        {
            BindsTwoWayByDefault = false,
            AffectsRender = true,
            AffectsArrange = true,
            AffectsParentArrange = true,
            DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
        });

        public List<Stripe> DayList
        {
            get { return (List<Stripe>)this.GetValue(DayProperty); }
            set { this.SetValue(DayProperty, value); }
        }

        internal IMachine myMachine { get; set; }

        #region Constructor
        public PlannerControl()
        {
            InitializeComponent();
        }
        #endregion Constructor


        public void MoveProcess(ActionStripe target, ActionStripe source)
        {
            (myMachine as InternalMachine).moveOrder(target, source);
            foreach(UserControls.WeekView lst in WeekslistBox.Items)
            {
                   lst.dayView1.ListBox1.ItemsSource = (myMachine as InternalMachine).getActionStripes(lst.dayView1.DateDT);
                   lst.dayView2.ListBox1.ItemsSource = (myMachine as InternalMachine).getActionStripes(lst.dayView2.DateDT);
                   lst.dayView3.ListBox1.ItemsSource = (myMachine as InternalMachine).getActionStripes(lst.dayView3.DateDT);
                   lst.dayView4.ListBox1.ItemsSource = (myMachine as InternalMachine).getActionStripes(lst.dayView4.DateDT);
                   lst.dayView5.ListBox1.ItemsSource = (myMachine as InternalMachine).getActionStripes(lst.dayView5.DateDT);
                   lst.dayView6.ListBox1.ItemsSource = (myMachine as InternalMachine).getActionStripes(lst.dayView6.DateDT);
                   lst.dayView7.ListBox1.ItemsSource = (myMachine as InternalMachine).getActionStripes(lst.dayView7.DateDT);
            }
        }
        public void create()
        {

                DateTime n = DateTime.Now;
                DayView dv = null;
            //do
            //{
            //    WeekView w = new WeekView(n.Year, DateUtils.GetGermanCalendarWeek(n).Week);

            //    foreach (var v in w.stackPanel1.Children)
            //    {
            //        dv = v as DayView;
            //        if (dv != null)
            //        {
            //            //dv. = myMachine;
            //            dv.PlannerControl = this;
            //            if(myMachine.KappaLine.Any(x => x.Day==dv.Date))
            //            {
            //                dv.ShiftPlan.DataContext = myMachine.KappaLine.First(x => x.Day==dv.Date).OrderByDescending(x => x.Type);
                      
            //            dv.ListBox1.DataContext= (myMachine as InternalMachine).getActionStripes(dv.Date.Date);
            //            }
            //        }
            //    }
            //    n = dv.Date.AddDays(1);
            //    int c = myMachine.WorkingLine.Where(x => x.ActionStripes.Any(y => y.Date.Date > dv.Date.Date)).Count();
            //    this.WeekslistBox.Items.Add(w);
            //} while (myMachine.WorkingLine.Where(x => x.ActionStripes.Any(y => y.Date.Date > dv.Date.Date)).Count()>0);

        }
        
        private void WeekslistBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

    }

}
