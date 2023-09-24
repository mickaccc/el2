using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Lieferliste_WPF.Utilities
{
    public static class PathUtils
    {
        public static string Devide(string value, string param)
        {
            var p = param.Split(',');
            StringBuilder sb = new StringBuilder(value);
            int ind = 0;
            foreach (var val in p)
            {
                ind += int.Parse(val);
                if (sb.Length > ind)
                    sb.Insert(ind, Path.DirectorySeparatorChar);
                ind++;
            }
            return sb.ToString();
        }
        public static string PathProvider(string root,string path)
        {
            string resPath = Path.Combine(root);

            if(!Directory.Exists(resPath)) { return String.Empty; }

            string[] subdir = path.Split('\\');
            
            foreach(var subdir2 in subdir)
            {
                if (Directory.Exists(Path.Combine(resPath, subdir2)) && subdir2 != String.Empty)
                {
                    resPath = Path.Combine(resPath, subdir2);
                }
                else { break; }
            }
            return resPath;
        }
    }




}
