using CompositeCommands.Core;
using El2Core.Models;
using El2Core.Services;
using El2Core.Utils;
using El2Core.ViewModelBase;
using Microsoft.EntityFrameworkCore;
using Prism.Ioc;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Lieferliste_WPF.ViewModels
{
    class ProjectsViewModel : ViewModelBase, IDialogAware
    {
        private string _title = "Projektübersicht";
        public string Title => _title;

        private DB_COS_LIEFERLISTE_SQLContext _dbctx;
        private IUserSettingsService _userSettingsService;
        private IContainerProvider _container;
        IApplicationCommands _applicationCommands;
        public IApplicationCommands ApplicationCommands
        {
            get { return _applicationCommands; }
            set { _applicationCommands = value; }
        }
        private NotifyTaskCompletion<ICollectionView> _projTask;
        public NotifyTaskCompletion<ICollectionView> ProjTask
        {
            get { return _projTask; }
            set
            {
                if (_projTask != value)
                {
                    _projTask = value;
                    NotifyPropertyChanged(() => ProjTask);
                }
            }
        }
        private string _wbsElement;
        public string WbsElement
        {
            get { return _wbsElement; }
            private set
            {
                if (_wbsElement != value)
                {
                    _wbsElement = value;
                    NotifyPropertyChanged(() => WbsElement);
                }
            }
        }
        private string _wbsInfo;
        public string WbsInfo
        {
            get { return _wbsInfo; }
            private set
            {
                if (_wbsInfo != value)
                {
                    _wbsInfo = value;
                    NotifyPropertyChanged(() => WbsInfo);
                }
            }
        }
        private List<OrderRb> _orderRbs;

        public event Action<IDialogResult> RequestClose;

        public ICollectionView OrdersView { get; private set; }
        public ProjectsViewModel(IContainerProvider container, IUserSettingsService userSettingsService, IApplicationCommands applicationCommands)
        {
            _container = container;
            _userSettingsService = userSettingsService;
            _applicationCommands = applicationCommands; 
            _dbctx = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();

            
        }

        private async Task<ICollectionView> LoadAsync(string projectNo)
        {
            var pro = await _dbctx.Projects
                .Include(x => x.OrderRbs)
                .ThenInclude(x => x.MaterialNavigation)
                .FirstAsync(x => x.ProjectPsp == projectNo);
                
            this.WbsElement = pro.ProjectPsp.Trim() ?? "NULL";
            this.WbsInfo = pro.ProjectInfo?.Trim() ?? string.Empty;

            _orderRbs = new List<OrderRb>(pro.OrderRbs.ToList());
            OrdersView = CollectionViewSource.GetDefaultView(_orderRbs);
            return OrdersView;
        }

 

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            var p = parameters.GetValue<string>("projectNo");
            ProjTask = new NotifyTaskCompletion<ICollectionView>(LoadAsync(p));
        }
    }
}
