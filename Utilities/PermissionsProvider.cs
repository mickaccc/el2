using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Lieferliste_WPF.Data;
using Lieferliste_WPF.Data.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore.Internal;

namespace Lieferliste_WPF.Utilities
{

    public sealed class PermissionsProvider
    {
        private static PermissionsProvider _instance;
        private readonly DataContext _db = new();


        private static readonly List<string> _permissions = new();
        

        public static PermissionsProvider GetInstance()
        {
            if (_instance == null)
            {
                //user = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                string user = Environment.UserName;
                _instance = new PermissionsProvider();
                
                _permissions.AddRange(_instance.GetPermissions(user));
                return _instance;
            }
            return _instance;
        }

        private PermissionsProvider(){ }
        public List<string> GetPermissions()
        {
            return _permissions;
        }
        public List<string> GetPermissions(string user)
        {
            List<string> result = new();
            try
            {
                var query = (from p in _db.PermissionRoles                            
                             join r in _db.Roles on p.RoleKey equals r.Id
                             join ur in _db.UserRoles on r.Id equals ur.RoleId
                             where ur.UserIdent == user                            
                             select p.PermissionKey.Trim()).ToList();


                                  
                result.AddRange((IEnumerable<string>)query);
                return result;
                
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show("Fehler beim laden vom Berechtigungssystem!\n" + e.Message + "\n" + e.InnerException,
                    "ERROR", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return result;
            }
        }

        public bool GetUserPermission(string permission)
        {
            return _permissions.Contains(permission);
        }


    }
}
