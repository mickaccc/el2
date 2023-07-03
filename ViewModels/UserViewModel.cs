using Lieferliste_WPF.Commands;
using Lieferliste_WPF.Data;
using Lieferliste_WPF.Data.Models;
using Lieferliste_WPF.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
    public class UserViewModel : Base.ViewModelBase, IDataErrorInfo
    {
        
  
        public ICommand SelectionChangedCommand { get; private set; }
        public ICommand WpartChangeCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }
        public ICommand NewCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        public ICommand CloseCommand { get; private set; }


        private static ICollectionView _roCV;
        private static ICollectionView _woCV;
        private static ICollectionView _usrCV;
        private static bool _isLocked = false;
        private static bool _loaded = false;
        private static bool _isNew = false;
        public static ObservableCollection<User>? Users { get; private set; }
        
        public static ObservableCollection<Wpart> Ro { get; private set; }
        
        public static ObservableCollection<Wpart> Wo { get; private set; }
        public static ObservableCollection<Wpart> Cost { get; private set; }


        public UserViewModel()
        {
            Wo = new();
            Ro = new();
            Cost = new();

            LoadData();

            _usrCV = CollectionViewSource.GetDefaultView(Users);
            SelectionChangedCommand = new ActionCommand(OnSelectionChangeExecuted, OnSelectionChangeCanExecute);
            SaveCommand = new ActionCommand(OnSaveExecuted, OnSaveCanExecute);
            NewCommand = new ActionCommand(OnNewExecuted, OnNewCanExecute);
            DeleteCommand = new ActionCommand(OnDeleteExecuted, OnDeleteCanExecute);
            CloseCommand = new ActionCommand(OnCloseExecuted, OnCloseCanExecute);
            
            _usrCV.MoveCurrentToFirst();

        }

        public string Error { get { return null; } }

        public string this[string columnName]
        {
            get
            {
                User us = (User)_usrCV.CurrentItem;
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

        private bool OnDeleteCanExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission("UM03");
        }

        private void OnDeleteExecuted(object obj)
        {
            var u = _usrCV.CurrentItem;
            if (u is User usr)
            { 
            Users?.Remove(usr);
            Dbctx.Users.Remove(usr);
                OnSaveExecuted(usr);
            }
        }

        private bool OnNewCanExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission("UM01") && !_isNew;
        }

        private void OnNewExecuted(object obj)
        {
            User u = new User();
            Users?.Add(u);
            
            _usrCV.MoveCurrentTo(u);
            _isLocked = true;
            foreach(Wpart w in Ro) { w.Check = false; }
            foreach(Wpart w in Wo) { w.Check = false; }
            foreach(Wpart w in Cost) { w.Check=false; }
            _isLocked = false;
            _isNew = true;
        }

        private void OnSaveExecuted(object obj)
        {
            try
            {
                var added = Users.Except(Dbctx.Users);
                User us = _usrCV.CurrentItem as User;
                int error = 0;
                string errMsg ="";
                if (_isNew)
                {
                    if (Dbctx.Users.Where(x => x.UserIdent == us.UserIdent).Count() == 0 &&
                        Dbctx.Users.Where(x => x.UsrName == us.UsrName).Count() == 0)
                    {
                        Dbctx.Users.Add(us);
                        foreach (Wpart wp in Ro.Where(x => x.Check))
                        {
                            UserRole ur = new() { RoleId = wp.Id, UserIdent = us.UserIdent };
                            Dbctx.UserRoles.Add(ur);
                        }
                        foreach (Wpart wp in Wo.Where(x => x.Check))
                        {
                            UserWorkArea uw = new() { WorkAreaId = wp.Id, UserId = us.UserIdent };
                            Dbctx.UserWorkAreas.Add(uw);
                        }
                        foreach (Wpart wp in Cost.Where(x => x.Check))
                        {
                            UserCost uc = new() { CostId = wp.Id, UsrIdent = us.UserIdent };
                            Dbctx.UserCosts.Add(uc);
                        }

                    }
                    else
                    {
                        error++;
                    }
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


        private static bool OnSelectionChangeCanExecute(object arg)
        {
            User user = (User)_usrCV.CurrentItem;
            return !user.UsrName.IsNullOrEmpty() &&
                !user.UserIdent.IsNullOrEmpty();
        }

        private static void OnSelectionChangeExecuted(object obj)
        {
            _isLocked = true;
            if (obj is User us)
            {
                foreach (Wpart wpart in Ro)
                {
                    wpart.Check = us.UserRoles.Any(x => x.RoleId == wpart.Id);
                }
                foreach (Wpart wpart in Wo)
                {
                    wpart.Check = us.UserWorkAreas.Any(x => x.WorkAreaId == wpart.Id);
                }
                foreach (Wpart wpart in Cost)
                {
                    wpart.Check = us.UserCosts.Any(x => x.CostId == wpart.Id);
                }

                _isLocked = false;                           
            }         
        }

        private void LoadData()
        {
            Users = Dbctx.Users
                .Include(x => x.UserWorkAreas)
                .Include(y => y.UserRoles)
                .ThenInclude(c => c.Role)
                .Include(x => x.UserCosts)
                .ThenInclude(x => x.Cost)
                .Where(x => x.Exited == false)
                .OrderBy(o => o.UsrName)
                .ToObservableCollection();
 
            var us = Users.Cast<User>().First();
            foreach (Costunit cost in Dbctx.Costunits)
            {
                Cost.Add(new Wpart()
                {
                    Id = cost.CostunitId,
                    Name = cost.Description,
                    Type = "COST",
                    Check = us.UserCosts.Any(y => y.CostId == cost.CostunitId)
                });
            }
            Cost.OrderBy(o => o.Id);

            foreach (WorkArea work in Dbctx.WorkAreas)
            {
                Wo.Add(new Wpart() { Id = work.WorkAreaId, Name = work.Bereich, Type = "WORKAREA",
                    Check = us.UserWorkAreas.Any(x => x.WorkAreaId == work.WorkAreaId)});
                
            }
            Wo.OrderBy(x => x.Name);
            
            
            int usrlvl = (int)Users.First(x => x.UserIdent == AppStatic.User.UserIdent).UserRoles.Max(y => y.Role.Rolelevel);
            foreach (Role ro in Dbctx.Roles.Where(x => x.Rolelevel <= usrlvl))
            {
                Ro.Add(new Wpart()
                {
                    Id = ro.Id,
                    Name = ro.Description,
                    Type = "ROLE",
                    Check = us.UserRoles.Any(x => x.RoleId == ro.Id)
                });
            }
            _ = Ro.OrderBy(o => o.Name);


            _loaded = true;
        }

        public class Wpart: ViewModels.Base.ViewModelBase, INotifyPropertyChanged
        {

            public int Id { get; set; }
            public string? Name { get; set; }
            public string? EmployNo { get; set; }
            private bool _check;
            public string? Type { get; set; } = null;

            public event PropertyChangedEventHandler? PropertyChanged;
            public bool Check
            {
                get { return _check; }
                set { _check = value;
                    Changed(nameof(Check));
                }
            }

            private void Changed(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                if (propertyName == nameof(Check) && _loaded && !_isLocked)
                {
                    User us = (User)_usrCV.CurrentItem;
                    switch (Type)
                    {
                        case "ROLE":
                            Debug.WriteLine("Change ROLE");
                            if (_check)
                            {
                                UserRole ur = Dbctx.UserRoles.FirstOrDefault(x => x.RoleId == Id && x.UserIdent == us.UserIdent);
                                if (ur == null)
                                {
                                    Dbctx.UserRoles.Add(new UserRole() { RoleId = Id, UserIdent = us.UserIdent });
                                    Debug.WriteLine("Added ROLE");
                                }
                            }
                            else
                            {
                                UserRole ur = Dbctx.UserRoles.FirstOrDefault(x => x.RoleId == Id && x.UserIdent == us.UserIdent);
                                if (ur != null) Dbctx.UserRoles.Remove(ur);
                            }
                            break;

                        case "WORKAREA":
                            Debug.WriteLine("Change WORKAREA");
                            if (_check)
                            {
                                UserWorkArea uw = Dbctx.UserWorkAreas.FirstOrDefault(x => x.WorkAreaId == Id && x.UserId == us.UserIdent);
                                if (uw == null)
                                {
                                    Dbctx.UserWorkAreas.Add(new UserWorkArea() { WorkAreaId = Id, UserId = us.UserIdent });
                                    Debug.WriteLine("Added WORKAREA");
                                }
                            }
                            else
                            {
                                UserWorkArea uw = Dbctx.UserWorkAreas.FirstOrDefault(x => x.WorkAreaId == Id && x.UserId == us.UserIdent);
                                if (uw != null) us.UserWorkAreas.Remove(uw);
                            }
                            break;

                        case "COST":
                            Debug.WriteLine("Cahnge COST");
                            if (_check)
                            {
                                UserCost uw = Dbctx.UserCosts.FirstOrDefault(x => x.CostId == Id && x.UsrIdent == us.UserIdent);
                                if (uw == null)
                                {
                                    Dbctx.UserCosts.Add(new UserCost() { CostId = Id, UsrIdent = us.UserIdent });
                                    Debug.WriteLine("Added COST");
                                }
                            }
                            else
                            {
                                UserCost uw = Dbctx.UserCosts.FirstOrDefault(x => x.CostId == Id && x.UsrIdent == us.UserIdent);
                                if (uw != null) Dbctx.UserCosts.Remove(uw);
                            }
                            break;
                    }
                    Dbctx.ChangeTracker.DetectChanges();
                }
                
            }
        }
    }
    public class UserNameValidationRule:ValidationRule
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
