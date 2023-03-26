using Lieferliste_WPF.Data.Models;
using Lieferliste_WPF.Data;
using Lieferliste_WPF.Utilities;
using System.Collections.Generic;
using System.Linq;


namespace Lieferliste_WPF.ViewModels
{
    class SettingsViewModel : Base.ViewModelBase
    {
        public ObservableList<Permission> PermissionsAvail { get; set; }
        public ObservableList<Permission> PermissionsChecked { get; set; }
        public List<Role> Roles { get; set; }
        public List<User> Users { get; set; }
        public ObservableList<Role> RolesChecked { get; set; }
        public ObservableList<Role> RolesAvail { get; private set; }
        private int _roleIdent = 0;
        private readonly string _userIdent;
        private DataContext _db = new DataContext();



        public int RoleIdent
        {
            get { return _roleIdent; }
            set
            {
                _roleIdent = value;
                RaisePropertyChanged("RoleIdent");
                reloadPermissionChecked();
                PermissionsAvail.Clear();
                using (var ctx = new DataContext())
                {



                    //foreach (Permission p in ctx.Permissions)
                    //{
                    //    if (!PermissionsChecked.Any(x => x.ValidKey == p.ValidKey))
                    //        PermissionsAvail.Add(p);
                    //}
                }
            }
        }




        public string UserIdent
        {
            get { return _userIdent; }
            set
            {
                RaisePropertyChanged("UserIdent");
                reLoadRolesChecked();
                RolesAvail.Clear();

                foreach (Role r in Roles)
                {
                    if (!RolesChecked.Any(x => x.Id == r.Id))
                        RolesAvail.Add(r);
                }

            }
        }

        public SettingsViewModel()
        {
            _userIdent = "";
            PermissionsAvail = new ObservableList<Permission>();
            Roles = new List<Role>();
            Users = new List<User>();
            PermissionsChecked = new ObservableList<Permission>();
            RolesChecked = new ObservableList<Role>();
            RolesAvail = new ObservableList<Role>();
            LoadData();
        }
        private void LoadData()
        {
            using (var ctx = new DataContext())
            {

                foreach (var p in ctx.Roles.ToList())
                {
                    Roles.Add(p);
                }
                //foreach (var u in ctx.Users.ToList())
                //{
                //    Users.Add(u);
                //}


            }
        }
        private void reLoadRolesChecked()
        {
            using (var ctx = new DataContext())
            {
                var ur = from u in ctx.UserRoles
                         join r in ctx.Roles on u.RoleId equals r.Id
                         where u.UserIdent == UserIdent
                         select u.Role;
                RolesChecked.Clear();
                RolesChecked.AddRange(ur);
            }
        }
        private void reloadPermissionChecked()
        {
            using (var ctx = new DataContext())
            {
                //var pc = from p in ctx.PermissionRoles
                //         join r in ctx.Permissions on p.PermissionKey equals r.ValidKey
                //         where p.RoleKey == RoleIdent
                //         select p.PermissionKeyNavigation;
                PermissionsChecked.Clear();
               //PermissionsChecked.AddRange(pc);
            }
        }
    }
}
