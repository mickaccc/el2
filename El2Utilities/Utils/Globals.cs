using El2Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Linq;
using System.Windows;
using Prism.Modularity;
using Prism.Ioc;

namespace El2Core.Utils

{
    public class Globals : IGlobals, IDisposable
    {
        public User User { get; private set; }
        public string PC { get; private set; }
        private DB_COS_LIEFERLISTE_SQLContext _context;
        public Globals(DB_COS_LIEFERLISTE_SQLContext context)
        {

            _context = context;
            PC = Environment.MachineName;
            LoadData();
            User ??= new();
        }
        private void LoadData()
        {

            if (_context == null)
            {
                MessageBox.Show("ERROR Connection not initialed!", "ERRER", MessageBoxButton.OK);
                return;
            }
            //user = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            string us = Environment.UserName;

            var u = _context.Users
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

        public void Dispose()
        {

        }
    }
}
