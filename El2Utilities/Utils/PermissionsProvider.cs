using El2Core.Models;
using System.Collections.Generic;

namespace El2Core.Utils
{

    public sealed class PermissionsProvider
    {
        private static PermissionsProvider? _instance;

        private HashSet<string> _permissions = new();
        private HashSet<int> _fullAccesses = new();

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
            foreach (var access in user.UserWorkAreas)
            {
                if (access.FullAccess) _fullAccesses.Add(access.WorkAreaId);
            }
        }

        public bool GetUserPermission(string permission)
        {
            return _permissions.Contains(permission);
        }
        public bool GetRelativeUserPermission(string permission, int workAreaId)
        {
            if(_permissions.Contains(permission))
            {
                return _fullAccesses.Contains(workAreaId);
            }
            return false;
        }

    }
}
