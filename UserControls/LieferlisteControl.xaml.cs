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
using System.Windows.Media.Animation;
using System.CodeDom;
using Lieferliste_WPF.View;
using System.IO;
using System.Windows.Documents;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Data;

namespace Lieferliste_WPF.UserControls
{
    /// <summary>
    /// Interaction logic for LieferlisteView1.xaml
    /// </summary>
    public partial class LieferlisteControl : UserControl
    {
        public LieferlisteControl()
        {
            InitializeComponent();
            //Dispatcher.ShutdownStarted += OnDispatcherShutDownStarted;

        }
  


        #region Properties
        
        public static readonly DependencyProperty AidProperty
            = DependencyProperty.Register("Aid"
                , typeof(string)
                , typeof(LieferlisteControl));
        public String Aid
        {
            get { return (string)GetValue(AidProperty); }
            set { SetValue(AidProperty, value); }
        }
        public static readonly DependencyProperty TtnrProperty
            = DependencyProperty.Register("TTNR"
                , typeof(string)
                , typeof(LieferlisteControl));
        public string TTNR
        {
            get { return (string)GetValue(TtnrProperty); }
            set { SetValue(TtnrProperty, value); }
        }
        public static readonly DependencyProperty MatTextProperty
            = DependencyProperty.Register("MatText"
                , typeof(string)
                , typeof(LieferlisteControl));
        public string MatText
        {
            get { return (string)GetValue(MatTextProperty); }
            set { SetValue(MatTextProperty, value); }
        }
        public static readonly DependencyProperty EndDateProperty
            = DependencyProperty.Register("EndDate"
                , typeof(DateTime)
                , typeof(LieferlisteControl));

        public DateTime EndDate
        {
            get { return (DateTime)GetValue(EndDateProperty); }
            set { SetValue(EndDateProperty, value); }
        }


        public DateTime? Termin
        {
            get { return (DateTime?)GetValue(TerminProperty); }
            set { SetValue(TerminProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Termin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TerminProperty =
            DependencyProperty.Register("Termin", typeof(DateTime?)
                , typeof(LieferlisteControl));


        public static readonly DependencyProperty VnrProperty
            = DependencyProperty.Register("Vnr"
                , typeof(int)
                , typeof(LieferlisteControl));
        public int Vnr
        {
            get { return (int)GetValue(VnrProperty); }
            set { SetValue(VnrProperty, value); }
        }
        public static readonly DependencyProperty VgTextProperty
            = DependencyProperty.Register("VgText"
                , typeof(string)
                , typeof(LieferlisteControl));
        public string VgText
        {
            get { return (string)GetValue(VgTextProperty); }
            set { SetValue(VgTextProperty, value); }
        }
        public static readonly DependencyProperty QuantityProperty
            = DependencyProperty.Register("Quantity"
                , typeof(int)
                , typeof(LieferlisteControl));
        public int Quantity
        {
            get { return (int)GetValue(QuantityProperty); }
            set { SetValue(QuantityProperty, value); }
        }
        public static readonly DependencyProperty QuantityScrapProperty
            = DependencyProperty.Register("QuantityScrap"
                , typeof(int)
                , typeof(LieferlisteControl));
        public int QuantityScrap
        {
            get { return (int)GetValue(QuantityScrapProperty); }
            set { SetValue(QuantityScrapProperty, value); }
        }
        public static readonly DependencyProperty QuantityYieldProperty
            = DependencyProperty.Register("QuantityYield"
                , typeof(int)
                , typeof(LieferlisteControl));
        public int QuantityYield
        {
            get { return (int)GetValue(QuantityYieldProperty); }
            set { SetValue(QuantityYieldProperty, value); }
        }
        public static readonly DependencyProperty QuantityReworkProperty
            = DependencyProperty.Register("QuantityRework"
                , typeof(int)
                , typeof(LieferlisteControl));
        public int QuantityRework
        {
            get { return (int)GetValue(QuantityReworkProperty); }
            set { SetValue(QuantityReworkProperty, value); }
        }


        public string WorkArea
        {
            get { return (string)GetValue(WorkAreaProperty); }
            set { SetValue(WorkAreaProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WorkArea.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WorkAreaProperty =
            DependencyProperty.Register("WorkArea", typeof(string), typeof(LieferlisteControl), new PropertyMetadata(""));


        public string WorkAreaText
        {
            get { return (string)GetValue(WorkAreaTextProperty); }
            set { SetValue(WorkAreaTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WorkAreaText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WorkAreaTextProperty =
            DependencyProperty.Register("WorkAreaText", typeof(string), typeof(LieferlisteControl), new PropertyMetadata("-"));



        public string Comment_Me
        {
            get { return (string)GetValue(Comment_MeProperty); }
            set { SetValue(Comment_MeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Comment_Me.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty Comment_MeProperty =
            DependencyProperty.Register("Comment_Me", typeof(string),
                typeof(LieferlisteControl),
                new PropertyMetadata(""));




        public String Comment_Te
        {
            get { return (String)GetValue(Comment_TeProperty); }
            set { SetValue(Comment_TeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Comment_Te.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty Comment_TeProperty =
            DependencyProperty.Register("Comment_Te", typeof(String),
                typeof(LieferlisteControl), new PropertyMetadata("",
                    new PropertyChangedCallback(OnComment_TeChangend)));

        private static void OnComment_TeChangend(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
        }

        public string Comment_Ma
        {
            get { return (string)GetValue(Comment_MaProperty); }
            set { SetValue(Comment_MaProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Comment_Ma.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty Comment_MaProperty =
            DependencyProperty.Register("Comment_Ma", typeof(string),
                typeof(LieferlisteControl),
                new PropertyMetadata(""));




        public String HasMouseOver
        {
            get { return (String)GetValue(HasMouseOverProperty); }
            set { SetValue(HasMouseOverProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HasMouseOver.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HasMouseOverProperty =
            DependencyProperty.Register("HasMouseOver", typeof(String), typeof(LieferlisteControl), new PropertyMetadata(""));



        public bool Invisible
        {
            get { return (bool)GetValue(InvisibleProperty); }
            set { SetValue(InvisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Invisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InvisibleProperty =
            DependencyProperty.Register("Invisible", typeof(bool), typeof(LieferlisteControl), new PropertyMetadata(false));


        public bool Doku
        {
            get { return (bool)GetValue(DokuProperty); }
            set { SetValue(DokuProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Doku.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DokuProperty =
            DependencyProperty.Register("Doku", typeof(bool), typeof(LieferlisteControl), new PropertyMetadata(false));



        public bool Archivated
        {
            get { return (bool)GetValue(ArchivatedProperty); }
            set { SetValue(ArchivatedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Archited.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ArchivatedProperty =
            DependencyProperty.Register("Archivated", typeof(bool), typeof(LieferlisteControl), new PropertyMetadata(false));

            
        public string SelectedValue
        {
            get { return (string)GetValue(SelectedValueProperty); }
            set { SetValue(SelectedValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedValueProperty =
            DependencyProperty.Register("SelectedValue", typeof(string), typeof(LieferlisteControl), new PropertyMetadata(""));


        #endregion



        void HandleLieferlisteControlLoaded(object sender, RoutedEventArgs e)
        {
            CommandBindings.Add(new CommandBinding(
                ELCommands.PresentText, HandlePresentTextExecuted,
                HandlePresentTextCanExecute));
            
        }

        private void HandlePresentTextCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {

            e.CanExecute = false;
        }

        private void HandlePresentTextExecuted(object sender, ExecutedRoutedEventArgs e)
        {
   
            RichTextEditor rtb = new RichTextEditor();
            TextBox txt = (TextBox)e.OriginalSource;
            MemoryStream memoryStream = new MemoryStream();
            //memoryStream.AsInputStream();
            memoryStream.Position = 0;
            
            rtb.DataContext = txt.Text;
            rtb.Show();
            
        }


        private void BtnMessOrder_Click(object sender, RoutedEventArgs e)
        {
            var lst = e.Source as FrameworkElement;

            var d = lst.DataContext as DataRowView;
            MessauftragDialog dialog = new((string)d.Row.ItemArray[10])
            {
                Owner = Application.Current.Windows[0],
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            dialog.ShowDialog();
        }


        private void Modified(object sender, RoutedEventArgs e)
        {

        }


        private void TextBlock_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is TextBlock send)
            {

                Storyboard s = (Storyboard)TryFindResource("EnterStoryBoard");
                Storyboard.SetTarget(s, send);
                s.Begin();
            }

        }
        private void TextBlock_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is TextBlock send)
            {
                Storyboard s = (Storyboard)TryFindResource("ExitStoryBoard");
                Storyboard.SetTarget(s, send);
                s.Begin();
            }

        }
        private void TextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if(sender is TextBlock send)
            {
                if(send.IsMouseCaptured)
                    send.ReleaseMouseCapture();
            }
        }
        private void TextBlock_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if( sender is TextBlock send)
            {
                SetValue(HasMouseOverProperty, send.Name);
                send.CaptureMouse();
            }
        }

        private void NotTextBlock_MouseEnter(object sender, MouseEventArgs e)
        {
            SetValue(HasMouseOverProperty, String.Empty);
        }

        private void btnExpl_Click(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.FileName = "Document"; // Default file name
            dialog.DefaultExt = ".txt"; // Default file extension
            dialog.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension

            // Show open file dialog box
            bool? result = dialog.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                string filename = dialog.FileName;
            }          
        }

        private void btnToArchiv_Click(object sender, RoutedEventArgs e)
        {
            SetValue(ArchivatedProperty, true);
        }



        private void txtCommetMa_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void txtCommetM_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void chkInvis_Checked(object sender, RoutedEventArgs e)
        {

        }
    }


}
