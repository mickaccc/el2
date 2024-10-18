using El2Core.ViewModelBase;
using Prism.Ioc;
using System;

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
