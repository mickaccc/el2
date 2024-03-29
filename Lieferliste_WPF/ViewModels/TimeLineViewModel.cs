using El2Core.Models;
using El2Core.ViewModelBase;
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
    internal class TimeLineViewModel : ViewModelBase
    {
        public TimeLineViewModel(IContainerExtension container) 
        {
            _container = container;
            LoadData();
        }

        IContainerExtension _container;
        private TimeSpan stripe;

        public TimeSpan Stripe
        {
            get
            {
                return stripe;
            }
            set
            {
                if (value != stripe)
                {
                    stripe = value;
                    NotifyPropertyChanged(() => Stripe);
                }
            }
        }
        private DateTime endTime;

        public DateTime EndTime
        {
            get
            {
                return endTime;
            }
            set
            {
                if (value != endTime)
                {
                    endTime = value;
                    NotifyPropertyChanged(() => EndTime);
                }
            }
        }


        private void LoadData()
        {

        }
    }
}
