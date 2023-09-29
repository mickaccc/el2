using Microsoft.EntityFrameworkCore;
using El2Utilities.Models;
using System;
using System.Linq;
using System.Windows;

namespace El2Utilities.Utils

{
    public static class AppStatic
    {
        public static User? User { get; private set; }
        public static String PC { get; private set; }
        private static readonly DB_COS_LIEFERLISTE_SQLContext _db = new();

        static AppStatic()
        {
            //User ??= new User();
            PC = Environment.MachineName;
            LoadData();
            
        }

        private static void LoadData()
        {

            try
            {
                
                string us = Environment.UserName;
                var q = _db.User
                    .Include(x => x.WorkArea)
                    .Include(x => x.Role)
                    .ThenInclude(x => x.PermissionRoles)
                    .ThenInclude(x => x.PermissionKeyNavigation)
                    .Include(x => x.Cost)
                    .FirstOrDefault(x => x.UserIdent == us);

                User = new()
                {
                    UserIdent = us,
                    UsrEmail = q.UsrEmail,
                    UsrGroup = q.UsrGroup,
                    UsrName = q.UsrName,
                    UsrInfo = q.UsrInfo,
                    UsrRegion = q.UsrRegion,
                    Cost = q.Cost,
                    Role = q.Role,
                    Personalnumber = q.Personalnumber,
                    Exited = q.Exited,
                    WorkArea = q.WorkArea                  
                };
            }
            catch (Exception e)
            {
                string usr = "null";
                if (User != null) { usr = User.UserIdent; }
                
                MessageBox.Show(String.Format("Error {0} {1}", e.Message, usr));
            }

        }
    }
}
