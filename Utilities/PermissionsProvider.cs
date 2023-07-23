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

    public sealed class PermissionsProvider : ViewModels.Base.ViewModelBase
    {
        private static PermissionsProvider _instance;
        
        private static readonly List<string> _permissions = new();
        

        public static PermissionsProvider GetInstance()
        {
            if (_instance == null)
            {
                //user = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                string user = Environment.UserName;
                _instance = new PermissionsProvider();
                
                _permissions.AddRange(_instance.LoadPermissions(user));
                return _instance;
            }
            return _instance;
        }

        private PermissionsProvider(){ }
        public List<string> GetPermissions()
        {
            return _permissions;
        }
        private List<string> LoadPermissions(string user)
        {
            List<string> result = new();
            try
            {
                var query = (from p in Dbctx.PermissionRoles                            
                             join r in Dbctx.Roles on p.RoleKey equals r.Id
                             join ur in Dbctx.UserRoles on r.Id equals ur.RoleId
                             where ur.UserIdent == user                            
                             select p.PermissionKey.Trim()).ToList();

                                  
                result.AddRange((IEnumerable<string>)query);
                           
            }
            catch (NullReferenceException e)
            {
                System.Windows.MessageBox.Show("Fehler beim laden vom Berechtigungssystem!\n" + e.Message + "\n" + e.InnerException,
                   "ERROR", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
            catch (Exception)
            {
                 
            }
            return result;
        }

        public bool GetUserPermission(string permission)
        {
            return _permissions.Contains(permission);
        }


    }
}
