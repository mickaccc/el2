using El2Utilities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Linq;
using System.Windows;
using Prism.Modularity;
using Prism.Ioc;

namespace El2Utilities.Utils

{
    public class AppStatic : IModule, IGlobals
    {
        public static User User { get; private set; }
        public static String PC { get; private set; }
        private static DB_COS_LIEFERLISTE_SQLContext _context;
        public AppStatic()
        {
        
            PC = Environment.MachineName;
            LoadData();
            User ??= new();
        }
        private static void LoadData()
        {

            if (_context == null)
            {
                MessageBox.Show("ERROR Connection not initialed!","ERRER",MessageBoxButton.OK);
                return;
            }
                //user = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                string us = Environment.UserName;
            using (var db = _context)
            {
                var u = db.Users
                    .Include(x => x.UserCosts)
                    .ThenInclude(x => x.Cost)
                    .Include(x => x.UserWorkAreas)
                    .ThenInclude(x => x.WorkArea)
                    .Include(x => x.UserRoles)
                    .ThenInclude(x => x.Role)
                    .ThenInclude(x => x.PermissionRoles)
                    .ThenInclude(x => x.PermissionKeyNavigation)
                    .Single(x => x.UserIdent == us);

                User = (User)u;
            }
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            containerProvider.Resolve<AppStatic>();
        }
    }
}
