using Lieferliste_WPF.Commands;
using Lieferliste_WPF.Entities;
using Lieferliste_WPF.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Lieferliste_WPF.ViewModels
{
    /// <summary>
    /// The view-model for a text file document.
    /// </summary>
    class DeliveryListViewModel : Support.CrudVM, IDisposable
    {
        #region Fields

        static DeliveryListViewModel _this = new DeliveryListViewModel();
        private bool isRun = false;
        public bool showInVisible { get; set; }
        /// <summary>
        /// Collection of Processes
        /// </summary>
        public ObservableCollection<ProcessVM> Processes { get; private set; }
        //public List<OrderList_Result> OrderDetails { get; private set; }
        private String _selectedOrder;
        private ICollectionView _processCV;
        private Dictionary<String, String> _filterCriterias;
        private String _sortField;
        private String _sortDirection;
        public ProcessVM SelectedProcess { get; set; }
        public ICommand SortAscCommand { get; private set; }
        public ICommand SortDescCommand { get; private set; }
        public ICommand ToArchiveCommand { get; private set; }
 

        /// <summary>
        /// Tooltip to display in the UI.
        /// </summary>
        public string ToolTip
        {
            get
            {
                var toolTip = new StringBuilder();
                if (_filterCriterias.Count>0)
                {
                    toolTip.Append("gefiltert:\n");
                    foreach (KeyValuePair<String,String> c in _filterCriterias)
                    {
                        toolTip.Append(c.Key).Append(" = ").Append(c.Value).Append("\n");
                    }
                }
                if (_sortField != string.Empty)
                {
                    toolTip.Append("sortiert nach:\n").Append(_sortField).Append(" / ").Append(_sortDirection);
                }
  
                return toolTip.ToString();
            }
        }
        public string Title { get { return "Lieferliste"; } }

        #endregion Fields

        #region Constructors
        private DeliveryListViewModel()
        {
        }
        private void Initialize()
        {
            _processCV = CollectionViewSource.GetDefaultView(Processes);
            _filterCriterias = new Dictionary<string, string>();
            _processCV.Filter = delegate(Object item)
            {
                ProcessVM temp = item as ProcessVM;
                if (temp == null) return true;
                bool retValue;
                retValue = Convert.ToBoolean(!temp.TheProcess.ausgebl);
                retValue = !temp.IsDeleted;
                if (retValue)
                {
                    bool tmpBool = true;
                    PropertyInfo[] prop = GetType().GetProperties();
                    foreach (String key in _filterCriterias.Keys)
                    {
                        PropertyInfo poI = temp.TheProcess.GetType().GetProperty(key);
                        if (poI.PropertyType.Name == "String")
                        {
                            String value = (String)poI.GetValue(temp.TheProcess);
                            Regex regex = new Regex("(?i)" + _filterCriterias[key]);

                            Match match = regex.Match((value == null) ? String.Empty : value);
                            tmpBool = match.Success;
                            if (tmpBool) break;
                        }
                        if (poI.PropertyType.Name == "Boolean")
                        {
                            bool value = (bool)poI.GetValue(temp.TheProcess, null);
                            tmpBool = value == Convert.ToBoolean(_filterCriterias[key]);
                        }
                    }
                    retValue = tmpBool;
                }

                return retValue;
            };
            addFilterCriteria("ausgebl", "false");
            SortAscCommand = new ActionCommand(OnSortAscExecuted, OnSortAscCanExecute);
            SortDescCommand = new ActionCommand(OnSortDescExecuted, OnSortDescCanExecute);
            ToArchiveCommand = new ActionCommand(OnArchiveExecuted, OnArchiveCanExecute);
        }

        #endregion Constructors
        public static DeliveryListViewModel This
        {
            get
            {
                return _this;
            }
        }

        public void SelectionChange(String OrderNumber)
        {
            _selectedOrder = OrderNumber;
            RaisePropertyChanged("OrderDetails");
        }

        #region Methods

        public void refresh()
        {
            _processCV.Refresh();
        }
        public override void addFilterCriteria(string PropertyName, string CriteriaValue)
        {
            int repeat=0;

            if (PropertyName == "alle") repeat=3;

                do
                {
                    switch (repeat)
                    {
                        case 3: PropertyName = "TTNR"; break;
                        case 2: PropertyName = "Teil"; break;
                        case 1: PropertyName = "Auftrag"; break;
                    }
                    repeat--;
                    if (PropertyName == "TTNR") PropertyName = "Material";
                    if (PropertyName == "Teil") PropertyName = "Teil";
                    if (PropertyName == "Auftrag") PropertyName = "AID";
                    if (_filterCriterias.ContainsKey(PropertyName))
                    {
                        _filterCriterias[PropertyName] = CriteriaValue;
                    }
                    else
                    {
                        _filterCriterias.Add(PropertyName, CriteriaValue);
                    }

                } while (repeat > 0);
            
            RaisePropertyChanged("ToolTip");
            _processCV.Refresh();
        }

        public override void removeFilterCriteria(string PropertyName)
        {
            _filterCriterias.Remove(PropertyName);
            RaisePropertyChanged("ToolTip");
            _processCV.Refresh();

        }
protected async override void GetData()
        {
            if (isRun) return;
            using (var data = db)
            {
                ObservableCollection<ProcessVM> _processes = new ObservableCollection<ProcessVM>();
                isRun = true;
                var processes = await (from p in data.lieferliste
                                       where p.abgeschlossen==false
                                       orderby p.SpaetEnd
                                       select p).ToListAsync();

                
                isRun = false;
                foreach (lieferliste l in processes)
                {
                    _processes.Add(new ProcessVM { IsNew = false, TheProcess = l });
                }
                Processes = _processes;
            }
            Initialize();
            RaisePropertyChanged("Processes");

        }
        
        //private void LoadData()
        //{

        //    //Processes = new List<lieferliste>();
        //    using (var ctx = new DB_COS_LIEFERLISTE_SQLEntities())
        //    {
        //        try
        //        {
        //            ctx.lieferlistes.OrderBy(x => x.SpaetEnd).Load();
 
        //            Processes = ctx.lieferlistes.Local;
                    
        //            _logger.Info("loaded lieferlistes");
        //        }
        //        catch (Exception ex)
        //        {
        //            _logger.Error("LoadData lieferlistes",ex);
        //            throw;
        //        }

        //        var t = Processes.FirstOrDefault();
        //        if (t != null)
        //        {
        //            try
        //            {
        //                _selectedOrder = t.AID;
        //                OrderDetails = (from o in ctx.OrderList(_selectedOrder) select o).ToList();
        //            }
        //            catch (Exception ex)
        //            {
        //                _logger.Error("LoadData OrderList",ex);
        //                throw;
        //            }
        //        }

                //Processes = new ObservableCollection<Process>();


                //foreach (DataSetEL4.lieferlisteRow row in dataTable)
                //{
                //    Process pro = new Process(row.AID);
                //    pro.isFilling = true;
                //    pro.ExecutionNumber = String.Format("{0:d4}", row.VNR);
                //    pro.ExecutionShortText = row.Text;
                //    pro.Material = row.Material;
                //    pro.MaterialDescription = row.Teil;
                //    pro.Quantity = (row.IsQuantityNull()) ? 0 : row.Quantity;
                //    pro.Quantity_yield = row.Is_Quantity_yieldNull() ? 0 : row._Quantity_yield;
                //    pro.Quantity_scrap = row.Is_Quantity_scrapNull() ? 0 : row._Quantity_scrap;
                //    pro.Quantity_rework = row.Is_Quantity_reworkNull() ? 0 : row._Quantity_rework;
                //    pro.Quantity_miss = row.Is_Quantity_missNull() ? 0 : row._Quantity_miss;

                //    pro.CommentMei = row.Bem_M;
                //    pro.CommentTL = row.Bem_T;
                //    pro.CommentMA = row.Bem_MA;

                //    pro.isHighPrio = row.Dringend;
                //    pro.CommentHighPrio = row.Bemerkung;

                //    pro.isReady = row.fertig;
                //    pro.isInVisible = row.ausgebl;
                //    pro.isPortfolioAvail = row.Mappe;
                //    pro.isArchivated = row.abgeschlossen;

                //    pro.marker = row.marker;
                //    pro.PlanTermin = row.Plantermin;
                //    pro.LieferTermin = row.LieferTermin;
                //    pro.Termin = row.IsTerminNull() ? (DateTime?)null : row.Termin;
                //    pro.LastEnd = row.SpaetEnd;

                //    pro.WorkPlace = row.Arbeitsplatz;
                //    pro.WorkPlaceSAP = row.ArbPlSAP;
                //    pro.WorkSpace = row.ArbBereich;

                //    pro.isFilling = false;
                //    Processes.Add(pro);
                //}
            //}
        //}
 
        public void addSortDescription(String field, bool asc)
        {
            _processCV.SortDescriptions.Clear();
            if (asc)
            {
                _processCV.SortDescriptions.Add(new SortDescription(field, ListSortDirection.Ascending));
                _sortDirection = "Aufsteigend";
            }
            else
            {
                _processCV.SortDescriptions.Add(new SortDescription(field, ListSortDirection.Descending));
                _sortDirection = "Absteigend";
            }
            _sortField = field;
            RaisePropertyChanged("ToolTip");
        }
        #endregion Methods

        #region Commands
        void OnSortAscExecuted(object parameter)
        {
            String field = null;
            switch (parameter.GetType().Name)
            {
                case "TextBox":
                    field = BindingOperations.GetBinding((parameter as TextBox), TextBox.TextProperty).Path.Path;
                    break;
                case "TextBlock":
                    field = BindingOperations.GetBinding((parameter as TextBlock), TextBlock.TextProperty).Path.Path;
                    break;

            }
            if (field != null)
            {
                _processCV.SortDescriptions.Clear();
                _processCV.SortDescriptions.Add(new SortDescription(field, ListSortDirection.Ascending));
 
            }
        }
        bool OnSortAscCanExecute(object parameter)
        {
            return true;
        }
        void OnSortDescExecuted(object parameter)
        {
            String field = null;
            switch (parameter.GetType().Name)
            {
                case "TextBox":
                    field = BindingOperations.GetBinding((parameter as TextBox), TextBox.TextProperty).Path.Path;
                    break;
                case "TextBlock":
                    field = BindingOperations.GetBinding((parameter as TextBlock), TextBlock.TextProperty).Path.Path;
                    break;

            }
            if (field != null)
            {
                _processCV.SortDescriptions.Clear();
                _processCV.SortDescriptions.Add(new SortDescription(field, ListSortDirection.Descending));
            }
        }
        bool OnSortDescCanExecute(object parameter)
        {
            return true;
        }
        void OnArchiveExecuted(object parameter)
        {
            String orderNr = parameter as string;
  
            foreach (ProcessVM pro in Processes.Where(x => x.TheProcess.AID == orderNr))
            {
                pro.IsDeleted=true;
            }
            _processCV.Refresh();

        }
        bool OnArchiveCanExecute(object parameter)
        {
            return true;
        }

        public void Dispose()
        {
            using (var data = new EntitiesLL())
            {
                String a = String.Empty;

                foreach(var pro in Processes.Where(x => x.IsDeleted))
                {
                    if (pro.TheProcess.AID != a)
                    {
                        var l = data.lieferliste.First(y => y.AID == pro.TheProcess.AID);
                        if (l != null)
                        {
                            l.abgeschlossen = true;
                            l.fertig = false;
                            a = l.AID;
                        }
                    }
                }
                    data.SaveChanges();
            }
        }


        #endregion Commands


    }
}
