using El2Utilities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;

namespace El2Utilities.Utils

{
    public static class AppStatic
    {
        public static User User { get; private set; }
        public static String PC { get; private set; }

        static AppStatic()
        {
            
            PC = Environment.MachineName;
            LoadData();
            User ??= new();
        }

        private static void LoadData()
        {


                //user = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                string us = Environment.UserName;
                using var db = new DB_COS_LIEFERLISTE_SQLContext();

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
}
