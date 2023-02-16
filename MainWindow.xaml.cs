using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using Lieferliste_WPF.Commands;
using Lieferliste_WPF.UserControls;
using FastMember;
using log4net;
using Lieferliste_WPF.Converters;
using Lieferliste_WPF.ViewModels;
using System.Threading;
using System.Globalization;
using Microsoft.Win32;
using System.Reflection;


namespace Lieferliste_WPF
{
    /// <summary>
    /// View class for the main window.
    /// The IDialogProvider interface allows the main window to provide 'file dialog' services
    /// to its view-model.
    /// </summary>
    public partial class MainWindow : Window,IDialogProvider
    {
        private SortableObservableCollection<TabItem> _tabItems;
        private TabItem _tabAdd;
        /// <summary>
        /// Name of the file used to save/restore AvalonDock layout.
        /// </summary>
        private static readonly string LayoutFileName = "MyLayoutFile.xml";

        /// <summary>
        /// Name of the embedded resource that contains the default AvalonDock layout.
        /// </summary>
        private static readonly string DefaultLayoutResourceName = "Lieferliste_WPF.Resources.DefaultLayoutFile.xml";
        static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static ToolBar FilterBar { get; set; }
        public static ToolBar SearchBar { get; set; }
        public static ToolBar SelectorBar { get; set; }
        public static ToolBar FormatBar { get; set; }
        public static MainWindow MainWindowRef { get; set; }
        public static string currentUser { get { return Environment.UserName; } }
        public ICommand SortCommand { get; internal set; }
        public static List<object> ViewList { get; set; }
        List<String> Proj { get; set; }
        Dictionary<int, String> Spezial { get; set; }
        GroupFilter gf = new GroupFilter();

        public partial class App : Application
        {

        }
        #region Constructor
        public MainWindow()
        {
            //AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            //{
            //    var exception = (Exception)e.ExceptionObject;
            //    Log.Error(String.Format("AppVersion {0} unhandled Exception: {1}", AssemblyInfo.Version, exception));
            //};

            
            try
            {

                if (!PermissionsManager.getInstance(currentUser).getUserPermission("app0010"))
                {
                    MessageBox.Show("Sie haben keine Berechtigung zum starten dieser Anwendung!", "eL²4-COS", MessageBoxButton.OK, MessageBoxImage.Stop);
                    Application.Current.Shutdown();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Fehler beim starten 'MainWindow'\n" + e.Message + "\n" + e.StackTrace);
            }

            InitializeComponent();
            
                       //
            // Create the main window's view-model and assign to the DataContext.
            //
            this.DataContext = new MainWindowViewModel(this);

            // initialize tabItem array
            _tabItems = new SortableObservableCollection<TabItem>();

            // add a tabItem with + in header 
            //_tabAdd = new TabItem();
            //_tabAdd.Header = "+";

            //AddTabItem(new MachineActionView());


            //MainTab.DataContext = _tabItems;

            //MainTab.SelectedIndex = 0;

            SearchBar = this.ToolBar_search;
            FilterBar = this.ToolBar_filter;
            SelectorBar = this.Toolbar_selector;
            FormatBar = this.ToolBar_format;
            MainWindowRef = this;
            ViewList = new List<object>();

        }
        #endregion Constructor

        #region Events

        private void Resource_Division_Click(object sender, RoutedEventArgs e)
        {
            //if (!MainTab.Items.Contains(typeof(Allocation)))
            //{
            //    Allocation Alloc = new Allocation((int)this.cmbUnion.SelectedValue);
            //    ViewList.Add(Alloc);
            //    AddTabItem(Alloc);
            //}


            //this.ToolBar_search.Visibility = Visibility.Visible;
            //this.ToolBar_filter.Visibility = Visibility.Collapsed;
            //this.ToolBar_format.Visibility = Visibility.Collapsed;
            //this.Toolbar_selector.Visibility = Visibility.Visible;
        }
        private void Show_Lieferlist_Click(object sender, RoutedEventArgs e)
        {
            //if (_LLViewModel == null)
            //{
            //    LieferlisteView1 lflst = new LieferlisteView1();
            //    _LLViewModel = (LLViewModel)lflst.Liefer.DataContext;
            //    AddTabItem(lflst);

            //}
            //this.ToolBar_search.Visibility = Visibility.Visible;
            //this.ToolBar_filter.Visibility = Visibility.Visible;
            //this.ToolBar_format.Visibility = Visibility.Collapsed;
            //this.Toolbar_selector.Visibility = Visibility.Collapsed;
        }


        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {

            //if (MainTab.HasItems)
            //{

            //    switch (MainTab.SelectedContent.GetType().Name)
            //    {
            //        case "LieferlisteView1":
            //            LieferlisteView1 lflst = (LieferlisteView1)MainTab.SelectedContent;
            //            if (lflst != null)
            //            {
            //                switch (cmbFields.Text.ToLower())
            //                {

            //                    case "alle":
            //                        _LLViewModel.addFilterCriteria("OrderNumber", ".*" + this.txtSearch.Text + ".*");
            //                        _LLViewModel.addFilterCriteria("Material", ".*" + this.txtSearch.Text + ".*");
            //                        _LLViewModel.addFilterCriteria("MaterialDescription", ".*" + this.txtSearch.Text + ".*");
            //                        _LLViewModel.refresh();
            //                        break;
            //                    case "ttnr":
            //                        _LLViewModel.removeFilterCriteria("MaterialDescription");
            //                        _LLViewModel.removeFilterCriteria("OrderNumber");
            //                        _LLViewModel.addFilterCriteria("Material", ".*" + this.txtSearch.Text + ".*");
            //                        _LLViewModel.refresh();
            //                        break;
            //                    case "teil":
            //                        _LLViewModel.removeFilterCriteria("Material");
            //                        _LLViewModel.removeFilterCriteria("OrderNumber");
            //                        _LLViewModel.addFilterCriteria("MaterialDescription", ".*" + this.txtSearch.Text + ".*");
            //                        _LLViewModel.refresh();
            //                        break;
            //                    case "auftrag":
            //                        _LLViewModel.removeFilterCriteria("MaterialDescription");
            //                        _LLViewModel.removeFilterCriteria("Material");
            //                        _LLViewModel.addFilterCriteria("OrderNumber", ".*" + this.txtSearch.Text + ".*");
            //                        _LLViewModel.refresh();
            //                        break;
            //                }
            //            }
            //            break;
            //        case "Allocation":
            //            Allocation Alloc = (Allocation)MainTab.SelectedContent;

            //            CollectionViewSource.GetDefaultView(Alloc.lstMain.ItemsSource).Refresh();

            //            break;
            //        case "AllocationWorkingList":
            //            break;
            //    }

            //}
        }


        private void cmbFilters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            //if (MainTab.SelectedContent.GetType() == typeof(LieferlisteView1))
            //{
            //    if (cmbKst.SelectedValue != null)
            //    {
            //        _LLViewModel.addFilterCriteria("WorkSpace", (String)cmbKst.SelectedValue);
            //        _LLViewModel.refresh();
            //    }
            //    else
            //    {
            //        _LLViewModel.removeFilterCriteria("WorkSpace");
            //        _LLViewModel.refresh();
            //    }

            //    if (cmbSpezial.SelectedIndex == -1)
            //    {
            //        _LLViewModel.removeFilterCriteria("isReady");
            //        _LLViewModel.removeFilterCriteria("isPortfolioAvail");
            //        _LLViewModel.removeFilterCriteria("isHighPrio");
            //        _LLViewModel.refresh();
            //    }

            //    switch (cmbSpezial.SelectedIndex)
            //    {
            //        case 0:
            //            _LLViewModel.removeFilterCriteria("isReady");
            //            _LLViewModel.removeFilterCriteria("isPortfolioAvail");
            //            _LLViewModel.removeFilterCriteria("isHighPrio");
            //            _LLViewModel.addFilterCriteria("ExecutionShortText", ".*AUFTRAG STARTEN.*");
            //            _LLViewModel.refresh();
            //            break;
            //        case 1:
            //            _LLViewModel.removeFilterCriteria("ExecutionShortText");
            //            _LLViewModel.removeFilterCriteria("isPortfolioAvail");
            //            _LLViewModel.removeFilterCriteria("isHighPrio");
            //            _LLViewModel.addFilterCriteria("isReady", "true");
            //            _LLViewModel.refresh();
            //            break;
            //        case 2:
            //            _LLViewModel.removeFilterCriteria("ExecutionShortText");
            //            _LLViewModel.removeFilterCriteria("isReady");
            //            _LLViewModel.removeFilterCriteria("isHighPrio");
            //            _LLViewModel.addFilterCriteria("isPortfolioAvail", "true");
            //            _LLViewModel.refresh();
            //            break;
            //        case 3:
            //            _LLViewModel.removeFilterCriteria("ExecutionShortText");
            //            _LLViewModel.removeFilterCriteria("isReady");
            //            _LLViewModel.removeFilterCriteria("isHighPrio");
            //            _LLViewModel.addFilterCriteria("isPortfolioAvail", "false");
            //            _LLViewModel.refresh();
            //            break;
            //        case 4:
            //            _LLViewModel.removeFilterCriteria("ExecutionShortText");
            //            _LLViewModel.removeFilterCriteria("isReady");
            //            _LLViewModel.removeFilterCriteria("isPortfolioAvail");
            //            _LLViewModel.addFilterCriteria("isHighPrio", "true");
            //            _LLViewModel.refresh();
            //            break;
            //        case 5:
            //            _LLViewModel.removeFilterCriteria("ExecutionShortText");
            //            _LLViewModel.removeFilterCriteria("isReady");
            //            _LLViewModel.removeFilterCriteria("isPortfolioAvail");
            //            _LLViewModel.addFilterCriteria("isHighPrio", "false");
            //            _LLViewModel.refresh();
            //            break;
            //    }

            //}

        }
        private void cmbUnion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectorBar.Visibility == Visibility.Visible)
            {
                ////if (!ViewList.Any(x => x.GetType().Name == "Allocation"))
                ////{
                ////    return;
                ////}
                ////else
                ////{

                ////    Allocation Alloc = new Allocation((int)cmbUnion.SelectedValue);

                ////    AddTabItem(Alloc);
                ////}
            }
        }
        private bool buildFilter(ref String filter, int type)
        {
            filter = String.Empty;
            switch (type)
            {
                case 1:
                    if (cmbProj.SelectedValue != null)
                    {
                        filter = "Projekt= '" + cmbProj.Text + "'";
                        return true;
                    }
                    return false;
                case 2:
                    if (cmbKst.SelectedValue != null)
                    {
                        filter = "ArbBID=" + cmbKst.SelectedValue;
                        return true;
                    }
                    return false;
                case 3:
                    switch (cmbSpezial.SelectedIndex)
                    {
                        case 0:
                            filter = "Text LIKE '%AUFTRAG STARTEN%'";
                            return true;
                        case 1:
                            filter = "fertig=1";
                            return true;
                        case 2:
                            filter = "Mappe=1";
                            return true;
                        case 3:
                            filter = "Mappe=0";
                            return true;
                        case 4:
                            filter = "Dringend=1";
                            return true;
                        case 5:
                            filter = "Dringend=0";
                            return true;
                    }
                    return false;
            }
            return false;
        }

        private void image1_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //.MainTab.Items.Add(new Home());
            this.ToolBar_search.Visibility = Visibility.Hidden;
            this.ToolBar_filter.Visibility = Visibility.Hidden;
            this.ToolBar_format.Visibility = Visibility.Collapsed;
            this.Toolbar_selector.Visibility = Visibility.Collapsed;
        }

        private void cmbFontFamily_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cmbFontSize_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            WPFAboutBox about = new WPFAboutBox(this);
            about.ShowDialog();
        }

        private void cmbFontSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btnClearProj_Click(object sender, RoutedEventArgs e)
        {
            cmbProj.Text = null;
        }

        private void btnClearSpezial_Click(object sender, RoutedEventArgs e)
        {
            cmbSpezial.Text = null;
        }
        private void btnClearKst_Click(object sender, RoutedEventArgs e)
        {
            cmbKst.Text = null;
        }
        #endregion Events

        #region Window-Events
        private void MainWindow_loaded(object sender, RoutedEventArgs e)
        {
            Proj = DbManager.Instance().getProjects().Rows.OfType<DataRow>()
                 .Select(dr => dr.Field<String>("Projekt")).ToList();


            var UnionList = from u in DbManager.Instance().getBereiche().AsEnumerable()
                            where u.BID > 0
                            select (u);

            cmbKst.DataContext = UnionList;
            cmbUnion.DataContext = UnionList.Where(x => x.Abteilung.Equals("COS"));


            this.cmbProj.ItemsSource = Proj;
            this.cmbProj.SelectionChanged += cmbFilters_SelectionChanged;

            CommandBindings.Add(new CommandBinding(ApplicationCommands.Print, HandleListPrintExecuted, HandleListPrintCanExecute));

            CommandBindings.Add(new CommandBinding(eLCommands.ToArchive, HandleArchiveExecuted, HandleArchiveCanExecute));
            CommandBindings.Add(new CommandBinding(eLCommands.ListSortAscending, HandleListSortAscExecuted, HandleListSortAscCanExecute));
            CommandBindings.Add(new CommandBinding(eLCommands.ListSortDescending, HandleListSortDescExecuted, HandleListSortDescCanExecute));
            CommandBindings.Add(new CommandBinding(eLCommands.ShowOrderView, HandleShowOrderViewExecuted, HandleShowOrderViewCanExecute));
        }
        #endregion Window-Events

        #region EventHandler der CommandBindings
        private void HandleListPrintExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            DocumentPreview doc = new DocumentPreview();
            //if (MainTab.SelectedContent.GetType() == typeof(Allocation))
            //{
            //    Allocation all = (Allocation)MainTab.SelectedContent;
            //    DataTable table = new DataTable();
            //    foreach (IMachine machine in all.ressOrdered.MachineContainer.Where(x => x.isSelected))
            //    {
            //        InternalMachine mach = machine as InternalMachine;
            //        using (var reader = ObjectReader.Create(mach.ProcessesLine, "OrderNumber", "ExecutionNumber", "Material", "MaterialDescription", "CommentTL"))
            //        {
            //            table.Load(reader);
            //        }
            //        table.TableName = "RessZuteil";
            //        doc.addTable(table);
            //        doc.ReportTitle = mach.MachineName;
            //        mach.isSelected = false;
            //    }
            //}
            doc.Show();
            //var dlg = new PrintDialog();
            //if (dlg.ShowDialog() == true)
            //{
            //    Size printArea = new Size(dlg.PrintableAreaWidth, dlg.PrintableAreaHeight);
            //    Thickness pageMargin = new Thickness(15, 20, 15, 60);
            //    //FixedDocument doc = GetListAsFixedDocument(printArea, pageMargin);

            //    //dlg.PrintDocument(doc.DocumentPaginator, "FriendStorage");

            //}
        }

        private FixedDocument GetListAsFixedDocument(Size printArea, Thickness pageMargin)
        {
            throw new NotImplementedException();
        }

        private void HandleListPrintCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            bool can = false;
            //if (MainTab.SelectedItem != null)
            //{
            //    if (MainTab.SelectedContent.GetType() == typeof(Allocation))
            //    {
            //        can = (MainTab.SelectedContent as Allocation).ressOrdered.MachineContainer.Where(x => x.isSelected).Count() == 1;
            //    }
            //}
            e.CanExecute = can;
        }

        private void HandleArchiveExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            String aid = (e.Parameter as String).Trim();

            if (aid != null)
            {
                if (DbManager.Instance().UpdateOrder(aid, "abgeschlossen", true) == true)
                {

                    //if (_LLViewModel != null)
                    //{
                    //    _LLViewModel.Processes.Where(x => x.OrderNumber == aid).ToList().ForEach(p => _LLViewModel.Processes.Remove(p));

                    //}
                }
            }
        }
        private void HandleArchiveCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = PermissionsManager.getInstance(currentUser).getUserPermission("lie0080abl");
        }

        private void HandleListSortAscExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            String field = String.Empty;

            switch (e.OriginalSource.GetType().Name)
            {
                case "TextBox":
                    field = BindingOperations.GetBinding((e.OriginalSource as TextBox), TextBox.TextProperty).Path.Path;
                    break;
                case "TextBlock":
                    field = BindingOperations.GetBinding((e.OriginalSource as TextBlock), TextBlock.TextProperty).Path.Path;
                    break;
                case "RichTextBox":
                    field = BindingOperations.GetBinding((e.OriginalSource as Xceed.Wpf.Toolkit.RichTextBox), Xceed.Wpf.Toolkit.RichTextBox.TextProperty).Path.Path;
                    break;

            }
            if (field != String.Empty)
            {
                //_LLViewModel.addSortDescription(field,true);
            }
        }
        private void HandleListSortAscCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void HandleListSortDescExecuted(object sender, ExecutedRoutedEventArgs e)
        {

            String field = String.Empty;

            switch (e.OriginalSource.GetType().Name)
            {
                case "TextBox":
                    field = BindingOperations.GetBinding((e.OriginalSource as TextBox), TextBox.TextProperty).Path.Path;
                    break;
                case "TextBlock":
                    field = BindingOperations.GetBinding((e.OriginalSource as TextBlock), TextBlock.TextProperty).Path.Path;
                    break;
                case "RichTextBox":
                    field = BindingOperations.GetBinding((e.OriginalSource as Xceed.Wpf.Toolkit.RichTextBox), Xceed.Wpf.Toolkit.RichTextBox.TextProperty).Path.Path;
                    break;
            }
            if (field != String.Empty)
            {
                //_LLViewModel.addSortDescription(field,false);
            }

        }
        private void HandleListSortDescCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void HandleShowOrderViewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            String orderNr = null;
            if (e.OriginalSource.GetType().Name == "TextBlock") orderNr = (e.OriginalSource as TextBlock).Text;
            if (e.OriginalSource.GetType().Name == "TextBox") orderNr = (e.OriginalSource as TextBox).Text;

            //if (orderNr != null)
            //{
            //    OrderView ord = new OrderView(orderNr);
            //    ord.listBox1.Width = this.ActualWidth - 50;

            //    ToolBar_filter.Visibility = Visibility.Collapsed;
            //    AddTabItem(ord);
            //}
        }
        private void HandleShowOrderViewCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        #endregion EventHandler der CommandBindings


        ////private void btnDelete_Click(object sender, RoutedEventArgs e)
        ////{
        ////    int tabIndex = (int)(sender as Button).CommandParameter;

        ////    var item = MainTab.Items.Cast<TabItem>().Where(i => i.TabIndex == tabIndex).SingleOrDefault();

        ////    TabItem tab = item as TabItem;

        ////    if (tab != null)
        ////    {
        ////        if (_tabItems.Count < 3)
        ////        {
        ////            MessageBox.Show("Cannot remove last tab.");
        ////        }
        ////        else if (MessageBox.Show(string.Format("Are you sure you want to remove the tab '{0}'?", tab.Header.ToString()),
        ////            "Remove Tab", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
        ////        {
        ////            // get selected tab
        ////            TabItem selectedTab = MainTab.SelectedItem as TabItem;

        ////            // clear tab control binding
        ////            MainTab.DataContext = null;

        ////            _tabItems.Remove(tab);

        ////            // bind tab control
        ////            MainTab.DataContext = _tabItems;

        ////            // select previously selected tab. if that is removed then select first tab
        ////            if (selectedTab == null || selectedTab.Equals(tab))
        ////            {
        ////                selectedTab = _tabItems[0];
        ////            }
        ////            MainTab.SelectedItem = selectedTab;
        ////        }
        ////    }
        ////}

        //public TabItem AddTabItem(object tabItem)
        //{
        //    int count = _tabItems.Count;
        //    bool validMultiple = true;

        //    TabItem tab = new TabItem();
        //    switch (tabItem.GetType().Name)
        //    {
        //        case "LieferlisteView1":
        //            // create new tab item
        //            if (!(_tabItems.Count(x => x.Name.Equals("LieferlisteView1")) > 0))
        //            {
        //                tab.Header = "Lieferliste";
        //                tab.HeaderTemplate = MainTab.FindResource("TabHeader") as DataTemplate;
        //            }
        //            validMultiple = false;
        //            break;
        //        case "Allocation":

        //            // create new tab item

        //            tab.Header = "Maschinenzuteilung";
        //            tab.HeaderTemplate = MainTab.FindResource("TabHeader") as DataTemplate;
        //            break;
        //        case "PlannerControl":

        //            // create new tab item

        //            tab.Header = "Plan " + (tabItem as PlannerControl).myMachine.MachineName;
        //            tab.HeaderTemplate = MainTab.FindResource("TabHeader") as DataTemplate;
        //            break;
        //        case "OrderView":

        //            // create new tab item

        //            tab.Header = "Auftrag " + (tabItem as OrderView).Tag;
        //            tab.HeaderTemplate = MainTab.FindResource("TabHeader") as DataTemplate;
        //            break;
        //    }
        //    // add controls to tab item, this case I added just a textbox
        //    int tabInd = 0;
        //    TabItem t = _tabItems.FirstOrDefault(x => x.Name == tabItem.GetType().Name);
        //    if (t != null)
        //    {
        //        tabInd = _tabItems.IndexOf(t);
        //    }
        //    if (t != null && !validMultiple)
        //    {
        //        MainTab.SelectedIndex = tabInd;
        //    }
        //    else
        //    {
        //        tab.Content = tabItem;
        //        tab.TabIndex = tabItem.GetHashCode();
        //        tab.Name = tabItem.GetType().Name;
        //        _tabItems.Add(tab);
        //        MainTab.SelectedItem = tab;
        //    }


        //    return tab;
        //}
    


        /// <summary>
        /// This method allows the user to select a file to open 
        /// (so the view-model can implement 'Open File' functionality).
        /// </summary>
        public bool UserSelectsFileToOpen(out string filePath)
        {
            var openFileDialog = new OpenFileDialog();
            var result = openFileDialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                filePath = openFileDialog.FileName;
                return true;
            }
            else
            {
                filePath = null;
                return false;
            }
        }

        /// <summary>
        /// This method allows the user to select a new filename for an existing file 
        /// (so the view-model can implement 'Save As' functionality).
        /// </summary>
        public bool UserSelectsNewFilePath(string oldFilePath, out string newFilePath)
        {
            var saveFileDialog = new SaveFileDialog();
            //saveFileDialog.FileName = this.ViewModel.ActiveDocument.FilePath;

            var result = saveFileDialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                newFilePath = saveFileDialog.FileName;
                return true;
            }
            else
            {
                newFilePath = string.Empty;
                return false;
            }
        }

        /// <summary>
        /// Display an error message dialog box.
        /// This allows the view-model to display error messages.
        /// </summary>
        public void ErrorMessage(string msg)
        {
            MessageBox.Show(this, msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Allow the user to confirm whether they want to close a modified document.
        /// </summary>
        public bool QueryCloseModifiedDocument(DeliveryListViewModel document)
        {
            string msg = " has been modified but not saved.\n" +
                         "Do you really want to close it?";
            var result = MessageBox.Show(this, msg, "File modified but not saved", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            return result == MessageBoxResult.Yes;
        }

        /// <summary>
        /// Allow the user to confirm whether they want to close the application 
        /// when 1 or more documents are modified.
        /// </summary>
        public bool QueryCloseApplicationWhenDocumentsModified()
        {
            string msg = "1 or more open files have been modified but not saved.\n" +
                         "Do you really want to exit?";
            var result = MessageBox.Show(this, msg, "File(s) modified but not saved", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            return result == MessageBoxResult.Yes;
        }

        /// <summary>
        /// Convenient accessor for the view-model.
        /// </summary>
        private MainWindowViewModel ViewModel
        {
            get
            {
                return (MainWindowViewModel)this.DataContext;
            }
        }

        /// <summary>
        /// Event raised when the 'NewFile' command is executed.
        /// </summary>
        private void NewFile_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.ViewModel.NewFile();
        }

        /// <summary>
        /// Event raised when the 'OpenFile' command is executed.
        /// </summary>
        private void OpenFile_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.ViewModel.OpenFile();
        }

        /// <summary>
        /// Event raised when the 'SaveFile' command is executed.
        /// </summary>
        private void SaveFile_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.ViewModel.SaveFile();
        }

        /// <summary>
        /// Event raised when the 'SaveFileAs' command is executed.
        /// </summary>
        private void SaveFileAs_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.ViewModel.SaveFileAs();
        }

        /// <summary>
        /// Event raised when the 'SaveAllFiles' command is executed.
        /// </summary>
        private void SaveAllFiles_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.ViewModel.SaveAllFiles();
        }

        /// <summary>
        /// Event raised when the 'CloseFile' command is executed.
        /// </summary>
        private void CloseFile_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.ViewModel.CloseFile();
        }

        /// <summary>
        /// Event raised when the 'CloseAllFiles' command is executed.
        /// </summary>
        private void CloseAllFiles_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.ViewModel.CloseAllFiles();
        }

        /// <summary>
        /// Event raised when the 'ShowAllPanes' command is executed.
        /// </summary>
        private void ShowAllPanes_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.ViewModel.ShowAllPanes();
        }

        /// <summary>
        /// Event raised when the 'HideAllPanes' command is executed.
        /// </summary>
        private void HideAllPanes_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.ViewModel.HideAllPanes();
        }

        /// <summary>
        /// Exit the application.
        /// </summary>
        private void Exit_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Event raised when AvalonDock has loaded.
        /// </summary>
        private void avalonDockHost_AvalonDockLoaded(object sender, EventArgs e)
        {
            if (System.IO.File.Exists(LayoutFileName))
            {
                //
                // If there is already a saved layout file, restore AvalonDock layout from it.
                //
                avalonDockHost.DockingManager.RestoreLayout(LayoutFileName);
            }
            else
            {
                //
                // This line of code can be uncommented to get a list of resources.
                //
                //string[] names = this.GetType().Assembly.GetManifestResourceNames();

                //
                // Load the default AvalonDock layout from an embedded resource.
                //
                var assembly = Assembly.GetExecutingAssembly();
                using (var stream = assembly.GetManifestResourceStream(DefaultLayoutResourceName))
                {
                    avalonDockHost.DockingManager.RestoreLayout(stream);
                }
            }
        }

        /// <summary>
        /// Event raised when a document is being closed by clicking the 'X' button in AvalonDock.
        /// </summary>
        private void avalonDockHost_DocumentClosing(object sender, DocumentClosingEventArgs e)
        {
            var document = (DeliveryListViewModel)e.Document;
            if (!this.ViewModel.QueryCanCloseFile(document))
            {
                e.Cancel = true;
            }
        }

        /// <summary>
        /// Event raised when the window is about to close.
        /// </summary>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            //
            // Notify the view-model that the application is closing,
            // allows the view-model the chance to cancel application exit.
            //
            if (!this.ViewModel.OnApplicationClosing())
            {
                //
                // The view-model has cancelled application exit.
                // This will happen when the 1 or more documents have been modified but not saved
                // and the user has selected 'No' when asked to confirm application exit.
                //
                e.Cancel = true;
                return;
            }

            //
            // When the window is closing, save AvalonDock layout to a file.
            //
            avalonDockHost.DockingManager.SaveLayout(LayoutFileName);
        }
    }
}
