using El2Core.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace El2Core.Utils
{

    public sealed class PermissionsProvider
    {
        private static PermissionsProvider? _instance;

        private HashSet<string> _permissions = new();


        public static PermissionsProvider GetInstance()
        {
            if (_instance == null)
            {

                _instance = new PermissionsProvider();
                User u = UserInfo.User;
                _instance.LoadPermissions(u);
                return _instance;
            }
            return _instance;
        }

        private PermissionsProvider() { }

        private void LoadPermissions(User user)
        {

            foreach (var item in user.UserRoles)
            {
                foreach (var permission in item.Role.PermissionRoles)
                {
                    _permissions.Add(permission.PermissionKey.Trim());
                }
            }
        }

        public bool GetUserPermission(string permission)
        {
            return _permissions.Contains(permission);
        }


    }
}
