using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace El2Core.Utils
{
    public readonly struct CustomFilter
    {
        public readonly string Name => _name;
        public static string SearchText => _searchText;
        public static string DefaultFilter => _defaultFilter;
        public static string ProjectFilter => _projectFilter;
        public static bool FilterInvers => _filterInvers;

        private static string _name = "NAME";
        private static string _searchText = string.Empty;
        private static string _defaultFilter = string.Empty;
        private static string _projectFilter = string.Empty;
        private static bool _filterInvers;

        public void Initialize(string Name, string SearchText, string DefaultFilter, string ProjectFilter, bool FilterInvers)
        {
            _name = Name;
            _searchText = SearchText;
            _defaultFilter = DefaultFilter;
            _projectFilter = ProjectFilter;
            _filterInvers = FilterInvers;
        }
    }
}
