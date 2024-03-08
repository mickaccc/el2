using BrendanGrant.Helpers.FileAssociation;
using CompositeCommands.Core;
using El2Core.Constants;
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
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Printing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT;


namespace Lieferliste_WPF.ViewModels
{
    [ComImport, System.Runtime.InteropServices.Guid("3E68D4BD-7135-4D10-8018-9FB6D9F33FA1"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IInitializeWithWindow
    {
        void Initialize([In] IntPtr hwnd);
    }

 
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
        private ObservableList<Attachment> _attachments = new();
        public ObservableList<Attachment> Attachments
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
        }

        private async Task<ICollectionView> LoadAsync(string projectNo)
        {
            var pro = await _dbctx.Projects
                .Include(x => x.OrderRbs)
                .ThenInclude(x => x.MaterialNavigation)
                .Include(x => x.ProjectAttachments)
                .FirstAsync(x => x.ProjectPsp == projectNo);

            this.WbsElement = pro.ProjectPsp.Trim() ?? "NULL";
            this.WbsInfo = pro.ProjectInfo?.Trim() ?? string.Empty;
            this.Project = pro;

            foreach(var o in pro.ProjectAttachments)
            {
                AddAttachment(o.AttachId, o.AttachmentLink, o.IsLink);
            }
            _orderRbs = new List<OrderRb>(pro.OrderRbs.ToList());
            OrdersView = CollectionViewSource.GetDefaultView(_orderRbs);
            return OrdersView;
        }

        private void AddAttachment(int id, string file, bool isLink)
        {
            Attachment attachment = new(id, isLink);
            FileInfo fi = new FileInfo(file);
            var fileass = new FileAssociationInfo(fi.Extension);
            if (fileass.Exists)
            {
                var prog = new ProgramAssociationInfo(fileass.ProgID);
                ImageSource icon;

                if (prog.Exists)
                {
                    icon = GetIcon(prog.DefaultIcon);
                }
                else icon = new BitmapImage(new Uri("\\Images\\unknown-file.png", UriKind.Relative));

                attachment.Content = icon;
                attachment.Name = (isLink) ? fi.FullName : fi.Name;
                Attachments.Add(attachment);
            }
        }
        public static ImageSource GetIcon(ProgramIcon programIcon)
        {
            try
            {      
                Icon icon = Icon.ExtractIcon(programIcon.Path, programIcon.Index, 32);

                return System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                            icon.Handle,
                            new Int32Rect(0, 0, icon.Width, icon.Height),
                            BitmapSizeOptions.FromEmptyOptions());
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format("{0}\n{1}", e.Message, e.InnerException), "GetIcon", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    AddFile(o[0]);
                }
            }
        }
        private void AddFile(string fileString)
        {
            FileInfo fi = new FileInfo(fileString);
            if (fi.Exists)
            {
                ProjectAttachment Patt = new();
                if (fi.Length < 0x500000)    //Filesize of 5 MiB
                {

                    MemoryStream ms = new MemoryStream();
                    using (FileStream file = new FileStream(fileString, FileMode.Open, FileAccess.Read))
                        file.CopyTo(ms);
                    Patt.AttachmentLink = fi.Name;
                    Patt.AttachmentBin = ms.ToArray();
                    Patt.Timestamp = DateTime.Now;
                }
                else if (MessageBox.Show("Die Datei ist größer als 5 MiB, soll es als Link gespeichert werden?", "",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    Patt.AttachmentLink = fi.FullName;
                    Patt.IsLink = true;
                    Patt.Timestamp = DateTime.Now;
                }
                else return;
                Project.ProjectAttachments.Add(Patt);
                _dbctx.SaveChanges();
                AddAttachment(Patt.AttachId, Patt.AttachmentLink, Patt.IsLink);
            }
            else MessageBox.Show("Datei wurde nicht gefunden", "Datei anfügen", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        private bool OnOpenFileCanExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission(Permissions.OpenFileProj);
        }

        private void OnOpenFileExecuted(object obj)
        {
            try
            {
                Attachment att = (Attachment)obj;
                FileInfo fi = new FileInfo(att.Name);
                string filepath;
                if (att.IsLink)
                {
                    filepath = att.Name;                   
                }
                else
                {
                    var pa = Project.ProjectAttachments.First(x => x.AttachId == att.Ident);
                    using MemoryStream memoryStream = new MemoryStream((byte[])pa.AttachmentBin);

                    filepath = Path.Combine(Path.GetTempPath(), fi.Name);
                    using FileStream fs = new(filepath, FileMode.Create);
                    memoryStream.CopyTo(fs);
                    fs.Flush();
                    fs.Close();
                }
                var asso = MiniFileAssociation.Association.GetAssociatedExePath(fi.Extension);

                if (asso != null) new Process() { StartInfo = new ProcessStartInfo(filepath) { UseShellExecute = true } }.Start();
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format("{0}\n{1}", e.Message, e.InnerException), "OpenStream", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private bool OnRemoveFileCanExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission(Permissions.DelProjAttachment);
        }

        private void OnRemoveFileExecuted(object obj)
        {
            var att = (Attachment)obj;
            Attachments.Remove(att);
            var dbAtt = _dbctx.ProjectAttachments.FirstOrDefault(x => x.AttachId == att.Ident);
            if (dbAtt != null)
            {
                _dbctx.ProjectAttachments.Remove(dbAtt);
                _dbctx.SaveChanges();
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

            Printing.DoPrintPreview(this, printDialog);
        }
        private bool OnAddFileCanExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission(Permissions.AddProjAttachment);
        }

        private async void OnAddFileExecutedAsync(object obj)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            var initializeWithWindowWrapper = openPicker.As<IInitializeWithWindow>();
            initializeWithWindowWrapper.Initialize(GetActiveWindow());
            openPicker.ViewMode = PickerViewMode.List;
            openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            openPicker.FileTypeFilter.Add("*");
            StorageFile op = await openPicker.PickSingleFileAsync();
            if (op != null) { AddFile(op.Path); }
        }
        private bool OnAddFileAsLinkCanExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission(Permissions.AddProjAttachment);
        }

        private async void OnAddFileAsLinkExecutedAsync(object obj)
        {
            FileOpenPicker openPicker = new();
            var initializeWithWindowWrapper = openPicker.As<IInitializeWithWindow>();
            initializeWithWindowWrapper.Initialize(GetActiveWindow());
            openPicker.ViewMode = PickerViewMode.List;
            openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            openPicker.FileTypeFilter.Add("*");
            StorageFile op = await openPicker.PickSingleFileAsync();
            if (op != null)
            {
                ProjectAttachment patt = new();
                patt.AttachmentLink = op.Path;
                patt.IsLink = true;

                Project.ProjectAttachments.Add(patt);
                _dbctx.SaveChanges();

                AddAttachment(patt.AttachId, patt.AttachmentLink, patt.IsLink);
            }
        }
    }
}
