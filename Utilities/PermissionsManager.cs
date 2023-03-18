using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using Lieferliste_WPF.Data;
using Lieferliste_WPF.Data.Models;
using System.Linq;

namespace Lieferliste_WPF.Utilities
{

    public class PermissionsManager
    {
        private static PermissionsManager? _instance;
        private readonly DataContext _db = new();


        private readonly List<String> _permissions;
        private readonly String _user;

        public static PermissionsManager getInstance(String user)
        {
            if (_instance == null)
            {
                _instance = new PermissionsManager(user);
                return _instance;
            }
            return _instance;
        }

        private PermissionsManager(String user)
        {

            _permissions = new List<string>();
            string pattern = @"^EMEA\\";
            Regex reg = new Regex(pattern);
            _user = reg.Replace(user.ToUpper(), "");
            try
            {

                var query = from p in _db.Permissions
                            join pR in _db.PermissionRoles on p.ValidKey equals pR.PermissionKey
                            join r in _db.Roles on pR.RoleKey equals r.Id
                            join usrR in _db.UserRoles on r.Id equals usrR.RoleId
                            join usr in _db.Users on usrR.UserIdent equals usr.UserIdent
                            where usr.UserIdent == _user
                            select new { p.ValidKey };

                //_permissions.AddRange((IEnumerable<string>)query);

            }
            catch (Exception e)
            {

                System.Windows.MessageBox.Show("Fehler Permissionsmanager!\n" + e.Message + "\n" + e.InnerException,
                    "ERROR", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }

        }


        public Boolean getUserPermission(String permission)
        {
            return _permissions.Contains(permission);
        }
        public List<string> getPermissions()
        {
            return _permissions;
        }

    }
}
