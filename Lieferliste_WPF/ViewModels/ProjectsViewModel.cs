using CompositeCommands.Core;
using El2Core.Models;
using El2Core.Services;
using El2Core.Utils;
using El2Core.ViewModelBase;
using GongSolutions.Wpf.DragDrop;
using Lieferliste_WPF.Utilities;
using Microsoft.EntityFrameworkCore;
using Prism.Ioc;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Lieferliste_WPF.ViewModels
{
    internal class ProjectsViewModel : ViewModelBase, IDialogAware, IDropTarget
    {
        private string _title = "Projektübersicht";
        public string Title => _title;

        private DB_COS_LIEFERLISTE_SQLContext _dbctx;
        private IUserSettingsService _userSettingsService;
        private IContainerProvider _container;
        private ICommand? _addFileCommand;
        public ICommand AddFileCommand => _addFileCommand ??= new ActionCommand(OnAddFileExecuted, OnAddFileCanExecute);


        private IApplicationCommands _applicationCommands;
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
        private List<Attachment> _attachments;
        public List<Attachment> Attachments
        {
            get { return _attachments; }
            set
            {
                if (value != _attachments)
                {
                    _attachments = value;
                    NotifyPropertyChanged(() => Attachments);
                }
            }
        }
        public event Action<IDialogResult> RequestClose;

        public ICollectionView OrdersView { get; private set; }
        public ProjectsViewModel(IContainerProvider container, IUserSettingsService userSettingsService, IApplicationCommands applicationCommands)
        {
            _container = container;
            _userSettingsService = userSettingsService;
            _applicationCommands = applicationCommands;
            _dbctx = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();

            _attachments = new List<Attachment>()
            {
                new() { Content = "TETST!!"},
                new() { Content = new object() },
                new() { Content = new Bitmap("C:\\Users\\mgsch\\Pictures\\DB_Y.png") }
            };
            //{
            //    new() { Name = "test1" },
            //    new() { Name = "test2" },
            //    new() { Name = "test3" },
            //    new() { Name = "test4" }
            //};
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

        public void DragOver(IDropInfo dropInfo)
        {
            dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
            dropInfo.Effects = DragDropEffects.Move;
        }

        public void Drop(IDropInfo dropInfo)
        {
            if(dropInfo.Data is object ob)
            {

            }
        }
        private bool OnAddFileCanExecute(object arg)
        {
            return true;
        }

        private void OnAddFileExecuted(object obj)
        {
            
        }
    }
}
