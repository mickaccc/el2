using El2Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

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
                _instance.LoadPermissions(UserInfo.User);
                return _instance;
            }
            return _instance;
        }

        private PermissionsProvider() { }

        private void LoadPermissions(User user)
        {

            foreach (var item in user.Permissions)
            {

                    _permissions.Add(item);
                
            }
            foreach (var access in user.WorkAreas)
            {
                foreach (var wo in access.AccountWorkAreas.Where(x => x.FullAccess))
                {
                     _fullAccesses.Add(wo.WorkAreaId);
                }
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

        public bool GetUserPermission(object delMeasureDocu)
        {
            throw new NotImplementedException();
        }
    }
}
