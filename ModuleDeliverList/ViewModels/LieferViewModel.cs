using CompositeCommands.Core;
using El2Core.Constants;
using El2Core.Models;
using El2Core.Services;
using El2Core.Utils;
using El2Core.ViewModelBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Prism.Events;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading;


namespace ModuleDeliverList.ViewModels
{
    [System.Runtime.Versioning.SupportedOSPlatform("windows10.0")]
    internal class LieferViewModel : ViewModelBase, IViewModel
    {
        public ICollectionView OrdersView { get; private set; }
        public ICommand TextSearchCommand => _textSearchCommand ??= new RelayCommand(OnTextSearch);
        public ICommand FilterDeleteCommand => _filterDeleteCommand ??= new RelayCommand(OnFilterDelete);
        public ICommand ToggleFilterCommand => _toggleFilterCommand ??= new RelayCommand(OnToggleFilter);
        public ICommand SortAscCommand => _sortAscCommand ??= new RelayCommand(OnAscSortExecuted);
        public ICommand SortDescCommand => _sortDescCommand ??= new RelayCommand(OnDescSortExecuted);
        public ICommand ProjectPrioCommand { get; private set; }
        public ICommand CreateRtfCommand { get; private set; }
        public ICommand CreatePdfCommand { get; private set; }
        public ICommand CreateHtmlCommand { get; private set; }
        public ICommand SendMailCommand { get; private set; }
        private ConcurrentObservableCollection<Vorgang> _orders { get; } = [];
        private DB_COS_LIEFERLISTE_SQLContext DBctx { get; set; }
        public ActionCommand SaveCommand { get; private set; }
        public ActionCommand FilterSaveCommand { get; private set; }
        public ActionCommand InvisibilityCommand { get; private set; }
        public string Title { get; } = "Lieferliste";
        public bool HasChange => DBctx.ChangeTracker.HasChanges();
        private readonly ILogger _Logger;
        private readonly Dictionary<string, string> _filterCriterias = new();
        private readonly string _sortField = string.Empty;
        private readonly string _sortDirection = string.Empty;
        private RelayCommand? _textSearchCommand;
        private RelayCommand? _filterDeleteCommand;
        private RelayCommand? _toggleFilterCommand;
        private RelayCommand? _sortAscCommand;
        private RelayCommand? _sortDescCommand;
        private string _searchFilterText = string.Empty;
        private string _selectedProjectFilter = string.Empty;
        private string _selectedSectionFilter = string.Empty;
        private string _markerCode = string.Empty;
        private static System.Timers.Timer? _autoSaveTimer;
        private IContainerProvider _container;
        private IEventAggregator _ea;
        private IUserSettingsService _settingsService;
        private CmbFilter _selectedDefaultFilter;
        private static List<Ressource> _ressources = [];
        private static SortedDictionary<int, string> _sections = [];
        public SortedDictionary<int, string> Sections => _sections;
        public List<string> PersonalFilterKeys { get; } = [.. PersonalFilterContainer.GetInstance().Keys];
        private ObservableCollection<ProjectStruct> _projects = new();
        public ObservableCollection<ProjectStruct> Projects
        {
            get { return _projects; }
            set
            {
                if (_projects != value)
                {
                    _projects = value;
                    NotifyPropertyChanged(() => Projects);
                }
            }
        }

        private bool _filterInvers;
        public bool FilterInvers
        {
            get { return _filterInvers; }
            set
            {
                if (_filterInvers != value)
                {
                    _filterInvers = value;
                    NotifyPropertyChanged(() => FilterInvers);
                    OrdersView.Refresh();
                }
            }
        }
        public string SearchFilterText
        {
            get { return _searchFilterText; }
            set
            {
                if (value != _searchFilterText)
                {
                    _searchTxtLock = true;
                    _searchFilterText = value;
                    NotifyPropertyChanged(() => SearchFilterText);
                    OrdersView.Refresh();
                    _searchTxtLock = false;
                }
            }
        }
        public string MarkerCode
        {
            get { return _markerCode; }
            set
            {
                if (value != _markerCode)
                {
                    _markerCode = value;
                    NotifyPropertyChanged(() => MarkerCode);
                    OrdersView.Refresh();
                }
            }
        }
        private static readonly object _lock = new();
        private static bool _searchTxtLock;
        private IApplicationCommands _applicationCommands;

        public IApplicationCommands ApplicationCommands
        {
            get => _applicationCommands;
            set
            {
                if (_applicationCommands != value)
                    _applicationCommands = value;
                NotifyPropertyChanged(() => ApplicationCommands);
            }
        }
        public NotifyTaskCompletion<ICollectionView>? OrderTask { get; private set; }

        internal CollectionViewSource OrdersViewSource { get; private set; } = new();

        public enum CmbFilter
        {
            [Description("Leer")]
            // ReSharper disable once InconsistentNaming
            NOT_SET = 0,
            [Description("ausgeblendet")]
            INVISIBLE,
            [Description("zum ablegen")]
            READY,
            [Description("Aufträge zum Starten")]
            START,
            [Description("Entwicklungsmuster")]
            DEVELOP,
            [Description("Verkaufsmuster")]
            SALES,
            [Description("Projekte mit Verzug")]
            PROJECTS_LOST,
            [Description("rote Aufträge")]
            ORDERS_RED,
            [Description("rote Projekte")]
            PROJECTS_RED,
            [Description("EXTERN")]
            EXERTN
        }
        public CmbFilter SelectedDefaultFilter
        {
            get => _selectedDefaultFilter;
            set
            {
                if (_selectedDefaultFilter != value)
                {
                    _selectedDefaultFilter = value;
                    NotifyPropertyChanged(() => SelectedDefaultFilter);
                    OrdersView.Refresh();
                }
            }
        }
        private string _selectedPersonalFilter;

        public string? SelectedPersonalFilter
        {
            get
            {
                return _selectedPersonalFilter;
            }
            set
            {
                if (value != _selectedPersonalFilter)
                {
                    _selectedPersonalFilter = value;
                    NotifyPropertyChanged(() => SelectedPersonalFilter);
                    OrdersView.Refresh();
                }
            }
        }

        public string SelectedProjectFilter
        {
            get => _selectedProjectFilter;
            set
            {
                if (value != _selectedProjectFilter)
                {
                    _selectedProjectFilter = value;
                    NotifyPropertyChanged(() => SelectedProjectFilter);
                    OrdersView.Refresh();
                }
            }
        }

        public string SelectedSectionFilter
        {
            get { return _selectedSectionFilter; }
            set
            {
                if (_selectedSectionFilter != value)
                {
                    _selectedSectionFilter = value;
                    NotifyPropertyChanged(() => SelectedSectionFilter);
                    OrdersView.Refresh();
                }
            }
        }


        public LieferViewModel(IContainerProvider container,
            IApplicationCommands applicationCommands,
            IEventAggregator ea,
            IUserSettingsService settingsService)
        {
            _applicationCommands = applicationCommands;
            DBctx = container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
            _container = container;
            var factory = _container.Resolve<ILoggerFactory>();
            _Logger = factory.CreateLogger<LieferViewModel>();
            _ea = ea;
            _settingsService = settingsService;

            InvisibilityCommand = new ActionCommand(OnInvisibilityExecuted, OnInvisibilityCanExecute);
            SaveCommand = new ActionCommand(OnSaveExecuted, OnSaveCanExecute);
            FilterSaveCommand = new ActionCommand(OnFilterSaveExecuted, OnFilterSaveCanExecute);
            ProjectPrioCommand = new ActionCommand(OnSetProjectPrioExecuted, OnSetProjectPrioCanExecute);
            CreateRtfCommand = new ActionCommand(OnCreateRtfExecuted, OnCreateRtfCanExecute);
            CreateHtmlCommand = new ActionCommand(OnCreateHtmlExecuted, OnCreateHtmlCanExecute);
            OrderTask = new NotifyTaskCompletion<ICollectionView>(LoadDataAsync());

            _ea.GetEvent<MessageVorgangChanged>().Subscribe(MessageVorgangReceived);
            _ea.GetEvent<MessageOrderChanged>().Subscribe(MessageOrderReceived);
            _ea.GetEvent<MessageOrderArchivated>().Subscribe(MessageOrderArchivated);

            if (_settingsService.IsAutoSave) SetAutoSave();
        }

        private AbstracatBuilder CreateTableBuilder()
        {
            TableBuilder t = new TableBuilder();
            string[] headers = new[] { "Auftragsnummer", "Material", "Bezeichnng", "Kurztext", "Termin" };
            AbstracatBuilder builder = new FlowTableBuilder(headers);
            List<Vorgang> query = OrdersView.Cast<Vorgang>().ToList();
            var sel = query.Select(x => new string?[]
            {
                x.Aid,
                x.AidNavigation.Material,
                x.AidNavigation.MaterialNavigation?.Bezeichng,
                x.Text,
                x.Termin.ToString(),
            }).ToList();
            builder.SetContext((List<string?[]>)sel);
            t.Build(builder);
            return builder;
        }

        // This method accepts an input stream and a corresponding data format.  The method
        // will attempt to load the input stream into a TextRange selection, apply Bold formatting
        // to the selection, save the reformatted selection to an alternat stream, and return 
        // the reformatted stream.  
        Stream BoldFormatStream(Stream inputStream, string dataFormat)
        {
            // A text container to read the stream into.
            FlowDocument workDoc = new FlowDocument();
            TextRange selection = new TextRange(workDoc.ContentStart, workDoc.ContentEnd);
            Stream outputStream = new MemoryStream();

            try
            {
                // Check for a valid data format, and then attempt to load the input stream
                // into the current selection.  Note that CanLoad ONLY checks whether dataFormat
                // is a currently supported data format for loading a TextRange.  It does not 
                // verify that the stream actually contains the specified format.  An exception 
                // may be raised when there is a mismatch between the specified data format and 
                // the data in the stream. 
                if (selection.CanLoad(dataFormat))
                    selection.Load(inputStream, dataFormat);
            }
            catch (Exception e) { return outputStream; /* Load failure; return a null stream. */ }

            // Apply Bold formatting to the selection, if it is not empty.
            if (!selection.IsEmpty)
                selection.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);

            // Save the formatted selection to a stream, and return the stream.
            if (selection.CanSave(dataFormat))
                selection.Save(outputStream, dataFormat);

            return outputStream;
        }

        private void MessageOrderArchivated(OrderRb rb)
        {
            try
            {
                lock (_lock)
                {
                    var o = _orders.FirstOrDefault(x => x.Aid == rb.Aid);
                    if (o != null)
                    {

                        if (rb.Abgeschlossen)
                        {
                            _orders.Remove(o);
                        }
                        DBctx.ChangeTracker.Entries<OrderRb>().First(x => x.Entity.Aid == rb.Aid).State = EntityState.Unchanged;
                        OrdersView.Refresh();
                    }
                }
            }
            catch (Exception ex)
            {
                _Logger.LogError("{message}", ex.ToString());
                MessageBox.Show(ex.Message, "MsgReceivedArchivated", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void MessageOrderReceived(List<(string, string)?> rb)
        {
            try
            {
                Task.Run(() =>
                {
                    lock (_lock)
                    {
                        foreach ((string, string) rbId in rb.Where(x => x != null))
                        {
                            var o = _orders.FirstOrDefault(x => x.Aid == rbId.Item2);
                            if (o != null)
                            {
                    
                                DBctx.ChangeTracker.Entries<Vorgang>().First(x => x.Entity.VorgangId == o.VorgangId).State = EntityState.Detached;
                                DBctx.Entry<Vorgang>(o).Reload();
                                o.RunPropertyChanged();
                                
                            }
                            else
                            {
                                foreach (var v in DBctx.Vorgangs.Where(x => x.Aid == rbId.Item2))
                                {
                                    Application.Current.Dispatcher.Invoke(AddRelevantProcess, v.VorgangId);
                                }
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                _Logger.LogError("{message}", ex.ToString());
                MessageBox.Show(ex.Message, "MsgReceivedLieferlisteOrder", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void MessageVorgangReceived(List<(string, string)?> vrgIdList)
        {
            try
            {
                Task.Run(() =>
         {
            lock (_lock)
            {
                foreach (var vrg in vrgIdList.Where(x => x != null))
                {
                     if (vrg != null)
                     {
                         var v = _orders.FirstOrDefault(x => x.VorgangId == vrg.Value.Item2);
                         if (v != null)
                         {
                             DBctx.Entry<Vorgang>(v).Reload();
                             v.RunPropertyChanged();
                             if (v.Aktuell == false)
                             {
                                 _orders.Remove(v);
                                 DBctx.ChangeTracker.Entries<Vorgang>().First(x => x.Entity.VorgangId == v.VorgangId).State = EntityState.Unchanged;
                             }
                         }
                         else v = DBctx.Vorgangs.FirstOrDefault(x => x.VorgangId.Trim() == vrg.Value.Item2);
                         {
                             if (v != null && v.Aktuell)
                                 Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, AddRelevantProcess, vrg.Value.Item2);
                         }
                     }
                }
            }
         });
            }
            catch (Exception ex)
            {
                _Logger.LogError("{message}", ex.ToString());
                MessageBox.Show(ex.Message, "MsgReceivedLieferlisteVorgang", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool OrdersView_FilterPredicate(object value)
        {

            try
            {
                var ord = (Vorgang)value;

                var accepted = ord.Aktuell;

                if (accepted && _selectedDefaultFilter == CmbFilter.NOT_SET) accepted = ord.Visability ??= false;

                if (!string.IsNullOrWhiteSpace(_searchFilterText))
                {
                    if (!(accepted = ord.Aid.Contains(_searchFilterText, StringComparison.CurrentCultureIgnoreCase)))
                        if (!(accepted = ord.AidNavigation.Material?.Contains(_searchFilterText, StringComparison.CurrentCultureIgnoreCase) ?? false))
                            accepted = ord.AidNavigation.MaterialNavigation?.Bezeichng?.Contains(_searchFilterText, StringComparison.CurrentCultureIgnoreCase) ?? false;
                }

                if (accepted && _selectedDefaultFilter == CmbFilter.INVISIBLE) accepted = !ord.Visability == !FilterInvers;
                if (accepted && _selectedDefaultFilter == CmbFilter.READY) accepted = ord.AidNavigation.Fertig == !FilterInvers;
                if (accepted && _selectedDefaultFilter == CmbFilter.START)
                    accepted = (ord.Text?.Contains("STARTEN", StringComparison.CurrentCultureIgnoreCase) ?? false) == !FilterInvers;
                if (accepted && _selectedDefaultFilter == CmbFilter.SALES) accepted = (ord.AidNavigation.Pro?.ProjectType ==
                        (int)ProjectTypes.ProjectType.SaleSpecimen) == !FilterInvers;
                if (accepted && _selectedDefaultFilter == CmbFilter.DEVELOP) accepted = (ord.AidNavigation.Pro?.ProjectType ==
                        (int)ProjectTypes.ProjectType.DevelopeSpecimen) == !FilterInvers;
                if (accepted && _selectedDefaultFilter == CmbFilter.EXERTN) accepted = (ord.ArbPlSap == "_EXTERN_") == !FilterInvers;
                if (accepted) accepted = !ord.AidNavigation.Abgeschlossen;
                if (accepted && !string.IsNullOrEmpty(_selectedProjectFilter)) accepted = ord.AidNavigation.ProId == _selectedProjectFilter;
                if (accepted && _selectedSectionFilter != string.Empty) accepted = _ressources?
                        .FirstOrDefault(x => x.Inventarnummer == ord.ArbPlSap?[3..])?
                        .WorkArea?.Bereich == _selectedSectionFilter;
                if (accepted && _markerCode != string.Empty) accepted = ord.AidNavigation.MarkCode?.Contains(_markerCode, StringComparison.InvariantCultureIgnoreCase) ?? false;
                if (accepted && _selectedDefaultFilter == CmbFilter.PROJECTS_LOST) accepted = ProjectsLost(ord.AidNavigation.Pro) == !FilterInvers;
                if (accepted && _selectedDefaultFilter == CmbFilter.ORDERS_RED) accepted = ord.AidNavigation.Prio?.Length > 0 == !FilterInvers;
                if (accepted && _selectedDefaultFilter == CmbFilter.PROJECTS_RED) accepted = ord.AidNavigation.Pro?.ProjectPrio == !FilterInvers;

                if (accepted && _selectedPersonalFilter != null)
                {
                    var b = PersonalFilterContainer.GetInstance();
                    accepted = b[_selectedPersonalFilter].TestValue(ord, _container);
                }
                return accepted;
            }
            catch (Exception ex)
            {
                _Logger.LogError("{message}", ex.ToString());
                MessageBox.Show(ex.ToString(), "Filter Lieferliste", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        private bool ProjectsLost(Project? pro)
        {
            if (pro != null)
            {
                foreach(var item in pro.OrderRbs.Where(x => x.Abgeschlossen == false))
                {
                    if(item.Vorgangs.Any(x => x.SpaetEnd < DateTime.Now && x.QuantityMiss > 0)) return true;
                }
            }
            return false;
        }

        private void OnTextSearch(object commandParameter)
        {
            if (commandParameter is string search)

                if (!_searchTxtLock && search.Length >= 3)
                    SearchFilterText = search;
        }

        private void SetAutoSave()
        {
            _autoSaveTimer = new System.Timers.Timer(15000);
            _autoSaveTimer.Elapsed += OnAutoSave;
            _autoSaveTimer.AutoReset = true;
            _autoSaveTimer.Enabled = true;
        }

        private void OnAutoSave(object? sender, ElapsedEventArgs e)
        {
            try
            {
                if (OrderTask != null && OrderTask.IsSuccessfullyCompleted)
                {
                    if (DBctx.ChangeTracker.HasChanges()) DBctx.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _Logger.LogError("{message}", ex.ToString());
                MessageBox.Show(ex.Message, "AutoSave", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #region Commands

        private bool OnCreateRtfCanExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission(Permissions.CopyClipboard);
        }

        private void OnCreateRtfExecuted(object obj)
        {
            AbstracatBuilder Tbuilder = CreateTableBuilder();

            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            var sett = new UserSettingsService();
            dlg.InitialDirectory = string.IsNullOrEmpty(sett.PersonalFolder) ? Environment.GetFolderPath(Environment.SpecialFolder.Personal) :
                sett.PersonalFolder;
            dlg.FileName = "Document"; // Default file name
            dlg.DefaultExt = ".rtf"; // Default file extension

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                string filename = dlg.FileName;
                using (FileStream fs = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    FlowDocument flow = Tbuilder.GetDoc() as FlowDocument;
                    TextRange tr = new TextRange(flow.ContentStart, flow.ContentEnd);
                    tr.Save(fs, DataFormats.Rtf);

                }
            }
        }
        private bool OnCreateHtmlCanExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission(Permissions.CopyClipboard);
        }

        private void OnCreateHtmlExecuted(object obj)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            var sett = new UserSettingsService();
            dlg.InitialDirectory = string.IsNullOrEmpty(sett.PersonalFolder) ? Environment.GetFolderPath(Environment.SpecialFolder.Personal) :
                sett.PersonalFolder;
            dlg.FileName = "Document"; // Default file name
            dlg.DefaultExt = ".html"; // Default file extension
            dlg.Filter = "Web documents (.html)|*.htm"; // Filter files by extension

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                string filename = dlg.FileName;

                AbstracatBuilder Tbuilder = CreateTableBuilder();
                string content = Tbuilder.GetHtml();
                File.WriteAllText(filename, content);
            }
        }
        private bool OnSetProjectPrioCanExecute(object arg)
        {
            if (arg is Vorgang v)
                if (v.AidNavigation.Pro != null)
                    return PermissionsProvider.GetInstance().GetUserPermission(Permissions.ProjectPrio);
            return false;
        }

        private void OnSetProjectPrioExecuted(object obj)
        {
            if (obj is Vorgang v)
            {
                if (v.AidNavigation.Pro != null)
                    v.AidNavigation.Pro.ProjectPrio = !v.AidNavigation.Pro.ProjectPrio;
            }
        }

        private bool OnInvisibilityCanExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission(Permissions.LieferVrgInvis);
        }

        private void OnInvisibilityExecuted(object obj)
        {
            if (obj is Vorgang v)
            {
                v.Visability = !v.Visability;
                OrdersView.Refresh();
            }
        }
        private bool OnFilterSaveCanExecute(object arg)
        {
            return false;
        }

        private void OnFilterSaveExecuted(object obj)
        {
            throw new NotImplementedException();
        }
        private void OnFilterDelete(object obj)
        {
            SearchFilterText = string.Empty;
            SelectedDefaultFilter = CmbFilter.NOT_SET;
            SelectedProjectFilter = string.Empty;
            SelectedSectionFilter = string.Empty;
            SelectedPersonalFilter = null;
            MarkerCode = string.Empty;
            FilterInvers = false;
        }
        private void OnToggleFilter(object obj)
        {
            FilterInvers = !FilterInvers;
        }


        private void OnSaveExecuted(object obj)
        {
            try
            {
                DBctx.SaveChanges();
            }
            catch (Exception e)
            {
                _Logger.LogError("{message}", e.ToString());
                MessageBox.Show(string.Format("{0}\nInnerEx\n{1}",e.Message,e.InnerException), "OnSave Liefer", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private bool OnSaveCanExecute(object arg)
        {
            try
            {
                return DBctx.ChangeTracker.HasChanges();
            }
            catch (InvalidOperationException e)
            {
                _Logger.LogError("{message}", e.ToString());
                MessageBox.Show(e.Message, "CanSave Liefer", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return false;
        }

        private void OnDescSortExecuted(object parameter)
        {
            var v = Translate();
            if (v != string.Empty)
            {
                OrdersViewSource.SortDescriptions.Clear();
                OrdersViewSource.SortDescriptions.Add(new SortDescription(v, ListSortDirection.Descending));
                OrdersView.Refresh();
            }
        }

        private void OnAscSortExecuted(object parameter)
        {

            var v = Translate();

            if (v != string.Empty)
            {
                OrdersView.SortDescriptions.Clear();
                OrdersView.SortDescriptions.Add(new SortDescription(v, ListSortDirection.Ascending));
                OrdersView.Refresh();
            }
        }
        #endregion

        public string HasMouse { get; set; }
        private string Translate()
        {
            string v = HasMouse switch
            {
                "txtOrder" => "Aid",
                "txtVnr" => "Vorgangs.Vnr",
                "txtMatText" => "MaterialNavigation.Bezeichng",
                "txtMatTTNR" => "Material",
                "txtVrgText" => "Vorgangs.Text",
                "txtPlanT" => "Termin",
                "txtProj" => "ProId",
                "txtPojInfo" => "ProInfo",
                "txtQuant" => "Quantity",
                "txtWorkArea" => "Vorgangs.ArbplSap",
                "txtQuantYield" => "Vorgangs.QuantityYield",
                "txtScrap" => "Vorgangs.QuantityScrap",
                "txtOpen" => "Vorgangs.QuantityMiss",
                "txtRessTo" => "Vorgangs.RidNavigation.RessName",
                _ => string.Empty,
            };

            return v;
        }

        public async Task<ICollectionView> LoadDataAsync()
        {
            //_projects.Add(new Project() { ProjectPsp = "leer"});

            if (!_sections.Keys.Contains(0)) _sections.Add(0, string.Empty);
            var a = await DBctx.Vorgangs
               .Include(v => v.AidNavigation)
               .ThenInclude(x => x.MaterialNavigation)
               .Include(r => r.RidNavigation)
               .Include(m => m.AidNavigation.DummyMatNavigation)
               .Include(d => d.AidNavigation.Pro)
               .ThenInclude(x => x.ProjectAttachments)
               .Include(v => v.ArbPlSapNavigation)
               .Where(x => x.AidNavigation.Abgeschlossen == false)
               .ToListAsync();
            
            var ress = await DBctx.Ressources.AsNoTracking()
                .Include(x => x.WorkArea)
                .ToArrayAsync();
            var filt = await DBctx.ProductionOrderFilters.AsNoTracking().ToArrayAsync();
            _ressources.AddRange(ress);
            SortedSet<ProjectStruct> pl = [new(string.Empty, ProjectTypes.ProjectType.None, string.Empty)];
            await Task.Factory.StartNew(() =>
            {
                HashSet<Vorgang> result = new();

                lock (_lock)
                {
                    SortedDictionary<string, ProjectTypes.ProjectType> proj = new();
 
                        bool relev = false;
                    foreach (var group in a.GroupBy(x => x.Aid))
                    {
                        if (filt.Any(y => y.OrderNumber == group.Key))
                        {
                            relev = true;
                        }
                        else
                        {
                            foreach (var vorg in group)
                            {

                                if (vorg.ArbPlSap?.Length >= 3 && !relev)
                                {
                                    if (int.TryParse(vorg.ArbPlSap[..3], out int c))
                                        if (UserInfo.User.UserCosts.Any(y => y.CostId == c))
                                        {
                                            relev = true;
                                            break;
                                        }
                                }

                            }
                        }
                        if (relev)
                        {
                            foreach (var vorg in group)
                            {
                                if (vorg.AidNavigation.ProId != null)
                                {
                                    var p = vorg.AidNavigation.Pro;
                                    if (p != null)
                                        pl.Add(new(p.ProjectPsp.Trim(), (ProjectTypes.ProjectType)p.ProjectType, p.ProjectInfo));
                                }

                                if (vorg.Aktuell)
                                {
                                    result.Add(vorg);
                                    var inv = (vorg.ArbPlSap != null) ? vorg.ArbPlSap[3..] : string.Empty;
                                    var z = ress.FirstOrDefault(x => x.Inventarnummer?.Trim() == inv)?.WorkArea;
                                    if (z != null)
                                    {
                                        if (!_sections.Keys.Contains(z.Sort) && z.Bereich != null)
                                            _sections.Add(z.Sort, z.Bereich);
                                    }
                                }
                            }
                        }
                        
                        relev = false;
                                               
                    }
                    _orders.AddRange(result.OrderBy(x => x.SpaetEnd));
                }
            });
            Projects.AddRange(pl);
            OrdersView = CollectionViewSource.GetDefaultView(_orders);
            OrdersView.Filter += OrdersView_FilterPredicate;
            ICollectionViewLiveShaping? live = OrdersView as ICollectionViewLiveShaping;
            if (live != null)
            {
                if (live.CanChangeLiveFiltering)
                {
                    live.LiveFilteringProperties.Add("Aktuell");
                    live.LiveFilteringProperties.Add("AidNavigation.Abgeschlossen");
                    live.IsLiveFiltering = true;
                    live.IsLiveSorting = false;
                }
            }
            return OrdersView;
        }
        private bool AddRelevantProcess(string vid)
        {

            try
            {
                using var db = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
                var vrgAdd = db.Vorgangs
                    .Include(x => x.AidNavigation)
                    .ThenInclude(x => x.MaterialNavigation)
                    .Include(x => x.AidNavigation.DummyMatNavigation)
                    .Include(x => x.RidNavigation)
                    .First(x => x.VorgangId.Trim() == vid);

                if (vrgAdd.ArbPlSap?.Length >= 3)
                {
                    if (int.TryParse(vrgAdd.ArbPlSap[..3], out int c))
                        if (UserInfo.User.UserCosts.Any(y => y.CostId == c))
                        {
                            _orders.Add(vrgAdd);
                            return true;
                        }
                }

                return false;
            }
            catch (Exception ex)
            {
                _Logger.LogError("{message}", ex.ToString());
                return false;
            }
        }
        public void Closing()
        {
            if (DBctx.ChangeTracker.HasChanges())
            {
                if (_settingsService.IsSaveMessage)
                {
                    var result = MessageBox.Show("Sollen die Änderungen in Lieferliste gespeichert werden?",
                        Title, MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        DBctx.SaveChanges();
                    }
                }
                else DBctx.SaveChanges();
            }
        }
    }
    public class ProjectStruct : IComparable
    {
        public string ProjectPsp { get; }
        public ProjectTypes.ProjectType ProjectType { get; }
        public string? ProjectInfo { get; }

        public ProjectStruct() { }
        public ProjectStruct(string ProjectPsp, ProjectTypes.ProjectType ProjectType, string? projectInfo)
        {
            this.ProjectPsp = ProjectPsp;
            this.ProjectType = ProjectType;
            this.ProjectInfo = projectInfo;
        }
        public int CompareTo(object? obj)
        {
            if (obj == null) return 1;
            ProjectStruct? otherProjectStruct = obj as ProjectStruct;
            if (otherProjectStruct != null)
                return this.ProjectPsp.CompareTo(otherProjectStruct.ProjectPsp);
            else
                throw new ArgumentException("Object is not a ProjectStruct");
        }
    }
}
