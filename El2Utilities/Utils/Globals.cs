using El2Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.IO;
using System.Linq;
using Rule = El2Core.Models.Rule;

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

                //FormattableString sql = $"SELECT * FROM dbo.idm_accounts a INNER JOIN dbo.idm_relations b ON a.account_id = b.account_id WHERE a.account_id = {us}";
                //var sq = db.IdmAccounts.FromSql(sql).ToList();
                var test = db.IdmAccounts.Join(db.IdmRelations,
                    account => account.AccountId,
                    relation => relation.AccountId,
                    (acc, rel) => new { Accoun = acc, Rela = rel })
                    .Where(x => x.Accoun.AccountId == us);

                var result = from account in db.IdmAccounts
                             join r in db.IdmRelations on account.AccountId equals r.AccountId
                             join role in db.IdmRoles on r.RoleId equals role.RoleId
                             join perm in db.RolePermissions on role.RoleId equals perm.RoleId
                             where account.AccountId == us
                             select new { account, perm };
                    

                if (idm.Count == 0) throw new KeyNotFoundException("User nicht gefunden");
                UserInfo userInfo = new UserInfo();
                var usr = result.First().account;
                var user = new User(usr.AccountId, usr.Firstname, usr.Lastname, usr.Email);
                foreach (var r in result)
                {
                    user.Permissions.Add(r.perm.PermissKey.Trim());                  
                }

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
    public class User(string id, string? firstName, string? lastname, string? email)
    {
        public string? FirstName { get; } = firstName;
        public string? LastName { get; } = lastname;
        public string? Email { get; } = email;
        public string UserId { get; } = id;
        public HashSet<string> Permissions { get; } = new HashSet<string>();
        public List<Costunit>? CostUnits { get; set; }
        public List<WorkArea>? WorkAreas { get; set; }

    }
}
