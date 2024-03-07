using El2Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace El2Core.Utils
{
    public readonly struct RuleInfo
    {
        public static List<Rule> Rules => _rules;
        private static List<Rule> _rules;

        public void Initialize(List<Rule> rules)
        {
            _rules = rules;
        }
    }
}
