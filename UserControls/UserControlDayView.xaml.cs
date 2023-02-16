using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Lieferliste_WPF.Dialogs;
using Lieferliste_WPF.Working;
using Lieferliste_WPF.Entities;
using Lieferliste_WPF.Planning;
using System.ComponentModel;

namespace Lieferliste_WPF.UserControls
{
    /// <summary>
    /// Interaction logic for DayView.xaml
    /// </summary>



    public partial class DayView : UserControl
    {
        public delegate void DayViewEventHandler(object sender, DayViewEventArgs args);
        public static readonly DependencyProperty DateProperty = DependencyProperty.Register("DateDT", typeof(DateTime), typeof(DayView), new FrameworkPropertyMetadata(DateTime.Now)
            {
                BindsTwoWayByDefault = true,
                DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            });
        public static readonly DependencyProperty ActionProperty = DependencyProperty.Register("Actions", typeof(LinkedList<Process>), typeof(DayView), new FrameworkPropertyMetadata(new LinkedList<Process>())
        {
            BindsTwoWayByDefault = true,
            DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
        });
        public LinkedList<Process> Actions
        {
            get { return (LinkedList<Process>)this.GetValue(ActionProperty); }
            set { this.SetValue(ActionProperty, value); }
        }

        public DateTime DateDT
        {
            get { return (DateTime)this.GetValue(DateProperty); }
            set { this.SetValue(DateProperty, value); }
        }
        // Constructor
        public DayView()
        {
            InitializeComponent();
            //base.DataContext = this;

        }

        void DayFilter(object sender, FilterEventArgs e)
        {
            var t = sender;
        }

        private void ListBox1_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("ListBox_DoubleClick");

        }


        private void Rectangle_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (ListBox1.Items.Count > 0)
                {
                    if (ListBox1.SelectedItem != null)
                    {
                        // Package th Data.
                        DataObject data = new DataObject();
                        data.SetData("ActionStripe", ListBox1.SelectedItem);

                        DragDrop.DoDragDrop(this, data, DragDropEffects.Move);
                    }
                }
            }
        }

        private void Rectangle_DragOver(object sender, DragEventArgs e)
        {

        }

        private void ListBox1_Drop(object sender, DragEventArgs e)
        {
            var dragged = e.Data.GetData("ActionStripe") as ActionStripe;
            var target = sender as ListBox;

            int index = this.GetCurrentIndex(target, e.GetPosition);
            if (index >= 0)
            {
                var t = target.Items[index] as ActionStripe;
                if (t != null && dragged != null)
                { 
                    
                    //this.PlannerControl.MoveProcess(t,dragged);

                }
            }
        }

        int GetCurrentIndex(ListBox listBox, Func<IInputElement, Point> getPosition)
        {
            for (int index = 0; index < listBox.Items.Count; index++)
            {
                var item = listBox.ItemContainerGenerator.ContainerFromIndex(index) as ListBoxItem;
                if (item != null && this.IsMouseOverTarget(item, getPosition))
                {
                    return index;
                }
            }
            return -1;
        }

        bool IsMouseOverTarget(Visual target, Func<IInputElement, Point> getPosition)
        {
            Rect bounds = VisualTreeHelper.GetDescendantBounds(target);
            Point position = getPosition((IInputElement)target);
            return bounds.Contains(position);
        }



        private void ListBox2_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SchDialog dialog = new SchDialog(this);

            dialog.ShowDialog();
        }

    }


    public class DayViewEventArgs : EventArgs
    {
        public DayViewEventArgs(bool result,
                            int start)
        {

        }
    }
}
