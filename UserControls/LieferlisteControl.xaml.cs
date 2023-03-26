using Lieferliste_WPF.Dialogs;
using Lieferliste_WPF.ViewModels;
using Lieferliste_WPF.ViewModels.Base;
using Lieferliste_WPF.Utilities;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Lieferliste_WPF.Commands;

namespace Lieferliste_WPF.UserControls
{
    /// <summary>
    /// Interaction logic for LieferlisteView1.xaml
    /// </summary>
    public partial class LieferlisteControl : UserControl
    {

        GroupFilter gf = new GroupFilter();


        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(ELCommands));
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(ELCommands));
        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        public LieferlisteControl()
        {

            InitializeComponent();
            Dispatcher.ShutdownStarted += OnDispatcherShutDownStarted;

        }

        private void LieferlisteControl_Loaded(object sender, RoutedEventArgs e)
        {

            // Do not load your data at design time.
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                //Load your data here and assign the result to the CollectionViewSource.
                //System.Windows.Data.CollectionViewSource myCollectionViewSource = (System.Windows.Data.CollectionViewSource)this.Resources["Resource Key for CollectionViewSource"];
                //.Source = (LieferViewModel)this.DataContext;
             }

        }

 
        private void ChkAusVis_Click(object sender, RoutedEventArgs e)
        {


        }

        private void BtnMessOrder_Click(object sender, RoutedEventArgs e)
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

        }
        private void OnDispatcherShutDownStarted(object sender, EventArgs e)
        {
            var disposable = DataContext as IDisposable;
            if (disposable is not null)
            {
                disposable.Dispose();
            }
        }

        private void Modified(object sender, RoutedEventArgs e)
        {

        }
    }


}
