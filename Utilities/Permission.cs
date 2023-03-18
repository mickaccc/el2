using System.Collections.Generic;

namespace Lieferliste_WPF.Utilities
{
    public class Access
    {
        public bool IsAllowed { get; set; }
        internal List<string> AccessControllList { get; private set; }

        internal Access(params string[] allowedRoles)
        {
            IsAllowed = false;
            AccessControllList = new List<string>();
            foreach (var role in allowedRoles)
            {
                AccessControllList.Add(role);
            }
        }
    }
}
