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
    [System.Runtime.Versioning.SupportedOSPlatform("windows10.0")]
    public class UserEditViewModel : ViewModelBase, IDropTarget, IViewModel
    {

        public string Title { get; } = "User Zuteilung";
        public ICommand SelectionChangedCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }
        public ICommand CloseCommand { get; private set; }
        public ICommand LostFocusCommand => _LostFocusCommand ??= new RelayCommand(OnLostFocus);

        private RelayCommand? _LostFocusCommand;
        private static ICollectionView? _usrCV;

        public ICollectionView RoleView { get { return roleSource.View; } }
        public ICollectionView WorkView { get { return workSource.View; } }
        public ICollectionView CostView { get { return costSource.View; } }

        private static bool _isNew = false;
        public static BindingList<User>? Users { get; private set; }

        private static HashSet<IdmRole> Roles { get; set; } = new();
        private static List<WorkArea> WorkAreas { get; set; }
        private static List<Costunit> CostUnits { get; set; }
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
            //var cost = e.Item as Costunit;
            //if (_usrCV?.CurrentItem is User us)
            //{
            //    e.Accepted = !us.CostUnits.Any(x => x.CostId == cost?.CostunitId);
            //}
        }

        private void WorkAreaFilterPredicate(object sender, FilterEventArgs e)
        {
            var work = e.Item as WorkArea;
            if (_usrCV?.CurrentItem is User us)
            {
                e.Accepted = !us.WorkAreas.Any(x => x.WorkAreaId == work?.WorkAreaId);
            }
        }

        private void RoleFilterPredicate(object sender, FilterEventArgs e)
        {
            //var role = e.Item as Role;
            //if (_usrCV?.CurrentItem is User us)
            //{
            //    e.Accepted = !us.UserRoles.Any(x => x.RoleId == role?.Id);
            //}
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
  

        private void OnSaveExecuted(object obj)
        {
            try
            {
 
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
            var u = _dbctx.IdmAccounts
                .Include(x => x.AccountWorkAreas)
                .ThenInclude(x => x.WorkArea)
                .Include(x => x.AccountCosts)
                .ThenInclude(x => x.Cost)
                .OrderBy(o => o.AccountId)
                .ToList();
            var us = (from role in _dbctx.IdmRoles
                      join r in _dbctx.IdmRelations on role.RoleId equals r.RoleId
                      join a in _dbctx.IdmAccounts on r.AccountId equals a.AccountId
                      select new { role, a });
            foreach (var user in u)
            {
     
                User user1 = new User(user.AccountId, user.Firstname, user.Lastname, user.Email);
                user1.WorkAreas = user.AccountWorkAreas.Select(x => x.WorkArea).ToList();
                user1.CostUnits = user.AccountCosts.Select(x => x.Cost).ToList();

                foreach (var r in us.AsEnumerable().Where(x => x.a.AccountId == user.AccountId))
                {
                    user1.Roles.Add(r.role.RoleName);
                }

                Users.Add(user1);
            }

            WorkAreas = [.. _dbctx.WorkAreas.OrderBy(x => x.Bereich)];
            CostUnits = [.. _dbctx.Costunits.OrderBy(x => x.CostunitId)];
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

                        if (dropInfo.Data.GetType() == typeof(WorkArea)
                            && UiName.Contains("WORKAREA"))
                        {
                            allowed = true;
                        }
                        if (dropInfo.Data.GetType() == typeof(Costunit)
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
            if (_usrCV != null)
            {
                User us = (User)_usrCV.CurrentItem;
                Object? data = null;

  
                if (dropInfo.Data is WorkArea w)
                {
                    data = new AccountWorkArea() { WorkAreaId = w.WorkAreaId, AccountId = us.UserId, WorkArea = w };
                    us.WorkAreas.Add((WorkArea)data);
                    WorkView.Refresh();

                }
                if (dropInfo.Data is Costunit c)
                {
                    data = new AccountCost() { CostId = c.CostunitId, AccountId = us.UserId, Cost = c };
                    us.CostUnits.Add((Costunit)data);
                    CostView.Refresh();

                }
                if (dropInfo.VisualTarget.GetValue(FrameworkElement.NameProperty) is string UiName && data == null)
                {
 
                    if (UiName.Contains("WORKAREA"))
                    {
                        us.WorkAreas.Remove((WorkArea)dropInfo.Data);
                        WorkView.Refresh();
                    }
                    if (UiName.Contains("COSTUNIT"))
                    {
                        us.CostUnits.Remove((Costunit)dropInfo.Data);
                        CostView.Refresh();
                    }
                }
                _usrCV.Refresh();
            }
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
