using CompositeCommands.Core;
using El2Core.Constants;
using El2Core.Models;
using El2Core.Services;
using El2Core.Utils;
using El2Core.ViewModelBase;
using GongSolutions.Wpf.DragDrop;
using Lieferliste_WPF.Utilities;
using Microsoft.EntityFrameworkCore;
using Prism.Dialogs;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Printing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;


namespace Lieferliste_WPF.ViewModels
{



    internal class ProjectsViewModel : ViewModelBase, IDialogAware, IDropTarget
    {
        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto, PreserveSig = true, SetLastError = false)]
        public static extern IntPtr GetActiveWindow();
        private string _title = "Projektübersicht";
        public string Title => _title;
        private DB_COS_LIEFERLISTE_SQLContext _dbctx;
        private IUserSettingsService _userSettingsService;
        private IContainerProvider _container;
        private ICommand? _openFileCommand;
        public ICommand OpenFileCommand => _openFileCommand ??= new ActionCommand(OnOpenFileExecuted, OnOpenFileCanExecute);
        private ICommand? _removeFileCommand;
        public ICommand RemoveFileCommand => _removeFileCommand ??= new ActionCommand(OnRemoveFileExecuted, OnRemoveFileCanExecute);
        private ICommand? _printProjectCommand;
        public ICommand PrintProjectCommand => _printProjectCommand ??= new ActionCommand(OnPrintExecuted, OnPrintCanExecute);
        private ICommand? _addFileCommand;
        public ICommand AddFileCommand => _addFileCommand ??= new ActionCommand(OnAddFileExecutedAsync, OnAddFileCanExecute);

        private ICommand? _addFileAsLinkCommand;
        public ICommand AddFileAsLinkCommand => _addFileAsLinkCommand ??= new ActionCommand(OnAddFileAsLinkExecutedAsync, OnAddFileAsLinkCanExecute);

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
        private Project Project { get; set; }
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
        private string? _wbsInfo;
        public string? WbsInfo
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
        private ObservableList<IDisplayAttachment> _attachments = new();
        public ObservableList<IDisplayAttachment> Attachments
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

        public ICollectionView? OrdersView { get; private set; }

        public DialogCloseListener RequestClose { get; }

        public ProjectsViewModel(IContainerProvider container, IUserSettingsService userSettingsService, IApplicationCommands applicationCommands)
        {
            _container = container;
            _userSettingsService = userSettingsService;
            _applicationCommands = applicationCommands;
            _dbctx = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
        }

        private async Task<ICollectionView?> LoadAsync(string projectNo)
        {
            try
            {
                var pro = await _dbctx.Projects
             .Include(x => x.OrderRbs)
             .ThenInclude(x => x.MaterialNavigation)
             .Include(x => x.ProjectAttachments)
             .FirstAsync(x => x.ProjectPsp == projectNo);

                this.WbsElement = pro.ProjectPsp.Trim() ?? "NULL";
                this.WbsInfo = pro.ProjectInfo?.Trim() ?? string.Empty;

                var profact = new ProjAttachmentCreator();
                foreach (var o in pro.ProjectAttachments)
                {
                    var attDisp = profact.CreateDisplayAttachment(o.AttachmentLink, o.IsLink);
                    attDisp.Id = o.AttachId;
                    _attachments.Add(attDisp);
                }
                _orderRbs = new List<OrderRb>(pro.OrderRbs.ToList());
                OrdersView = CollectionViewSource.GetDefaultView(_orderRbs);
                return OrdersView;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Load Projects", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return null;
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
            if (PermissionsProvider.GetInstance().GetUserPermission(Permissions.AddProjAttachment))
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                dropInfo.Effects = DragDropEffects.All;
            }
        }

        public void Drop(IDropInfo dropInfo)
        {
            if(dropInfo.Data is IDataObject f)
            {
                var o = (string[])f.GetData(DataFormats.FileDrop);
                if (o.Length > 0)
                {
                    AddAttachment(o[0], false);
                }
            }
        }
        private void AddAttachment(string link, bool isLink)
        {
            var profact = new ProjAttachmentCreator();

            var att = profact.CreateDbAttachment(link, isLink);

            var patt = new ProjectAttachment();
            patt.Timestamp = att.TimeStamp;
            patt.AttachmentBin = att.BinaryData;
            patt.AttachmentLink = att.Link;
            patt.IsLink = att.IsLink;
            using var db = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
            var pro = db.Projects.Single(x => x.ProjectPsp == this.Project.ProjectPsp);
            pro.ProjectAttachments.Add(patt);
            db.SaveChanges();

            var attDisp = profact.CreateDisplayAttachment(att.Link, att.IsLink);
            attDisp.Id = patt.AttachId;
            _attachments.Add(attDisp);
        }
        private bool OnOpenFileCanExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission(Permissions.OpenFileProj);
        }

        private void OnOpenFileExecuted(object obj)
        {
            if (obj is ProjDisplayAttachment disp)
            {
                var fact = new ProjAttachmentCreator();
                if (disp.IsLink)
                {
                    AttachmentFactory.OpenFile(disp.Name, null);
                }
                else
                {
                    using var db = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
                    var att = db.ProjectAttachments.Single(x => x.AttachId.Equals(disp.Id));
                    using MemoryStream ms = new(att.AttachmentBin);
                    AttachmentFactory.OpenFile(disp.Name, ms);
                }
            }
        }
        private bool OnRemoveFileCanExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission(Permissions.DelProjAttachment);
        }

        private void OnRemoveFileExecuted(object obj)
        {
            if (obj is ProjDisplayAttachment att)
            {
                Attachments.Remove(att);
                var dbAtt = _dbctx.ProjectAttachments.FirstOrDefault(x => x.AttachId == att.Id);
                if (dbAtt != null)
                {
                    _dbctx.ProjectAttachments.Remove(dbAtt);
                    _dbctx.SaveChanges();
                }
            }
        }
        private bool OnPrintCanExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission(Permissions.PrintProj);
        }

        private void OnPrintExecuted(object obj)
        {
            PrintDialog printDialog = new PrintDialog();
            PrintTicket ticket = new PrintTicket();
            PrintingProxy proxy = new PrintingProxy();

            //proxy.PrintPreview(Title, null, null,  ticket);
        }
        private bool OnAddFileCanExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission(Permissions.AddProjAttachment);
        }

        private async void OnAddFileExecutedAsync(object obj)
        {
            var f = await AttachmentFactory.GetFilePath();
            if (f != null)
                AddAttachment(f, false);
        }
        private bool OnAddFileAsLinkCanExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission(Permissions.AddProjAttachment);
        }

        private async void OnAddFileAsLinkExecutedAsync(object obj)
        {
            var f = await AttachmentFactory.GetFilePath();
            if (f != null)
                AddAttachment(f, true);
        }
    }
    internal class ProjDbAttachment : IDbAttachment
    {
        public string Link { get; set; }
        public bool IsLink { get; set ; }
        public DateTime TimeStamp { get; set; }
        public byte[]? BinaryData { get; set; }
    }
    internal class ProjDisplayAttachment : IDisplayAttachment
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public object? Content { get; set; }
        public bool IsLink { get; set; }
    }
    internal class ProjAttachmentCreator : AttachmentFactory
    {
        public override IDbAttachment CreateDbAttachment(string link, bool isLink)
        {
            var attachment = new ProjDbAttachment();
            var att = FloatAttachment(attachment, link, isLink);
            return att;
        }

        public override IDisplayAttachment CreateDisplayAttachment(string link, bool isLink)
        {
            var attachment = new ProjDisplayAttachment();
            var att = FloatAttachment(attachment, link, isLink);
            return att;
        }
    }
}
