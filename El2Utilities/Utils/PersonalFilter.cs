using El2Core.Models;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace El2Core.Utils
{
    public class PersonalFilterContainer
    {

        Dictionary<string, PersonalFilter> _filters = [];
        public void Add(string name, PersonalFilter filter)
        {
            _filters.Add(name, filter);
        }
    }
    public class PersonalFilter
    {
        private IContainerProvider _container;
        private string _RuleCommand;
        private string _Property;
        private PropertyNames _Prop;

        public PropertyNames Prop => _Prop;
        public string RuleCommand => _RuleCommand;
        public string Property => _Property;
        public PersonalFilter(string rule, string prop, IContainerProvider container) { _RuleCommand = rule; _Property = prop; _container = container; }
        public bool TestValue(Vorgang vorgang)
        {
            var navi = _Property.Split('.');
            if (navi.Length > 0)
            {
                using var db = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
                var modelData = db.Vorgangs.EntityType;
                foreach (var prop in navi)
                {
                    var nav = modelData.FindDeclaredNavigation(prop);
                    if(nav != null)
                    {
                        modelData = nav.TargetEntityType;
                    }
                    else
                    {
                        var result = modelData.FindDeclaredProperty(prop);
                        var r = result.PropertyInfo.GetValue(vorgang, null);
                    }
                }
            }
            else
            {

                Regex regex = new Regex(_RuleCommand);
                PropertyInfo info = vorgang.GetType().GetProperty(_Property);
                if (info != null)
                {
                    var get = info.GetValue(vorgang, null);

                    return regex.Match(get.ToString()).Success;
                }
            }

            return false;
        }


    }
    public struct PropertyNames
    {
        public static string Auftragsnummer = "Aid";
        public static string Material = "AidNavigation.Material";


    }

}
