using El2Core.Models;
using El2Core.ViewModelBase;
using El2Core.Utils;
using GongSolutions.Wpf.DragDrop;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Prism.Regions;
using Prism.Ioc;

namespace ModuleRoleEdit.ViewModels
{
    [System.Runtime.Versioning.SupportedOSPlatform("windows7.0")]
    public class RoleEditViewModel : ViewModelBase, IDropTarget
    {
        public string Title { get; } = "Rollen Zuteilung"; 
        public ICommand SelectionChangedCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }
        public ICommand CloseCommand { get; private set; }

        private static ICollectionView _roleCV;
        private static bool _hasChanges = false;
        private IContainerExtension _container;
        public static ObservableCollection<Role>? Roles { get; private set; } = new();
        
        public static ObservableCollection<Permission> PermissionsAvail { get; private set; } = new();
        public static ObservableCollection<PermissionRole> PermissionsInter { get; private set; } = new();
        private static List<Permission> _permissionsAll = new();
        



        public RoleEditViewModel(IContainerExtension container)
        {
            _container = container;
            LoadData();

            _roleCV = CollectionViewSource.GetDefaultView(Roles);
            SelectionChangedCommand = new ActionCommand(OnSelectionChangeExecuted, OnSelectionChangeCanExecute);
            SaveCommand = new ActionCommand(OnSaveExecuted, OnSaveCanExecute);
            CloseCommand = new ActionCommand(OnCloseExecuted, OnCloseCanExecute);
            
            _roleCV.MoveCurrentToFirst();
            PermissionsInter.CollectionChanged += OnCollectionChanged;

        }

        private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            _hasChanges = true;
        }

        public string Error { get { return null; } }

        public string this[string columnName]
        {
            get
            {
                User us = (User)_roleCV.CurrentItem;
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

        private void OnSaveExecuted(object obj)
        {
            if (_roleCV.CurrentItem is Role role)
            {
                using (var Dbctx = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>())
                {
                    var inserts = PermissionsInter.ExceptBy(role.PermissionRoles.Select(x => x.PermissionKey), y => y.PermissionKey);
                    var removes = role.PermissionRoles.ExceptBy(PermissionsInter.Select(x => x.PermissionKey), y => y.PermissionKey);
                    if (inserts.Any()) Dbctx.PermissionRoles.AddRange(inserts);
                    if (removes.Any()) Dbctx.PermissionRoles.RemoveRange(removes);

                    Dbctx.SaveChanges();
                    _hasChanges = false;
                }
            }
        }

        private bool OnSaveCanExecute(object arg)
        {
            return _hasChanges;
        }


        private static bool OnSelectionChangeCanExecute(object arg)
        {
            return true;
        }

        private static void OnSelectionChangeExecuted(object obj)
        {
            
            if (obj is Role us)
            {
                PermissionsInter.Clear();
                foreach (var p in us.PermissionRoles)
                {
                    PermissionsInter.Add(new PermissionRole()
                    {
                        Created = p.Created,
                        PermissionKey = p.PermissionKey,
                        RoleKey = p.RoleKey,
                    });
   
                }
                PermissionsAvail.Clear();
               
                foreach(var p in _permissionsAll.ExceptBy(PermissionsInter.Select(o => o.PermissionKey), o => o.PKey))
                {

                        PermissionsAvail.Add(new Permission()
                        {
                            PKey = p.PKey,
                            Description = p.Description,
                            Categorie = p.Categorie
                        });
                }
                _hasChanges = false;
                                         
            }         
        }

        private void LoadData()
        {
            using (var Dbctx = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>())
            {
               var r = Dbctx.Roles
                .Include(x => x.PermissionRoles)
                .ThenInclude(x => x.PermissionKeyNavigation)
                .ToList();

                foreach(var role  in r)
                {
                    Roles.Add(role);
                }
                var p = Dbctx.Permissions.ToList();

                _permissionsAll.AddRange(p);
            }
        }

        public void DragOver(IDropInfo dropInfo)
        {
            if (dropInfo.Data is Permission || dropInfo.Data is PermissionRole)
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                dropInfo.Effects = DragDropEffects.Move;
            }
        }

        public void Drop(IDropInfo dropInfo)
        {
            Role? r = _roleCV.CurrentItem as Role;
            if (dropInfo.Data is Permission p)
            {
                
                if (r != null) 
                {
                    PermissionsInter.Add(new PermissionRole() { Created = DateTime.Now, PermissionKey = p.PKey, RoleKey = r.Id });
                    PermissionsAvail.Remove(p);
                }
            }
            else if (dropInfo.Data is PermissionRole pr)
            {
                if (r != null)
                {
                    var p2 = _permissionsAll.Find(x => x.PKey == pr.PermissionKey);
                    PermissionsAvail.Add(p2);
                    PermissionsInter.Remove(pr);
                }
            }
        }
    }
    public class RoleNameValidationRule:ValidationRule
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
