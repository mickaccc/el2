using El2Core.Utils;
using El2Core.Models;
using GongSolutions.Wpf.DragDrop;
using Lieferliste_WPF.Commands;
using Lieferliste_WPF.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using WpfCustomControlLibrary;
using El2Core.ViewModelBase;
using Prism.Ioc;

namespace Lieferliste_WPF.ViewModels
{
    [System.Runtime.Versioning.SupportedOSPlatform("windows7.0")]
    public class UserEditViewModel : ViewModelBase, IDropTarget
    {

        public string Title { get; } = "User Zuteilung";
        public ICommand SelectionChangedCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }
        public ICommand NewCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        public ICommand CloseCommand { get; private set; }
        public ICommand LostFocusCommand { get; private set; }


        private static ICollectionView _usrCV;

        public ICollectionView RoleView { get { return roleSource.View; } }
        public ICollectionView WorkView { get { return workSource.View; } }
        public ICollectionView CostView { get { return costSource.View; } }

        private static bool _isNew = false;
        public static ObservableCollection<User>? Users { get; private set; }
        private static HashSet<Role> Roles { get; set; } = new();
        private static HashSet<WorkArea> WorkAreas { get; set; } = new();
        private static HashSet<Costunit> CostUnits { get; set; } = new();
        private IContainerExtension _container;
        internal CollectionViewSource roleSource { get; private set; } = new();
        internal CollectionViewSource workSource { get; private set; } = new();
        internal CollectionViewSource costSource { get; private set; } = new();
        private int _validName = 0;
        private int _validIdent = 0;
        public int ValidName
        {
            get { return _validName; }
            set
            {
                _validName = value;
                NotifyPropertyChanged(() => ValidName);
            }
        }
        public int ValidIdent
        {
            get { return _validIdent; }
            set
            {
                _validIdent = value;
                NotifyPropertyChanged(() => ValidIdent);
            }
        }

        public UserEditViewModel(IContainerExtension container)
        {
            _container = container;

            LoadData();
            _usrCV = CollectionViewSource.GetDefaultView(Users);

            SelectionChangedCommand = new ActionCommand(OnSelectionChangeExecuted, OnSelectionChangeCanExecute);
            LostFocusCommand = new ActionCommand(OnLostFocusExecuted, OnLostFocusCanExecute);
            SaveCommand = new ActionCommand(OnSaveExecuted, OnSaveCanExecute);
            NewCommand = new ActionCommand(OnNewExecuted, OnNewCanExecute);
            DeleteCommand = new ActionCommand(OnDeleteExecuted, OnDeleteCanExecute);
            CloseCommand = new ActionCommand(OnCloseExecuted, OnCloseCanExecute);

            roleSource.Source = Roles;
            workSource.Source = WorkAreas;
            costSource.Source = CostUnits;

            roleSource.Filter += RoleFilterPredicate;
            workSource.Filter += WorkAreaFilterPredicate;
            costSource.Filter += CostFilterPredicate;
          
        }

        private bool OnLostFocusCanExecute(object arg)
        {
            return true;
        }

        private void OnLostFocusExecuted(object obj)
        {
            if(obj is HintTextBox textBox)
            {

                if (textBox.Name == "U01")
                {
                    if (String.IsNullOrWhiteSpace(textBox.Text))
                    {
                        ValidName = 1;
                    }
                    else { ValidName = 0; }
                }
                if (textBox.Name == "U02")
                {
                    if(String.IsNullOrWhiteSpace(textBox.Text))
                    {
                        ValidIdent = 1;
                    }
                    else{ ValidIdent = 0;}
                }   
            }
        }

        private void CostFilterPredicate(object sender, FilterEventArgs e)
        {
            var cost = e.Item as Costunit;
            if (_usrCV.CurrentItem is User us)
            {
                e.Accepted = !us.UserCosts.Any(x => x.CostId == cost.CostunitId);
            }
        }

        private void WorkAreaFilterPredicate(object sender, FilterEventArgs e)
        {
            var work = e.Item as WorkArea;
            if (_usrCV.CurrentItem is User us)
            {
                e.Accepted = !us.UserWorkAreas.Any(x => x.WorkAreaId == work.WorkAreaId);
            }
        }

        private void RoleFilterPredicate(object sender, FilterEventArgs e)
        {
            var role = e.Item as Role;
            if (_usrCV.CurrentItem is User us)
            {
                e.Accepted = !us.UserRoles.Any(x => x.RoleId == role.Id);
            }
        }


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
                using (var Dbctx = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>())
                {
                    if (Dbctx.ChangeTracker.HasChanges())
                    {
                        MessageBoxResult result = MessageBox.Show("Wollen Sie die Änderungen noch Speichern?", "Datenbank Speichern"
                            , MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (result == MessageBoxResult.Yes) { Dbctx.SaveChangesAsync(); }

                    }
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
                using (var Dbctx = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>())
                {
                    Users?.Remove(usr);
                    Dbctx.Users.Remove(usr);
                    OnSaveExecuted(usr);
                }
            }
        }

        private static bool OnNewCanExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission("UM01") && !_isNew;
        }

        private void OnNewExecuted(object obj)
        {
            User u = new();
            Users?.Add(u);
            
            _usrCV.MoveCurrentTo(u);
            RoleView.Refresh();
            WorkView.Refresh();
            CostView.Refresh();
            _isNew = true;
        }

        private void OnSaveExecuted(object obj)
        {
            if (_isNew)
            {
                using (var Dbctx = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>())
                {
                    var user = Users.Except(Dbctx.Users).FirstOrDefault();
                    if (user != null)
                    {
                        if (user.UsrName.IsNullOrEmpty()) ValidName = 1;
                        if (user.UserIdent.IsNullOrEmpty() || Dbctx.Users.Any(x => x.UserIdent == user.UserIdent)) ValidIdent = 1;
                        if (ValidName == 1 || ValidIdent == 1)
                        {
                            MessageBox.Show("Speichern wegen ungültigen Daten nicht möglich", "Speicherfehler",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                        Dbctx.Users.Add((User)user);
                    }
                }
                using (var Dbctx = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>())
                {
                    Dbctx.SaveChangesAsync();
                    _isNew = false;
                    ValidIdent = 0;
                    ValidName = 0;
                }
            }
        }

        private bool OnSaveCanExecute(object arg)
        {
            using var Dbctx = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
                return Dbctx.ChangeTracker.HasChanges() || _isNew;
        }


        private static bool OnSelectionChangeCanExecute(object arg)
        {
            return true;
        }

        private void OnSelectionChangeExecuted(object obj)
        {
            RoleView.Refresh();
            WorkView.Refresh();
            CostView.Refresh();   
        }

        private void LoadData()
        {
            using var Dbctx = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
            Users = Dbctx.Users
                .Include(x => x.UserWorkAreas)
                .Include(y => y.UserRoles)
                .ThenInclude(c => c.Role)
                .Include(x => x.UserCosts)
                .ThenInclude(x => x.Cost)
                .Where(x => x.Exited == false)
                .OrderBy(o => o.UsrName)
                .ToObservableCollection();


            Roles = Dbctx.Roles.ToHashSet();
            WorkAreas = Dbctx.WorkAreas.ToHashSet();
            CostUnits = Dbctx.Costunits.ToHashSet();
        }

        public void DragOver(IDropInfo dropInfo)
        {
            bool allowed = false;
            if (!dropInfo.TargetCollection.Equals(dropInfo.DragInfo.SourceCollection))
            {
                if (dropInfo.VisualTarget.GetValue(FrameworkElement.NameProperty) is string UiName)
                {
                    if ((dropInfo.Data.GetType() == typeof(Role)
                        || dropInfo.Data.GetType() == typeof(UserRole))
                        && UiName.Contains("ROLE"))
                    {
                        allowed = true;
                    }
                    if ((dropInfo.Data.GetType() == typeof(WorkArea)
                        || dropInfo.Data.GetType() == typeof(UserWorkArea))
                        && UiName.Contains("WORKAREA"))
                    {
                        allowed = true;
                    }
                    if ((dropInfo.Data.GetType() == typeof(Costunit)
                        || dropInfo.Data.GetType() == typeof(UserCost))
                        && UiName.Contains("COSTUNIT"))
                    {
                        allowed = true;
                    }
                    if (allowed)
                    {
                        dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                        dropInfo.Effects = DragDropEffects.Copy;
                    }
                }
            }
        }

        public void Drop(IDropInfo dropInfo)
        {
            User us = (User)_usrCV.CurrentItem;
            Object? data = null;

            if (dropInfo.Data is Role r)
            {
                data = new UserRole() { RoleId = r.Id, UserIdent = us.UserIdent, Role = r };
                us.UserRoles.Add((UserRole)data);
                RaisePropertyChanged(nameof(us.UserRoles));
                RoleView.Refresh();
                
            }
            if (dropInfo.Data is WorkArea w)
            {
                data = new UserWorkArea() { WorkAreaId = w.WorkAreaId, UserId = us.UserIdent, WorkArea = w };
                us.UserWorkAreas.Add((UserWorkArea)data);
                WorkView.Refresh();
                
            }
            if (dropInfo.Data is Costunit c)
            {
                data = new UserCost() { CostId = c.CostunitId, UsrIdent = us.UserIdent, Cost = c };
                us.UserCosts.Add((UserCost)data);
                CostView.Refresh();

            }
            if (dropInfo.VisualTarget.GetValue(FrameworkElement.NameProperty) is string UiName && data == null)
            {
                if(UiName.Contains("ROLE"))
                {
                    us.UserRoles.Remove((UserRole)dropInfo.Data);
                    RoleView.Refresh();
                }
                if (UiName.Contains("WORKAREA"))
                {
                    us.UserWorkAreas.Remove((UserWorkArea)dropInfo.Data);
                    WorkView.Refresh();
                }
                if (UiName.Contains("COSTUNIT"))
                {
                    us.UserCosts.Remove((UserCost)dropInfo.Data);
                    CostView.Refresh();
                }
            }
            _usrCV.Refresh();
        }

    }
}
