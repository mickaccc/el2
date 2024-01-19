using El2Core.Constants;
using El2Core.Models;
using El2Core.Services;
using El2Core.Utils;
using El2Core.ViewModelBase;
using GongSolutions.Wpf.DragDrop;
using Lieferliste_WPF.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Lieferliste_WPF.ViewModels
{
    [System.Runtime.Versioning.SupportedOSPlatform("windows7.0")]
    public class UserEditViewModel : ViewModelBase, IDropTarget, IViewModel
    {

        public string Title { get; } = "User Zuteilung";
        public ICommand SelectionChangedCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }
        public ICommand NewCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        public ICommand CloseCommand { get; private set; }
        public ICommand LostFocusCommand => _LostFocusCommand ??= new RelayCommand(OnLostFocus);

        private RelayCommand _LostFocusCommand;
        private static ICollectionView _usrCV;

        public ICollectionView RoleView { get { return roleSource.View; } }
        public ICollectionView WorkView { get { return workSource.View; } }
        public ICollectionView CostView { get { return costSource.View; } }

        private static bool _isNew = false;
        public static BindingList<User>? Users { get; private set; }

        private static HashSet<Role> Roles { get; set; } = new();
        private static HashSet<WorkArea> WorkAreas { get; set; } = new();
        private static HashSet<Costunit> CostUnits { get; set; } = new();
        private IContainerExtension _container;
        private readonly IUserSettingsService _SettingsService;
        internal CollectionViewSource roleSource { get; } = new();
        internal CollectionViewSource workSource { get; } = new();
        internal CollectionViewSource costSource { get; } = new();

        private DB_COS_LIEFERLISTE_SQLContext _dbctx;
        private int _validName = 1;
        private int _validIdent = 1;


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

        public bool HasChange => _dbctx.ChangeTracker.HasChanges();

        public UserEditViewModel(IContainerExtension container, IUserSettingsService settingsService)
        {
            _container = container;
            _dbctx = container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
            _SettingsService = settingsService;
            LoadData();
            _usrCV = CollectionViewSource.GetDefaultView(Users);
            _usrCV.MoveCurrentToFirst();

            SelectionChangedCommand = new ActionCommand(OnSelectionChangeExecuted, OnSelectionChangeCanExecute);
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



        private void OnLostFocus(object obj)
        {
            if (obj is TextBox textBox)
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
                    if (String.IsNullOrWhiteSpace(textBox.Text))
                    {
                        ValidIdent = 1;
                    }
                    else { ValidIdent = 0; }
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


        private void OnCloseExecuted(object obj)
        {

            _dbctx.ChangeTracker.DetectChanges();
            if (_dbctx.ChangeTracker.HasChanges())
            {
                MessageBoxResult result = MessageBox.Show("Wollen Sie die Änderungen noch Speichern?", "Datenbank Speichern"
                    , MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes) { _dbctx.SaveChangesAsync(); }

            }

        }

        private bool OnCloseCanExecute(object arg)
        {
            return true;
        }

        private bool OnDeleteCanExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission(Permissions.UserDelete);
        }

        private void OnDeleteExecuted(object obj)
        {
            var u = _usrCV.CurrentItem;

            if (u is User usr)
            {

                Users?.Remove(usr);
                _dbctx.Users.Remove(usr);
                OnSaveExecuted(usr);

            }
        }

        private static bool OnNewCanExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission(Permissions.UserNew) && !_isNew;
        }

        private void OnNewExecuted(object obj)
        {
            var u = Users?.AddNew();

            _usrCV.MoveCurrentTo(u);
            RoleView.Refresh();
            WorkView.Refresh();
            CostView.Refresh();
            _isNew = true;
        }

        private void OnSaveExecuted(object obj)
        {
            try
            {
                if (_isNew)
                {

                    var user = Users.Except(_dbctx.Users).FirstOrDefault();
                    if (user != null)
                    {
                        if (user.UsrName.IsNullOrEmpty()) ValidName = 1;
                        if (user.UserIdent.IsNullOrEmpty() || _dbctx.Users.Any(x => x.UserIdent == user.UserIdent)) ValidIdent = 1;
                        if (ValidName == 1 || ValidIdent == 1)
                        {
                            MessageBox.Show("Speichern wegen ungültigen Daten nicht möglich", "Speicherfehler",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                        _dbctx.Users.Add((User)user);
                    }

                    _isNew = false;
                    ValidIdent = 0;
                    ValidName = 0;

                }
                _dbctx.SaveChangesAsync();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "!ERROR!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool OnSaveCanExecute(object arg)
        {
            _dbctx.ChangeTracker.DetectChanges();
            return _dbctx.ChangeTracker.HasChanges() || ((ValidName == 0 && ValidIdent == 0) && _isNew);
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
            Users = new();
            var u = _dbctx.Users
                .Include(x => x.UserWorkAreas)
                .Include(y => y.UserRoles)
                .ThenInclude(c => c.Role)
                .Include(x => x.UserCosts)
                .ThenInclude(x => x.Cost)
                .Where(x => x.Exited == false)
                .OrderBy(o => o.UsrName)
                .ToList();
            foreach (var user in u)
            {
                User user1 = user;
                user1.UserCosts = user.UserCosts.ToObservableCollection();
                user1.UserRoles = user.UserRoles.ToObservableCollection();
                user1.UserWorkAreas = user.UserWorkAreas.ToObservableCollection();
                Users.Add(user1);
            }

            var rl = UserInfo.User.UserRoles.Max(x => x.Role.Rolelevel);
            Roles = _dbctx.Roles.Where(x => x.Rolelevel <= rl).ToHashSet();
            WorkAreas = _dbctx.WorkAreas.ToHashSet();
            CostUnits = _dbctx.Costunits.ToHashSet();
        }

        public void DragOver(IDropInfo dropInfo)
        {
            if (PermissionsProvider.GetInstance().GetUserPermission(Permissions.UserDrop))
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
        }

        public void Drop(IDropInfo dropInfo)
        {
            User us = (User)_usrCV.CurrentItem;
            Object? data = null;

            if (dropInfo.Data is Role r)
            {
                data = new UserRole() { RoleId = r.Id, UserIdent = us.UserIdent, Role = r };
                us.UserRoles.Add((UserRole)data);
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
                if (UiName.Contains("ROLE"))
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

        public void Closing()
        {
            if (_dbctx.ChangeTracker.HasChanges())
            {
                if (_SettingsService.IsSaveMessage)
                {
                    var result = MessageBox.Show(string.Format("Sollen die Änderungen in {0} gespeichert werden?", Title),
                        Title, MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        _dbctx.SaveChanges();
                    }
                }
                else _dbctx.SaveChanges();
            }
        }
    }
}
