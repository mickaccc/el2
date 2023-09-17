using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
        public static string PathProvider(string path)
        {
            bool isString = false;
            bool isMethod = false;
            bool isMethodParam = false;

            StringBuilder sb = new StringBuilder();
            StringBuilder methodParam = new StringBuilder();
            StringBuilder method = new StringBuilder();
            foreach (var val in path)
            {

                if (val == '"')
                {
                    isString = !isString;
                    continue;
                }
                if (val == '{')
                {
                    isMethod = true;
                    continue;
                }
                if (val == '}' || val == ')')
                {
                    isMethod = false;
                    isMethodParam = false;
                    var s = methodParam.ToString().Split(";");
                    sb.Append(Path.DirectorySeparatorChar).Append(Devide(s[0], s[1])).Append(Path.DirectorySeparatorChar);
                    continue;
                }
                if (val == '(')
                {
                    isMethodParam = true;
                    continue;
                }
                if (isString) sb.Append(val);
                if (isMethod)
                {
                    if (isMethodParam)
                    {
                        methodParam.Append(val);
                    }
                    else
                    {
                        method.Append(val);
                    }
                }


            }
            DirectoryInfo dir = new DirectoryInfo(sb.ToString());
            while (!dir.Exists) { dir.MoveTo(dir.Parent.ToString()); }
            
            return dir.FullName;
        }
    }




}
