using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;

namespace Lieferliste_WPF.Utilities
{
    public class PermissionsManager
    {
        private static PermissionsManager _instance;

        private DataSetPermission ds;
        private DataSetPermissionTableAdapters.PermissionUSERTableAdapter tableAdapt;
        private HashSet<String> _permissions;
        private String _user;

        public static PermissionsManager getInstance(String user)
        {
            if (_instance == null)
            {
                _instance = new PermissionsManager(user);
                return _instance;
            }
            return _instance;
        }

        private PermissionsManager(String user)
        {

            _permissions = new HashSet<string>();
            string pattern = @"^EMEA\\";
            Regex reg = new Regex(pattern);
            _user = reg.Replace(user.ToUpper(), "");
            try
            {
                ds = new DataSetPermission();
                tableAdapt = new DataSetPermissionTableAdapters.PermissionUSERTableAdapter();
                tableAdapt.Fill(ds.PermissionUSER, _user);
            }
            catch (Exception e)
            {

                System.Windows.MessageBox.Show("Fehler Permissionsmanager!\n" + e.Message + "\n" + e.InnerException,
                    "ERROR", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
            foreach (DataRow dr in ds.PermissionUSER.Rows)
            {
                _permissions.Add(dr[0].ToString());
            }
        }


        public Boolean getUserPermission(String permission)
        {
            return _permissions.Contains(permission);
        }
        public HashSet<DataRow> getPermissions()
        {

            //EL4_SQLDataSetTableAdapters.tblBerechtigungTableAdapter adapt = new EL4_SQLDataSetTableAdapters.tblBerechtigungTableAdapter();
            //adapt.Fill(ds.tblBerechtigung);
            HashSet<DataRow> rows = new HashSet<DataRow>();
            //foreach (DataRow dr in ds.tblBerechtigung.Rows)
            //{
            //    rows.Add(dr);
            //}
            return rows;
        }

    }
}
