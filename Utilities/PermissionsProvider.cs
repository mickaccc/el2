using Lieferliste_WPF.Data;
using System.Data;

namespace Lieferliste_WPF.Utilities
{

    public sealed class PermissionsProvider
    {
        private static PermissionsProvider? _instance;

        private static HashSet<string> _permissions = new();
        

        public static PermissionsProvider GetInstance()
        {
            if (_instance == null)
            {
                //user = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                string user = Environment.UserName;
                _instance = new PermissionsProvider();

               LoadPermissions(user);
                return _instance;
            }
            return _instance;
        }

        private PermissionsProvider(){ }

        private static void LoadPermissions(string user)
        {
            using DataContext db = new();
            var query = (from p in db.PermissionRoles
                         join r in db.Roles on p.RoleKey equals r.Id
                         join ur in db.UserRoles on r.Id equals ur.RoleId
                         where ur.UserIdent == user
                         select p.PermissionKey.Trim());

            foreach (var item in query)
            {
                _permissions.Add(item);
            }
        }

        public static bool GetUserPermission(string permission)
        {
            return _permissions.Contains(permission);
        }


    }
}
