using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lieferliste_WPF.Interfaces
{
    interface IProgressbarInfo
    {
        static double ProgressValue { get; set; }
        private static bool IsLoading { get; set; }
        void SetProgressIsBusy();
    }
}
