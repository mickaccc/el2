using El2Core.Models;
using Lieferliste_WPF.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lieferliste_WPF.ViewModels
{
    internal class TimeLineViewModel
    {
        public TimeLineViewModel(IContainerExtension container) 
        {
            _container = container;
            LoadData();
        }

        IContainerExtension _container;


        private void LoadData()
        {
            using var db = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
            var vorg = db.Vorgangs
                .Include(x => x.AidNavigation)
                .Include(x => x.RidNavigation)
                .Where(x => x.SysStatus.Contains("RÜCK") == false)
                .First();

            var stripe = ProcessStripeService.GetProcessLength(vorg, DateTime.Now);
        }
    }
}
