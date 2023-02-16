using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Lieferliste_WPF.Entities;
using Lieferliste_WPF.UserControls;
using Lieferliste_WPF.Working;
using System.Collections.ObjectModel;
using Lieferliste_WPF.Planning;
using Lieferliste_WPF.ViewModels;

namespace Lieferliste_WPF.Dialogs
{
    /// <summary>
    /// Interaction logic for SchDialog.xaml
    /// </summary>
    public partial class SchDialog : Window
    {

        private IEnumerable<Stripe> _dayStripes;
        private InternalMachine _parentDV;
        public SchDialog(DayView dv)
        {
            InitializeComponent();
            System.Drawing.Color col;
            col = Lieferliste_WPF.Properties.Settings.Default.Stripe1;
            grd_S1.Background = new SolidColorBrush(Color.FromArgb(col.A, col.R, col.G, col.B));
            col = Lieferliste_WPF.Properties.Settings.Default.Stripe2;
            grd_S2.Background = new SolidColorBrush(Color.FromArgb(col.A, col.R, col.G, col.B));
            col = Lieferliste_WPF.Properties.Settings.Default.Stripe3;
            grd_S3.Background = new SolidColorBrush(Color.FromArgb(col.A, col.R, col.G, col.B));
            _parentDV = dv.DataContext as InternalMachine;
            //this.actualDate.Text = _parentDV.Date.ToString("dd.MM.yyyy");

            if (_parentDV.WorkingWeeks.Count > 0)
            {
                //_dayStripes = (IEnumerable<Stripe>)_parentDV.Kappa;
                //this.grd_S1.DataContext = _dayStripes.SingleOrDefault(x => x.Type == 1);
                //this.grd_S2.DataContext = _dayStripes.SingleOrDefault(x => x.Type == 2);
                //Stripe tre = null;
                //if (_parentDV.Kappa.Any(x => x.Day==dv.Date.AddDays(-1)))
                //{
                //    tre = _parentDV.PlannerControl.myMachine.KappaLine.First(x => x.Day==dv.Date.AddDays(-1)).LastOrDefault(x => x.Type == 3);
                //}
                //this.grd_S3.DataContext = _dayStripes.SingleOrDefault(x => x.Type == 4);
                //this.txt_start3.DataContext = tre;
                //this.scrb_start3.DataContext = tre;
            }
        }


        private void btn_save_Click(object sender, RoutedEventArgs e)
        {

            //DateTime tmpDt = _parentDV.Date;
            //DateTime endDt = (this.dtp_rngEnd.SelectedDate==null) ? tmpDt:(DateTime)this.dtp_rngEnd.SelectedDate;


            //int rid = _parentDV.PlannerControl.myMachine.RID;
            //ObservableCollection<DayLine> mdays = null;

            
            //TimeSpan? st1,st2,st3, end1,end2,end3;
            //TimeSpan st, end;
            //st1 = (TimeSpan.TryParse(this.txt_start1.Text, out st)) ? st : (TimeSpan?)null;
            //end1 = ((TimeSpan.TryParse(this.txt_end1.Text, out end)) && st1!=null) ? end : (TimeSpan?)null;
            //st2 = (TimeSpan.TryParse(this.txt_start2.Text, out st)) ? st : (TimeSpan?)null;
            //end2 = ((TimeSpan.TryParse(this.txt_end2.Text, out end)) && st2!=null) ? end : (TimeSpan?)null;
            //st3 = (TimeSpan.TryParse(this.txt_start3.Text, out st)) ? st : (TimeSpan?)null;
            //end3 = ((TimeSpan.TryParse(this.txt_end3.Text, out end)) && st3!=null) ? end : (TimeSpan?)null;


            //    mdays = _parentDV.PlannerControl.myMachine.KappaLine;
            //    while (tmpDt <= endDt)
            //    {
            //        if ((!(tmpDt.DayOfWeek == DayOfWeek.Sunday)) &&
            //            (!(tmpDt.DayOfWeek == DayOfWeek.Saturday)) &&
            //            (!(DbManager.Instance().isHolyday(tmpDt))) ||
            //            (tmpDt==endDt))
            //        {
            //            if (!(mdays.Any(x => x.Day==tmpDt)))
            //            {
            //                DayLine dayLine = new DayLine(tmpDt);
                            
                            
            //                if (st1!=null && end1!=null)
            //                {
            //                    dayLine.Add(new ShiftOne((TimeSpan)st1,(TimeSpan)end1,this.comment1.Text));
                               
            //                }
            //                if (st2!=null && end2!=null)
            //                {
            //                    dayLine.Add(new ShiftTwo((TimeSpan)st2, (TimeSpan)end2, this.comment2.Text));
            //                }
            //                if (st3!=null && end3!=null)
            //                {
            //                    dayLine.Add(new ShiftThree(ShiftThree.MinMinute, (int)((TimeSpan)end3).TotalMinutes, this.comment3.Text));
            //                    if (!mdays.Any(x => x.Day==tmpDt.AddDays(-1)))
            //                    {
            //                        DayLine dayLinePre = new DayLine(tmpDt.AddDays(-1));
            //                        dayLinePre.Add(new ShiftThree((int)((TimeSpan)st3).TotalMinutes,ShiftThree.MaxMinute,this.comment3.Text));
            //                        mdays.Add(dayLinePre);
            //                        DbManager.Instance().InsertRessKappa(rid, dayLinePre);
            //                    }
            //                        Stripe stripe = mdays.First(x => x.Day==tmpDt.AddDays(-1)).Where(x => x.Type== 3 && x.End==ShiftThree.MaxMinute).SingleOrDefault();
            //                        if (stripe != null) mdays.First(x => x.Day==tmpDt.AddDays(-1)).Remove(stripe);
            //                        mdays.First(x => x.Day==tmpDt.AddDays(-1)).Add(new ShiftThree((int)((TimeSpan)st3).TotalMinutes,ShiftThree.MaxMinute,this.comment3.Text));
            //                        DbManager.Instance().UpdateRessKappa(rid,mdays.First(x => x.Day==tmpDt.AddDays(-1)));
            //                }
            //                mdays.Add(dayLine);
            //                DbManager.Instance().InsertRessKappa(rid, dayLine);
                          
            //            }
            //            else
            //            {
            //              if (chk_edit1.IsChecked.Equals(true))
            //                {
            //                    Stripe stripe = mdays.First(x => x.Day==tmpDt).Where(x => x.Type == 1).SingleOrDefault();
            //                  if (stripe!=null)
            //                    {
            //                        mdays.First(x => x.Day==tmpDt).Remove(stripe);
            //                        mdays.First(x => x.Day==tmpDt).Updated = DateTime.Now;
            //                  }
            //                  if (st1!=null && end1!=null)
            //                  {
            //                      mdays.First(x => x.Day==tmpDt).Add(new ShiftOne((TimeSpan)st1,(TimeSpan)end1,this.comment1.Text));
            //                      mdays.First(x => x.Day==tmpDt).Updated = DateTime.Now;
            //                    }
            //                }
            //                if (chk_edit2.IsChecked.Equals(true))
            //                {

            //                    Stripe stripe = mdays.First(x => x.Day==tmpDt).Where(x => x.Type == 2).SingleOrDefault();
            //                    if (stripe != null)
            //                    {
            //                        mdays.First(x => x.Day==tmpDt).Remove(stripe);
            //                        mdays.First(x => x.Day==tmpDt).Updated = DateTime.Now;
            //                    }
            //                    if (st2 != null && end2!=null)
            //                    {
            //                        mdays.First(x => x.Day==tmpDt).Add(new ShiftTwo((TimeSpan)st2, (TimeSpan)end2,this.comment2.Text));
            //                        mdays.First(x => x.Day==tmpDt).Updated = DateTime.Now;

            //                    }
            //                }
            //                if (chk_edit3.IsChecked.Equals(true))
            //                {
                                
            //                    Stripe stripe = mdays.First(x => x.Day==tmpDt).Where(x => x.Type == 3 && x.Start==ShiftThree.MinMinute).SingleOrDefault();
            //                    if (stripe != null)
            //                    {
            //                        mdays.First(x => x.Day==tmpDt).Remove(stripe);
            //                        mdays.First(x => x.Day==tmpDt).Updated = DateTime.Now;
            //                    }
            //                    if(mdays.Any(x => x.Day==tmpDt.AddDays(-1)))
            //                    {
            //                    stripe = mdays.First(x => x.Day==tmpDt.AddDays(-1)).Where(x => x.Type == 3 && x.End==ShiftThree.MaxMinute).SingleOrDefault();
            //                    if (stripe != null)
            //                    {
            //                        mdays.First(x => x.Day==tmpDt.AddDays(-1)).Remove(stripe);
            //                        mdays.First(x => x.Day==tmpDt.AddDays(-1)).Updated = DateTime.Now;
            //                    }
            //                    }
            //                    if (st3 != null && end3 !=null)
            //                    {
            //                        if (tmpDt.DayOfWeek == DayOfWeek.Monday) st3=st3.Value.Subtract(TimeSpan.FromHours(1));
            //                        mdays.First(x => x.Day==tmpDt).Add(new ShiftThree(ShiftThree.MinMinute,(int)((TimeSpan)end3).TotalMinutes,this.comment3.Text));
            //                        if(!(mdays.Any(x => x.Day==tmpDt.AddDays(-1)))) mdays.Add(new DayLine(tmpDt.AddDays(-1)));
            //                        mdays.First(x => x.Day==tmpDt.AddDays(-1)).Add(new ShiftThree((int)((TimeSpan)st3).TotalMinutes,ShiftThree.MaxMinute,this.comment3.Text));
            //                        mdays.First(x => x.Day==tmpDt).Updated = DateTime.Now;
            //                        mdays.First(x => x.Day==tmpDt.AddDays(-1)).Updated = DateTime.Now;

            //                    }
            //                }
            //                DbManager.Instance().UpdateRessKappa(rid, mdays.First(x => x.Day==tmpDt));
            //                if (mdays.Any(x => x.Day==tmpDt.AddDays(-1)))
            //                {
            //                DbManager.Instance().UpdateRessKappa(rid, mdays.First(x => x.Day==tmpDt.AddDays(-1)));
            //                }
                            
            //            }
            //        }
            //        tmpDt = tmpDt.AddDays(1);
            //    }

            //Close();

        }
        private bool ValidForm(Grid grd)
        {
            foreach (Control c in grd.Children)
            {
                TextBox tex = c as TextBox;
                if (tex != null)
                {
                }
            }
            return false;
        }
    
    }
}
