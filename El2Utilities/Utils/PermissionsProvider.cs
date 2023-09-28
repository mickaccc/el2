using Statics;
using System;
using System.Collections.Generic;

namespace El2Utilities.Utils
{

    public sealed class PermissionsProvider
    {
        private static PermissionsProvider? _instance;

        private HashSet<string> _permissions = new();
        

        public static PermissionsProvider GetInstance()
        {
            if (_instance == null)
            {
                //user = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                string user = Environment.UserName;
                _instance = new PermissionsProvider();

               _instance.LoadPermissions(user);
                return _instance;
            }
            return _instance;
        }

        private PermissionsProvider(){ }

        private void LoadPermissions(string user)
        {
            

            foreach (var item in AppStatic.User.Role)
            {
                foreach(var permission in item.PermissionRoles)
                {
                    _permissions.Add(permission.PermissionKey);
                }
                
            }
        }

        public bool GetUserPermission(string permission)
        {
            return _permissions.Contains(permission);
        }


    }
}
