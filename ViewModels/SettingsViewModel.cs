using Lieferliste_WPF.Entities;
using Lieferliste_WPF.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace Lieferliste_WPF.ViewModels
{
    class SettingsViewModel : Base.ViewModelBase
    {
        public ObservableList<tblBerechtigung> PermissionsAvail { get; set; }
        public ObservableList<tblBerechtigung> PermissionsChecked { get; set; }
        public List<Roles> Roles { get; set; }
        public List<tblUserListe> Users { get; set; }
        public ObservableList<Roles> RolesChecked { get; set; }
        public ObservableList<Roles> RolesAvail { get; private set; }
        private int _roleIdent = 0;
        private string _userIdent;


        public int RoleIdent
        {
            get { return _roleIdent; }
            set
            {
                _roleIdent = value;
                RaisePropertyChanged("RoleIdent");
                reloadPermissionChecked();
                PermissionsAvail.Clear();
                using (var ctx = new EntitiesPermiss())
                {
                    ObservableList<tblBerechtigung> _permiss = new ObservableList<tblBerechtigung>();

                    foreach (tblBerechtigung p in ctx.tblBerechtigung)
                    {
                        if (!PermissionsChecked.Any(x => x.Berechtigung == p.Berechtigung))
                            PermissionsAvail.Add(p);
                    }
                }

            }
        }



        public string UserIdent
        {
            get { return _userIdent; }
            set
            {
                _userIdent = value;
                RaisePropertyChanged("UserIdent");
                reLoadRolesChecked();
                RolesAvail.Clear();

                foreach (Roles r in Roles)
                {
                    if (!RolesChecked.Any(x => x.id == r.id))
                        RolesAvail.Add(r);
                }

            }
        }

        //DB_COS_LIEFERLISTE_SQLEntities ctx = new DB_COS_LIEFERLISTE_SQLEntities(Statics.);

        public SettingsViewModel()
        {
            PermissionsAvail = new ObservableList<tblBerechtigung>();
            Roles = new List<Roles>();
            Users = new List<tblUserListe>();
            PermissionsChecked = new ObservableList<tblBerechtigung>();
            RolesChecked = new ObservableList<Roles>();
            RolesAvail = new ObservableList<Roles>();
            LoadData();
        }
        private void LoadData()
        {
            using (var ctx = new EntitiesPermiss())
            {

                foreach (var p in ctx.Roles.ToList())
                {
                    Roles.Add(p);
                }
                foreach (var u in ctx.tblUserListe.ToList())
                {
                    Users.Add(u);
                }


            }
        }
        private void reLoadRolesChecked()
        {
            using (var ctx = new EntitiesPermiss())
            {
                var ur = from u in ctx.UserRoles
                         join r in ctx.Roles on u.RoleID equals r.id
                         where u.UserIdent == UserIdent
                         select u.Roles;
                RolesChecked.Clear();
                RolesChecked.AddRange(ur);
            }
        }
        private void reloadPermissionChecked()
        {
            using (var ctx = new EntitiesPermiss())
            {
                var pc = from p in ctx.PermissionRoles
                         join r in ctx.tblBerechtigung on p.PermissionKey equals r.Berechtigung
                         where p.RoleKey == RoleIdent
                         select p.tblBerechtigung;
                PermissionsChecked.Clear();
                PermissionsChecked.AddRange(pc);
            }
        }
    }
}
