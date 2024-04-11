using El2Core.Models;
using Microsoft.EntityFrameworkCore;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace El2Core.Utils

{
    public interface IGlobals
    {
        static string PC { get; }
        static User User { get; }
    }
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
                Rules = db.Rules.ToList(); 
            }
        }

        public void Dispose()
        {

        }
    }
    public readonly struct UserInfo
    {
        public static string? PC => _PC ?? string.Empty;
        public static User User => _User;

        private static string? _PC;
        private static User _User;

        public void Initialize(string PC, User Usr)
        {
            _PC = PC;
            _User = Usr;
        }
    }
    public readonly struct RuleInfo
    {
        public static ImmutableDictionary<string, Rule> Rules { get; private set; } = ImmutableDictionary<string, Rule>.Empty;
        public RuleInfo(List<Rule> rules)
        {
            Rules = rules.ToImmutableDictionary(x => x.RuleName.Trim(), x => x);   
        }
    }
}
