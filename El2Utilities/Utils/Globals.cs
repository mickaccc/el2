using El2Core.Models;
using Microsoft.EntityFrameworkCore;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
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
        public string PC { get; }
        public List<Rule> Rules { get; private set; }
        private IContainerProvider _container;
        public Globals(IContainerProvider container)
        {
            _container = container;
            PC = Environment.MachineName;
            LoadData();
            
        }
        public static UserInfo CreateUserInfo(IContainerProvider container)
        {
            //user = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            string us = Environment.UserName;

            using (var db = container.Resolve<DB_COS_LIEFERLISTE_SQLContext>())
            {
                var idm = db.IdmRelations
                    .Include(x => x.Account)
                    .Include(x => x.Role)
                    .Where(x => x.AccountId == us)
                    .ToList();

                if (idm.Count == 0) throw new KeyNotFoundException("User nicht gefunden");
                UserInfo userInfo = new UserInfo();
                List<IdmRole> roles = new List<IdmRole>();
                foreach(var r in idm)
                {
                    roles.Add(r.Role);
                }
                var usr = idm.First().Account;
                var user = new User(usr.AccountId, usr.Firstname, usr.Lastname, usr.Email, roles);

                var acc = db.IdmAccounts
                    .Include(x => x.AccountCosts)
                    .ThenInclude(x => x.Cost)
                    .Include(x => x.AccountWorkAreas)
                    .ThenInclude(x => x.WorkArea)
                    .First(x => x.AccountId == us);
                user.CostUnits = acc.AccountCosts.Select(x => x.Cost).ToList();
                user.WorkAreas = acc.AccountWorkAreas.Select(x => x.WorkArea).ToList();
                userInfo.Initialize(Environment.MachineName, user);

                return userInfo;  
                //Rules = db.Rules.ToList();
            }
        }
        private void LoadData()
        {

            //user = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            string us = Environment.UserName;

            using (var db = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>())
            {
                var u = db.IdmRelations
                    .Include(x => x.Account)
                    .Include(x => x.Role)
                    .ToList();
  
                var rel = u.Where(x => x.AccountId == us);
                var usr = rel.First().Account;
                User = new User(usr.AccountId, usr.Firstname, usr.Lastname, usr.Email, rel.Select(x => x.Role).ToList());
                    
                Rules = db.Rules.ToList(); 
            }
        }
        public void SaveRule(Rule rule)
        {
            using var db = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
            if (db.Rules.All(x => x.RuleValue != rule.RuleValue))
            {
                db.Rules.Add(rule);
            }
            else
            {
                var r = db.Rules.First(x => x.RuleValue == rule.RuleValue);
                r.RuleName = rule.RuleName;
                r.RuleData = rule.RuleData;
            }
            db.SaveChanges();
        }
        public void SaveProjectSchemes(List<ProjectScheme> projectSchemes)
        {

            var serializer = XmlSerializerHelper.GetSerializer(typeof(List<ProjectScheme>));
            StringWriter sw = new StringWriter();
            serializer.Serialize(sw, projectSchemes);

                Rule rule = new Rule();
                rule.RuleName = "ProjectSchema";
                rule.RuleValue = "ProjStruct";
                rule.RuleData = sw.ToString();
            SaveRule(rule);

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
        public static ImmutableDictionary<string, ProjectScheme> ProjectSchemes { get; private set; } = ImmutableDictionary<string, ProjectScheme>.Empty;
        public RuleInfo(List<Rule> rules)
        {
            Rules = rules.ToImmutableDictionary(x => x.RuleValue.Trim(), x => x);
            Dictionary<string, ProjectScheme> sc = new Dictionary<string, ProjectScheme>();
            var scheme = rules.SingleOrDefault(x => x.RuleValue.Trim() == "ProjStruct");
            if (scheme != null)
            {
                if (scheme.RuleData == null) return;
                var serializer = XmlSerializerHelper.GetSerializer(typeof(List<ProjectScheme>));
                TextReader xmlData = new StringReader(scheme.RuleData);
                List<ProjectScheme> projectSchemes = (List<ProjectScheme>)serializer.Deserialize(xmlData);

                foreach(var item in projectSchemes)
                {
                    sc.Add(item.Key, item);
                }
                ProjectSchemes = sc.ToImmutableDictionary();
            }
        }
    }
    public class ProjectScheme
    {
        public string Key { get; set; }
        public string Regex { get; set; }
        public ProjectScheme() { }
        public ProjectScheme(string key, string regex) { Key = key; Regex = regex; }
    }
    public class User(string id, string? firstName, string? lastname, string? email, List<IdmRole>? roles)
    {
        public string? FirstName { get; } = firstName;
        public string? LastName { get; } = lastname;
        public string? Email { get; } = email;
        public string UserId { get; } = id;
        public ImmutableArray<IdmRole>? Roles { get; } = [.. roles];
        public List<Costunit>? CostUnits { get; set; }
        public List<WorkArea>? WorkAreas { get; set; }

    }
}
