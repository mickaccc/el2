﻿
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.CodeDom;
using System.IO;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using CompositeCommands.Core;

namespace ModuleDeliverList.UserControls
{
    /// <summary>
    /// Interaction logic for LieferlisteView1.xaml
    /// </summary>
    public partial class LieferlisteControl : UserControl
    {
        public LieferlisteControl()
        {
            InitializeComponent();
        }

        #region Properties

        public static readonly DependencyProperty AidProperty
            = DependencyProperty.Register("Aid"
                , typeof(string)
                , typeof(LieferlisteControl),
                new PropertyMetadata("",OnPropertyChanged));
        public String Aid
        {
            get { return (string)GetValue(AidProperty); }
            set { SetValue(AidProperty, value); }
        }
        public static readonly DependencyProperty TtnrProperty
            = DependencyProperty.Register("TTNR"
                , typeof(string)
                , typeof(LieferlisteControl),
                new PropertyMetadata("",OnPropertyChanged));
        public string TTNR
        {
            get { return (string)GetValue(TtnrProperty); }
            set { SetValue(TtnrProperty, value); }
        }
        public static readonly DependencyProperty MatTextProperty
            = DependencyProperty.Register("MatText"
                , typeof(string)
                , typeof(LieferlisteControl),
                new PropertyMetadata("",OnPropertyChanged));
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



        public DateTime EckEnd
        {
            get { return (DateTime)GetValue(EckEndProperty); }
            set { SetValue(EckEndProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EckEnd.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EckEndProperty =
            DependencyProperty.Register("EckEnd", typeof(DateTime), typeof(LieferlisteControl));


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
                , typeof(LieferlisteControl),
                new PropertyMetadata(0,OnPropertyChanged));
        public int Vnr
        {
            get { return (int)GetValue(VnrProperty); }
            set { SetValue(VnrProperty, value); }
        }
        public static readonly DependencyProperty VgTextProperty
            = DependencyProperty.Register("VgText"
                , typeof(string)
                , typeof(LieferlisteControl),
                new PropertyMetadata("",OnPropertyChanged));
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
            DependencyProperty.Register("WorkArea", typeof(string), typeof(LieferlisteControl),
                new PropertyMetadata("",OnPropertyChanged));



        public string WorkAreaText
        {
            get { return (string)GetValue(WorkAreaTextProperty); }
            set { SetValue(WorkAreaTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WorkAreaText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WorkAreaTextProperty =
            DependencyProperty.Register("WorkAreaText", typeof(string), typeof(LieferlisteControl), new PropertyMetadata("-"));



        public string MachineText
        {
            get { return (string)GetValue(MachineTextProperty); }
            set { SetValue(MachineTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MachineText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MachineTextProperty =
            DependencyProperty.Register("MachineText", typeof(string), typeof(LieferlisteControl), new PropertyMetadata(""));



        public string Comment_MeI
        {
            get { return (string)GetValue(Comment_MeIProperty); }
            set { SetValue(Comment_MeIProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Comment_MeI.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty Comment_MeIProperty =
            DependencyProperty.Register("Comment_MeI", typeof(string), typeof(LieferlisteControl), new PropertyMetadata(""));



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



        public bool Comment_MeAllow
        {
            get { return (bool)GetValue(Comment_MeAllowProperty); }
            set { SetValue(Comment_MeAllowProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Comment_MeAllow.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty Comment_MeAllowProperty =
            DependencyProperty.Register("Comment_MeAllow", typeof(bool),
                typeof(LieferlisteControl), new PropertyMetadata(false));




        public string Comment_TeI
        {
            get { return (string)GetValue(Comment_TeIProperty); }
            set { SetValue(Comment_TeIProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Comment_TeI.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty Comment_TeIProperty =
            DependencyProperty.Register("Comment_TeI", typeof(string), typeof(LieferlisteControl), new PropertyMetadata(""));



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



        public bool Commént_TeAllow
        {
            get { return (bool)GetValue(Commént_TeAllowProperty); }
            set { SetValue(Commént_TeAllowProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Commént_TeAllow.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty Commént_TeAllowProperty =
            DependencyProperty.Register("Commént_TeAllow", typeof(bool),
                typeof(LieferlisteControl), new PropertyMetadata(false));


        private static void OnComment_TeChangend(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
        }


        public string Comment_MaI
        {
            get { return (string)GetValue(Comment_MaIProperty); }
            set { SetValue(Comment_MaIProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Comment_MaI.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty Comment_MaIProperty =
            DependencyProperty.Register("Comment_MaI", typeof(string),
                typeof(LieferlisteControl), new PropertyMetadata(""));


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



        public bool Comment_MaAllow
        {
            get { return (bool)GetValue(Comment_MaAllowProperty); }
            set { SetValue(Comment_MaAllowProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Comment_MaAllow.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty Comment_MaAllowProperty =
            DependencyProperty.Register("Comment_MaAllow", typeof(bool),
                typeof(LieferlisteControl), new PropertyMetadata(false));


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

        public static readonly DependencyProperty ExplorerCommandProperty = DependencyProperty.Register("ExplorerCommand", typeof(ICommand), typeof(LieferlisteControl));
 
        public Dictionary<string,object> AvailableItems
        {
            get { return (Dictionary<string,object>)GetValue(AvailableItemsProperty); }
            set { SetValue(AvailableItemsProperty, value); }
        }

        public string HasMouseOver { get; internal set; }

        // Using a DependencyProperty as the backing store for AvailableItems.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AvailableItemsProperty =
            DependencyProperty.Register("AvailableItems", typeof(Dictionary<string,object>), typeof(LieferlisteControl));

        #endregion

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var newValue = e.NewValue;
            object? val;
            LieferlisteControl? lc = d as LieferlisteControl;
            if (lc != null)
            { 
                if(lc.AvailableItems == null) lc.AvailableItems = new Dictionary<string,object>();
                if (lc.AvailableItems.TryGetValue(e.Property.Name.ToLower(), out val))
                {
                    lc.AvailableItems[e.Property.Name.ToLower()] = newValue;
                }
                else
                {
                    lc.AvailableItems.Add(e.Property.Name.ToLower(), newValue);
                }
            }
            
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

        private void btnToArchiv_Click(object sender, RoutedEventArgs e)
        {
            SetValue(ArchivatedProperty, true);
        }




        private void chkInvis_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void CommentHighPrio_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }


}
