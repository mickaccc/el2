using El2Core.Utils;
using El2Core.ViewModelBase;
using ModuleDeliverList.Views;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Lieferliste_WPF.ViewModels
{
    
    public class LoadingViewViewModel : ViewModelBase, INavigationAware
    {
        private IContainerExtension _container;
        private NotifyTaskCompletion<UIElement>? _contentViewTask;
        public NotifyTaskCompletion<UIElement>? ContentViewTask
        {
            get { return _contentViewTask; }
            set
            {
                if (_contentViewTask != value)
                {
                    _contentViewTask = value;
                    //NotifyPropertyChanged(() => ContentViewTask);
                }
            }
        }
        public LoadingViewViewModel(IContainerExtension container)
        {
            _container = container;
        }
        private async Task<UIElement> LoadingViewAsync()
        {
            var ret = 
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Loaded, () => { _container.Resolve<Liefer>(); });

            return (UIElement)ret.Result;
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            var u = navigationContext.Uri;
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            throw new NotImplementedException();
        }
    }
}
