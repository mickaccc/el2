using Lieferliste_WPF.Commands;
using Lieferliste_WPF.Data;
using Lieferliste_WPF.Data.Models;
using Lieferliste_WPF.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Lieferliste_WPF.ViewModels
{
    public class MachineEditViewModel : Base.ViewModelBase, IDataErrorInfo
    {
        

        public ICommand SaveCommand { get; private set; }
        public ICommand CloseCommand { get; private set; }


        private static ICollectionView _ressCV;
        private static bool _isLocked = false;
        private static bool _loaded = false;
        private static bool _isNew = false;
        public static ImmutableList<Ressource>? Ressources { get; private set; }
        public List<WorkArea> WorkAreas { get; private set; } = new();
        public MachineEditViewModel()
        {

            LoadData();

            _ressCV = CollectionViewSource.GetDefaultView(Ressources);

            SaveCommand = new ActionCommand(OnSaveExecuted, OnSaveCanExecute);
            CloseCommand = new ActionCommand(OnCloseExecuted, OnCloseCanExecute);
            
            _ressCV.MoveCurrentToFirst();


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
                if (Dbctx.ChangeTracker.HasChanges())
                {
                    MessageBoxResult result = MessageBox.Show("Wollen Sie die Änderungen noch Speichern?", "Datenbank Speichern"
                        , MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes) { Dbctx.SaveChangesAsync(); }

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
            try
            {
                int error = 0;
                string errMsg ="";
                if (_ressCV.CurrentItem is User us)
                {
    
                }
                if (error != 0) errMsg = "\n" + error + " neue Datensätze sind Fehlerhaft.\n" +
                        "Name oder User sind leer oder nicht eindeutig.\n" +
                        "diese Datensätze wurden nicht gespeichert!";
                int result = Dbctx.SaveChanges();
                if (result > 0)
                {
                    MessageBox.Show("Es wurden " + result + " Datensätze gespeichert", "Nachricht" + errMsg, MessageBoxButton.OK);
                    
                }
                else if (error != 0)
                { MessageBox.Show(errMsg, "Nachricht", MessageBoxButton.OK); }
                else { MessageBox.Show("es wurden keine Änderungen entdeckt.", "Nachricht", MessageBoxButton.OK); }
             

            }
            catch (SqlException ex)
            {

                MessageBox.Show(ex.Message + "\n" + ex.SqlState, "Error", MessageBoxButton.OK,MessageBoxImage.Error);
                
            }
            _isNew = false;
        }

        private bool OnSaveCanExecute(object arg)
        {

            return _isNew || Dbctx.ChangeTracker.HasChanges();
        }



        private void LoadData()
        {
            
            Ressources = Dbctx.Ressources
                .Include(y => y.RessourceCostUnits)
                .Include(x => x.WorkArea)
                .Where(y => y.WorkAreaId != null)
                .ToImmutableList();

            WorkAreas = Dbctx.WorkAreas
                .OrderBy(x => x.Bereich)
                .Select(y => new WorkArea() { WorkAreaId = y.WorkAreaId, Bereich = y.Bereich })
                .ToList();
            WorkAreas.Insert(0, new WorkArea() { WorkAreaId = 0, Bereich = "nicht zugeteilt"});
            _loaded = true;
        }
        private object _selectedItem;
        public object SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                RaisePropertyChanged();
            }
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
