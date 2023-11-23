﻿using El2Core.Utils;
using El2Core.ViewModelBase;
using ModuleDeliverList.Views;
using Prism.Ioc;
using Prism.Regions;
using System;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Lieferliste_WPF.ViewModels
{

    public class LoadingViewModel : ViewModelBase, INavigationAware
    {
        private readonly IContainerExtension _container;
        private NotifyTaskCompletion<Grid>? _contentViewTask;
        public NotifyTaskCompletion<Grid>? ContentViewTask
        {
            get { return _contentViewTask; }
            set
            {
                if (_contentViewTask != value)
                {
                    _contentViewTask = value;
                    NotifyPropertyChanged(() => ContentViewTask);
                }
            }
        }
        public string Title { get; set; } = string.Empty;
        public LoadingViewModel(IContainerExtension container)
        {
            _container = container;
        }
        private async Task<Grid> LoadingViewAsync(Grid view)
        {
            var vi = view as Liefer;
            var ret = await Task.Run(() => _container.Resolve<Liefer>());
            // dispatcher.BeginInvoke(DispatcherPriority.Loaded, () => { _container.Resolve<Liefer>(); });

            return (Grid)ret;
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            var u = navigationContext.Uri;
            Title = navigationContext.Parameters["Title"].ToString();
            var objParam = navigationContext.Parameters["View"] as Grid;
            ContentViewTask = new NotifyTaskCompletion<Grid>(LoadingViewAsync(objParam));
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