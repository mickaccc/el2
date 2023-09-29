using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lieferliste_WPF.Data;
using Lieferliste_WPF.Data.Models;


namespace Lieferliste_WPF.Utilities
{
    internal class DbManager
    {
        private static readonly DbManager _instance = new();
        private DbManager() { }
        public static DbManager GetInstance()
        {

            return _instance;
        }

        public Vorgang? GetVorgangSelect(string vid)
        {
            return null;
        }

        internal IEnumerable<object> getResources()
        {
            throw new NotImplementedException();
        }

        internal IEnumerable getUsers()
        {
            throw new NotImplementedException();
        }
    }
}
