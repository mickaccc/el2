using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Data;
using Lieferliste_WPF.Entities;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Lieferliste_WPF.Commands;
using System.Windows.Controls;

namespace Lieferliste_WPF.ViewModels
{
    class LLViewModel:ViewModelBase
    {
        public bool showInVisible { get; set; }
        public ObservableCollection<Process> Processes { get; private set; }
        private ICollectionView _processCV;
        private Dictionary<String, String> _filterCriterias;
        public ICommand SortAscCommand { get; private set; }
        public ICommand SortDescCommand { get; private set; }

        #region Constructors
        public LLViewModel()
        {
            LoadData();
            _processCV = CollectionViewSource.GetDefaultView(Processes);
            _filterCriterias = new Dictionary<string, string>();
            _processCV.Filter = delegate(Object item)
            {
                Process temp = item as Process;
                bool retValue;
                retValue = (showInVisible) ? temp.isInVisible : !temp.isInVisible;
                if (retValue)
                {
                    bool tmpBool = true;
                    PropertyInfo[] prop = GetType().GetProperties();
                    foreach (String key in _filterCriterias.Keys)
                    {
                        PropertyInfo poI = temp.GetType().GetProperty(key);
                        if (poI.PropertyType.Name == "String")
                        {
                            String value = (String)poI.GetValue(item, null);
                            Regex regex = new Regex("(?i)"+_filterCriterias[key]);
                            
                            Match match = regex.Match(value);
                            tmpBool = match.Success;
                            if (tmpBool) break;
                        }
                        if (poI.PropertyType.Name == "Boolean")
                        {
                            bool value = (bool)poI.GetValue(item, null);
                            tmpBool = value == Convert.ToBoolean(_filterCriterias[key]);
                        }
                    }
                    retValue = tmpBool;
                }
                
                return retValue;
            };
            addFilterCriteria("isInvisible", "false");
            SortAscCommand = new ActionCommand(OnSortAscExecuted, OnSortAscCanExecute);
            SortDescCommand = new ActionCommand(OnSortDescExecuted, OnSortDescCanExecute);

        }



        #endregion Constructors
        private void LoadData()
        {
            Processes = new ObservableCollection<Process>();
            DataSetEL4.lieferlisteDataTable dataTable = DbManager.Instance().GetLieferliste();

            foreach (DataSetEL4.lieferlisteRow row in dataTable)
            {
                Process pro = new Process(row.AID);
                pro.isFilling = true;
                pro.ExecutionNumber = String.Format("{0:d4}", row.VNR);
                pro.ExecutionShortText = row.Text;
                pro.Material = row.Material;
                pro.MaterialDescription = row.Teil;
                pro.Quantity = (row.IsQuantityNull()) ? 0:row.Quantity;
                pro.Quantity_yield = row.Is_Quantity_yieldNull() ? 0:row._Quantity_yield;
                pro.Quantity_scrap = row.Is_Quantity_scrapNull() ? 0:row._Quantity_scrap;
                pro.Quantity_rework = row.Is_Quantity_reworkNull() ? 0:row._Quantity_rework;
                pro.Quantity_miss = row.Is_Quantity_missNull() ? 0 : row._Quantity_miss;

                pro.CommentMei = row.Bem_M;
                pro.CommentTL = row.Bem_T;
                pro.CommentMA = row.Bem_MA;

                pro.isHighPrio = row.Dringend;
                pro.CommentHighPrio = row.Bemerkung;

                pro.isReady = row.fertig;
                pro.isInVisible = row.ausgebl;
                pro.isPortfolioAvail = row.Mappe;

                pro.marker = row.marker;
                pro.PlanTermin = row.Plantermin;
                pro.LieferTermin = row.LieferTermin;
                pro.Termin = row.IsTerminNull() ? (DateTime?)null:row.Termin;
                pro.LastEnd = row.SpaetEnd;

                pro.WorkPlace = row.Arbeitsplatz;
                pro.WorkPlaceSAP = row.ArbPlSAP;
                pro.WorkSpace = row.ArbBereich;

                pro.isFilling = false;
                Processes.Add(pro);
            }
        }
        
        public void addFilterCriteria(string PropertyName, string CriteriaValue)
        {
            if(_filterCriterias.ContainsKey(PropertyName))
            {
                _filterCriterias[PropertyName] = CriteriaValue;
            }else{
                _filterCriterias.Add(PropertyName,CriteriaValue);
            }
            
        }

        public void removeFilterCriteria(string PropertyName)
        {
            _filterCriterias.Remove(PropertyName);
            
        }

        public void refresh()
        {
            _processCV.Refresh();
        }
        private bool FilterPro(FilterEventArgs e)
        {
            bool retValue;
            retValue = (showInVisible) ? !(e.Item as Process).isInVisible : (e.Item as Process).isInVisible;

            return retValue;
        }

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
                case "RichTextBox":
                    field = BindingOperations.GetBinding((parameter as Xceed.Wpf.Toolkit.RichTextBox), Xceed.Wpf.Toolkit.RichTextBox.TextProperty).Path.Path;
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
                case "RichTextBox":
                    field = BindingOperations.GetBinding((parameter as Xceed.Wpf.Toolkit.RichTextBox), Xceed.Wpf.Toolkit.RichTextBox.TextProperty).Path.Path;
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
        public void addSortDescription(String field,bool asc)
        {
            _processCV.SortDescriptions.Clear();
            if(asc)
            {
                _processCV.SortDescriptions.Add(new SortDescription(field,ListSortDirection.Ascending));
            }else{
                _processCV.SortDescriptions.Add(new SortDescription(field,ListSortDirection.Descending));
            }
        }
    }
}
