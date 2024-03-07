using El2Core.Models;
using Microsoft.EntityFrameworkCore;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;

namespace El2Core.Utils

{
    public class Globals : IGlobals, IDisposable
    {
        public User User { get; private set; }
        public string PC { get; private set; }
        public List<Rule> Rules { get; private set; }
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

                Rules ??= new();
                foreach (var rule in db.Rules)
                { 
                    var r = new Rule() { RuleId = rule.RuleId, RuleName = rule.RuleName.Trim(), RuleValue = rule.RuleValue };
                    Rules.Add(r);
                }
            }
        }

        public void Dispose()
        {

        }
    }
}
