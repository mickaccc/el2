using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Lieferliste_WPF.UserControls
{
    /// <summary>
    /// Interaction logic for Allocation.xaml
    /// </summary>

    public partial class AllocationWorkingList : UserControl
    {

        public int Criteria { get; set; }

        public AllocationWorkingList(int Crit)
        {
            Criteria = Crit;
            InitializeComponent();


            ressOrdered.Criteria = Crit;
            ressOrdered.Height = this.Height - 80;
        }




        private void ressOrder_DoubleClick(object sender, RoutedEventArgs e)
        {

                Visual frWorkEle = e.OriginalSource as Visual;

                UIElement UI = null;
                int RID = 0;
                StackPanel sta = null;
                switch (frWorkEle.GetType().Name)
                {
                    case "StackPanel":
                        sta = frWorkEle as StackPanel;
                        RID = (int)sta.Tag;
                        UI = ressOrdered.nodePlannerControls[RID];
                        break;
                    case "TextBlock":
                        TextBlock txt = frWorkEle as TextBlock;
                        if (txt.Parent != null)
                        {
                            UI = new OrderView(txt.Text);
                        }
                        else
                        {
                            while (VisualTreeHelper.GetParent(frWorkEle).GetType() != typeof(MainWindow))
                            {
                                frWorkEle = VisualTreeHelper.GetParent(frWorkEle) as Visual;

                                if (frWorkEle != null)
                                {
                                    if (frWorkEle.GetType() == typeof(StackPanel))
                                    {
                                        sta = frWorkEle as StackPanel;
                                        break;
                                    }
                                }
                            }

                            if (sta != null)
                            {
                                RID = (int)sta.Tag;
                                UI = ressOrdered.nodePlannerControls[RID];
                            }
                        }

                        break;
                    default:
                        while (VisualTreeHelper.GetParent(frWorkEle).GetType() != typeof(MainWindow))
                        {
                            frWorkEle = VisualTreeHelper.GetParent(frWorkEle) as Visual;

                            if (frWorkEle != null)
                            {
                                if (frWorkEle.GetType() == typeof(StackPanel))
                                {
                                    sta = frWorkEle as StackPanel;
                                    break;
                                }
                            }
                        }

                        if (sta != null)
                        {
                            RID = (int)sta.Tag;
                            UI = ressOrdered.nodePlannerControls[RID];
                        }
                        break;

                }
                if (UI != null)
                {



                }

            
        }
    }
}
