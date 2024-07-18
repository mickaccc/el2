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
        static IdmAccount User { get; }
    }
    public class Globals : IGlobals, IDisposable
    {
        public IdmAccount User { get; private set; }
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
                var u = db.IdmAccounts 
                .Include(x => x.AccountCosts)
                .ThenInclude(x => x.Cost)
                .Include(x => x.AccountWorkAreas)
                .ThenInclude(x => x.WorkArea)
                .Include(y => y.IdmRelations)
                .ThenInclude(y => y.Role)
                .Single(x => x.AccountId == us);
                    
                User = u;
                    
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
        public static IdmAccount User => _User;

        private static string? _PC;
        private static IdmAccount _User;

        public void Initialize(string PC, IdmAccount Usr)
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
}
