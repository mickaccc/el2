using Lieferliste_WPF.Dialogs;
using Lieferliste_WPF.Entities;
using Lieferliste_WPF.ViewModels;
using Lieferliste_WPF.ViewModels.Base;
using Lieferliste_WPF.Utilities;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace Lieferliste_WPF.UserControls
{
    /// <summary>
    /// Interaction logic for LieferlisteView1.xaml
    /// </summary>
    public partial class LieferlisteView1 : UserControl
    {
        GroupFilter gf = new GroupFilter();
        ProcessVM _selectedProcess = null;

        //    public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(eLCommands));
        //public ICommand Command
        //{
        //    get { return (ICommand) GetValue(CommandProperty); }
        //    set{SetValue(CommandProperty, value);}
        //}

        //public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(eLCommands));
        //public object CommandParameter
        //{
        //    get { return (object) GetValue(CommandParameterProperty); }
        //    set { SetValue(CommandParameterProperty, value); }
        //}

        public LieferlisteView1()
        {

            InitializeComponent();
            Dispatcher.ShutdownStarted += OnDispatcherShutDownStarted;

        }

        //private void LieferlisteView1_Loaded(object sender, RoutedEventArgs e)
        //{

        //    // Do not load your data at design time.
        //    // if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
        //    // {
        //    // 	//Load your data here and assign the result to the CollectionViewSource.
        //    // 	System.Windows.Data.CollectionViewSource myCollectionViewSource = (System.Windows.Data.CollectionViewSource)this.Resources["Resource Key for CollectionViewSource"];
        //    // 	myCollectionViewSource.Source = your data
        //    // }

        //}

        /// <summary>
        /// Convenient accessor for the view-model.
        /// </summary>
        private DeliveryListViewModel ViewModel
        {
            get
            {
                return (DeliveryListViewModel)this.DataContext;
            }
        }
        private void chkAusVis_Click(object sender, RoutedEventArgs e)
        {


        }

        private void btnMessOrder_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement lst = e.Source as FrameworkElement;

            DataRowView d = lst.DataContext as DataRowView;
            MessauftragDialog dialog = new MessauftragDialog((String)d.Row.ItemArray[10]);
            dialog.Owner = App.Current.Windows[0];
            dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dialog.ShowDialog();
        }



        private void listBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var sel = (e.Source as ListView).SelectedItem;
            lieferliste pro = null;
            if (sel != null)
                pro = (sel as ProcessVM).TheProcess as lieferliste;

            if (pro != null && !_selectedProcess.TheProcess.AID.Equals(pro.AID))
            {

                _selectedProcess = sel as ProcessVM;
                OrderViewModel.This.ReLoad(pro.AID);

            }
        }
        private void OnDispatcherShutDownStarted(object sender, EventArgs e)
        {
            var disposable = DataContext as IDisposable;
            if (!ReferenceEquals(null, disposable))
            {
                disposable.Dispose();
            }
        }

        private void modified(object sender, RoutedEventArgs e)
        {
            _selectedProcess.IsModified = true;
        }
    }


}
