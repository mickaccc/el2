using El2Core.Constants;
using El2Core.Models;
using El2Core.Utils;
using El2Core.ViewModelBase;
using GongSolutions.Wpf.DragDrop;
using Microsoft.EntityFrameworkCore;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Lieferliste_WPF.ViewModels
{
    [System.Runtime.Versioning.SupportedOSPlatform("windows7.0")]
    public class RoleEditViewModel : ViewModelBase, IDropTarget
    {
        public string Title { get; } = "Rollen Zuteilung";
        public ICommand SelectionChangedCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }
        public ICommand SyncCommand { get; private set; }
        public ICommand CloseCommand { get; private set; }

        private static ICollectionView _roleCV;
        private static bool _hasChanges = false;
        private List<string> dbPermiss = [];
        private List<string> appPermiss = [];
        private readonly IContainerExtension _container;
        private DB_COS_LIEFERLISTE_SQLContext _Dbctx;
        public static ObservableCollection<IdmRole>? Roles { get; } = new();

        public static ICollectionView PermissionsAvail { get; private set; }
        private static readonly List<Permission> _permissionsAll = new();




        public RoleEditViewModel(IContainerExtension container)
        {
            _container = container;
            _Dbctx = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
            LoadData();

            _roleCV = CollectionViewSource.GetDefaultView(Roles);
            PermissionsAvail = CollectionViewSource.GetDefaultView(_permissionsAll);
            PermissionsAvail.Filter += OnPermissionFilter;
            SelectionChangedCommand = new ActionCommand(OnSelectionChangeExecuted, OnSelectionChangeCanExecute);
            SaveCommand = new ActionCommand(OnSaveExecuted, OnSaveCanExecute);
            CloseCommand = new ActionCommand(OnCloseExecuted, OnCloseCanExecute);
            SyncCommand = new ActionCommand(OnSyncExecuted, OnSyncCanExecute);

            _roleCV.MoveCurrentToFirst();

        }


        private bool OnPermissionFilter(object obj)
        {
            if (obj is Permission permission)
            {
                var role = _roleCV.CurrentItem as IdmRole;
                if (role.RolePermissions.Any(x => x.PermissKey == permission.PKey))
                    return false;
            }
            return true;
        }

        private bool OnSyncCanExecute(object arg)
        {
            return dbPermiss.Any() || appPermiss.Any();
        }

        private void OnSyncExecuted(object obj)
        {
            using var db = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
            foreach (var item in dbPermiss)
            {
                var perm = db.Permissions.Single(x => x.PKey == item);
                _permissionsAll.Remove(perm);
                db.Permissions.Remove(perm);
            }
            foreach (var item in appPermiss)
            {
                var perm = new Permission() { PKey = item };
                _permissionsAll.Add(perm);
                db.Permissions.Add(perm);
            }
            db.SaveChanges();
            dbPermiss.Clear();
            appPermiss.Clear();
        }

        private void OnCloseExecuted(object obj)
        {
            if (obj is Window ob)
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

        private void OnSaveExecuted(object obj)
        {
            _Dbctx.SaveChangesAsync();
        }

        private bool OnSaveCanExecute(object arg)
        {
            return _Dbctx.ChangeTracker.HasChanges();
        }

        private static bool OnSelectionChangeCanExecute(object arg)
        {
            return true;
        }

        private static void OnSelectionChangeExecuted(object obj)
        {

            if (obj is IdmRole us)
            {

                PermissionsAvail.Refresh();

            }
        }

        private void LoadData()
        {

            var r = _Dbctx.IdmRoles
                .Include(x => x.RolePermissions)
                .ThenInclude(x => x.PermissKeyNavigation)
                .ToList();
            HashSet<Permission> permissions = new HashSet<Permission>();
            foreach (var role in r)
            {
                Roles.Add(role);

            }

            var p = _Dbctx.Permissions.AsNoTracking();

            _permissionsAll.AddRange(p);
            SyncronDiffs();
        }
        private void SyncronDiffs()
        {
            using var db = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
            bool ret;
            List<string> keys = [];
            FieldInfo[] prop = typeof(Permissions).GetFields();
            foreach (var key in prop)
            {
                if (key.GetValue(typeof(Permissions)) is string k && k[0] != '!')
                    keys.Add(k);
            }

            var per = db.Permissions.Select(x => x.PKey.Trim()).ToList();
            dbPermiss.AddRange(per.Except(keys));
            appPermiss.AddRange(keys.Except(per));

        }
        public void DragOver(IDropInfo dropInfo)
        {
            if (PermissionsProvider.GetInstance().GetUserPermission(Permissions.RoleDrop))
            {
                if (dropInfo.Data is Permission || dropInfo.Data is RolePermission)
                {
                    dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                    dropInfo.Effects = DragDropEffects.Move;
                }
            }
        }

        public void Drop(IDropInfo dropInfo)
        {
            IdmRole? r = _roleCV.CurrentItem as IdmRole;
            if (dropInfo.Data is Permission p)
            {

                if (r != null)
                {
                    r.RolePermissions.Add(new RolePermission() { Created = DateTime.Now, PermissKey = p.PKey, RoleId = r.RoleId });
                    PermissionsAvail.Refresh();
                }
            }
            else if (dropInfo.Data is RolePermission pr)
            {
                if (r != null)
                {
                    r.RolePermissions.Remove(pr);
                    PermissionsAvail.Refresh();
                }
            }
        }
    }
}
