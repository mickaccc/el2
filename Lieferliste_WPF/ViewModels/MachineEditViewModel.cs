using El2Core.Models;
using El2Core.Utils;
using El2Core.ViewModelBase;
using Lieferliste_WPF.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Lieferliste_WPF.ViewModels
{
    public class MachineEditViewModel : ViewModelBase, IDataErrorInfo
    {

        public string Title { get; } = "Maschinen Zuteilung";
        public ICommand SaveCommand { get; private set; }
        public ICommand CloseCommand { get; private set; }
        public ICommand TextSearchCommand => textSearchCommand ??= new RelayCommand(OnTextSearch);

        private ICollectionView _ressCV;
        private RelayCommand textSearchCommand;
        private string _searchFilterText = String.Empty;
        private readonly IContainerExtension _container;
        private DB_COS_LIEFERLISTE_SQLContext _dbCtx;
        public static ObservableCollection<Ressource>? Ressources { get; set; }
        public static ObservableCollection<WorkArea> WorkAreas { get; private set; } = new();
        public MachineEditViewModel(IContainerExtension container)
        {
            _container = container;
            _dbCtx = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
            LoadData();

            _ressCV = CollectionViewSource.GetDefaultView(Ressources);

            SaveCommand = new ActionCommand(OnSaveExecuted, OnSaveCanExecute);
            CloseCommand = new ActionCommand(OnCloseExecuted, OnCloseCanExecute);

            _ressCV.MoveCurrentToFirst();
            _ressCV.CurrentChanged += OnChanged;
            _ressCV.Filter += RessView_FilterPredicate;
            
        }


        private bool RessView_FilterPredicate(object value)
        {
            var val = value as Ressource;
            Ressource res = (Ressource)value;

            bool accepted = true;
            int v;
            if (!string.IsNullOrWhiteSpace(_searchFilterText))
            {
                _searchFilterText = _searchFilterText.ToUpper();
                if (int.TryParse(_searchFilterText, out v))
                {
                    if (!(accepted = res.Inventarnummer?.ToUpper().StartsWith(_searchFilterText) ?? false))
                        accepted = res.RessourceCostUnits.Any(y => y.CostId.Equals(v));
                }
                else
                {
                    accepted = (res.RessName != null) ? res.RessName.ToUpper().Contains(_searchFilterText) : false;
                }
            }
            return accepted;
        }
        private void OnTextSearch(object commandParameter)
        {
            if (commandParameter is TextChangedEventArgs change)
            {
                TextBox tb = (TextBox)change.OriginalSource;
                if (tb.Text.Length >= 3 || tb.Text.Length == 0)
                {
                    _searchFilterText = tb.Text;

                    var uiContext = SynchronizationContext.Current;
                    uiContext?.Send(x => _ressCV.Refresh(), null);
                }
            }
        }
        private void OnChanged(object? sender, EventArgs e)
        {
            

        }
        public string Error { get { return null; } }

        public string this[string columnName]
        {
            get
            {
                User us = (User)_ressCV.CurrentItem;
                string result = null;
                if (columnName == nameof(User.UsrName))
                {
                    if (us.UsrName.IsNullOrEmpty()) return "Der Eintrag darf nicht leer sein";
                }
                return result;
            }
        }
        private void OnCloseExecuted(object obj)
        {
            if(obj is Window ob)
            {
                    if (_dbCtx.ChangeTracker.HasChanges())
                    {
                        MessageBoxResult result = MessageBox.Show("Wollen Sie die Änderungen noch Speichern?", "Datenbank Speichern"
                            , MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (result == MessageBoxResult.Yes) { _dbCtx.SaveChangesAsync(); }

                    }
                
                ob.Close();
               
            }
        }

        private bool OnCloseCanExecute(object arg)
        {
            return true;
        }

        private void OnSaveExecuted(object obj)
        {
            _dbCtx.SaveChanges();
            
        }

        private bool OnSaveCanExecute(object arg)
        {
            return _dbCtx.ChangeTracker.HasChanges();
        }

        private void LoadData()
        {
            
                Ressources = _dbCtx.Ressources
                    .Include(y => y.RessourceCostUnits)
                    .Include(x => x.WorkArea)
                    .Where(y => y.Inventarnummer != null)
                    .ToObservableCollection();

                

                WorkAreas = _dbCtx.WorkAreas
                    .OrderBy(x => x.Bereich)
                    .Select(y => new WorkArea() { WorkAreaId = y.WorkAreaId, Bereich = y.Bereich })
                    .ToObservableCollection();
                WorkAreas.Insert(0, new WorkArea() { WorkAreaId = 0, Bereich = "nicht zugeteilt" });
                     
        }
    }
    public class MachineNameValidationRule:ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            
            if (value is string val)
            {
                if (val.IsNullOrEmpty()) { return new ValidationResult(false, "Der Eintrag darf nicht leer sein"); }
            }
            return ValidationResult.ValidResult;
        }
    }
}
