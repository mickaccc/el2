using El2Core.Constants;
using El2Core.Models;
using El2Core.Services;
using El2Core.Utils;
using El2Core.ViewModelBase;
using GongSolutions.Wpf.DragDrop;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
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
        ILogger logger;
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
            var factory = _container.Resolve<ILoggerFactory>();
            logger = factory.CreateLogger<UserEditViewModel>();
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


        private void CostFilterPredicate(object sender, FilterEventArgs e)
        {
            var cost = e.Item as Costunit;
            if (_usrCV?.CurrentItem is User us)
            {
                e.Accepted = !us.AccountCostUnits.Any(x => x.CostId == cost?.CostunitId);
            }
        }

        private void WorkAreaFilterPredicate(object sender, FilterEventArgs e)
        {
            var work = e.Item as WorkArea;
            if (_usrCV?.CurrentItem is User us)
            {
                e.Accepted = !us.AccountWorkAreas.Any(x => x.WorkAreaId == work?.WorkAreaId);
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
                logger.LogError("{message}", e.ToString());
            }
        }

        private bool OnSaveCanExecute(object arg)
        {
            _dbctx.ChangeTracker.DetectChanges();
            return _dbctx.ChangeTracker.HasChanges();
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
            try
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
                    user1.AccountWorkAreas = user.AccountWorkAreas.ToList();
                    user1.AccountCostUnits = user.AccountCosts.ToList();

                    foreach (var r in us.AsEnumerable().Where(x => x.a.AccountId == user.AccountId))
                    {
                        user1.Roles.Add(r.role.RoleName);
                    }

                    Users.Add(user1);
                }

                WorkAreas = [.. _dbctx.WorkAreas.OrderBy(x => x.Bereich)];
                CostUnits = [.. _dbctx.Costunits.OrderBy(x => x.CostunitId)];
            }
            catch (Exception e)
            {

                logger.LogError("{message}", e.ToString());
            }
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

                        if ((dropInfo.Data.GetType() == typeof(WorkArea) || dropInfo.Data.GetType() == typeof(AccountWorkArea))
                            && UiName.Contains("WORKAREA"))
                        {
                            allowed = true;
                        }
                        if ((dropInfo.Data.GetType() == typeof(Costunit) || dropInfo.Data.GetType() == typeof(AccountCost))
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


                if (dropInfo.Data is WorkArea w)
                {
                    var data = new AccountWorkArea() { WorkAreaId = w.WorkAreaId, AccountId = us.UserId, WorkArea = w };
                    us.AccountWorkAreas.Add(data);
                    _dbctx.AccountWorkAreas.Add(data);
                    WorkView.Refresh();
                    logger.LogInformation("add Workarea {message}", (WorkArea)dropInfo.Data);
                }
                if (dropInfo.Data is Costunit c)
                {
                    var data = new AccountCost() { CostId = c.CostunitId, AccountId = us.UserId, Cost = c };
                    us.AccountCostUnits.Add(data);
                    _dbctx.AccountCosts.Add(data);
                    CostView.Refresh();
                    logger.LogInformation("add Workarea {message}", (Costunit)dropInfo.Data);
                }
                if (dropInfo.VisualTarget.GetValue(FrameworkElement.NameProperty) is string UiName &&
                    (dropInfo.Data is AccountCost || dropInfo.Data is AccountWorkArea))
                {
 
                    if (UiName.Contains("WORKAREA"))
                    {

                        us.AccountWorkAreas.Remove((AccountWorkArea)dropInfo.Data);
                        _dbctx.AccountWorkAreas.Remove((AccountWorkArea)dropInfo.Data);
                        WorkView.Refresh();
                        logger.LogInformation("remove Workarea {message}", (AccountWorkArea)dropInfo.Data);
                    }
                    if (UiName.Contains("COSTUNIT"))
                    {
                        us.AccountCostUnits.Remove((AccountCost)dropInfo.Data);
                        _dbctx.AccountCosts.Remove((AccountCost)dropInfo.Data);
                        CostView.Refresh();
                        logger.LogInformation("remove Cost {message}", (AccountCost)dropInfo.Data);
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
