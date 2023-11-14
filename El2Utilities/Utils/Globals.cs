using El2Core.Models;
using Microsoft.EntityFrameworkCore;
using Prism.Ioc;
using System;
using System.Linq;

namespace El2Core.Utils

{
    public class Globals : IGlobals, IDisposable
    {
        public User User { get; private set; }
        public string PC { get; private set; }
        private IContainerProvider _container;
        public Globals(IContainerProvider container)
        {
            _container = container;
            PC = Environment.MachineName;
            LoadData();
            User ??= new();
        }
        private void LoadData()
        {

            //user = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            string us = Environment.UserName;

            using (var db = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>())
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

                User = u;
            }

        }

        public void Dispose()
        {

        }
    }
}
