using Microsoft.EntityFrameworkCore;
using Statics.Models;

namespace Statics

{
    public static class AppStatic
    {
        public static User User { get; private set; }
        public static String PC { get; private set; }
        private static readonly DB_COS_LIEFERLISTE_SQLContext _db;

        static AppStatic()
        {
            LoadData();
            User ??= new User();
        }

        private static void LoadData()
        {

                PC = Environment.MachineName;
                string us = Environment.UserName;
                var q = _db.User
                    .Include(x => x.WorkArea)
                    .Include(x => x.Role)
                    .ThenInclude(x => x.PermissionRoles)
                    .Include(x => x.Cost)
                    .First(x => x.UserIdent == us);

                User = q;
           
        }
    }
}
