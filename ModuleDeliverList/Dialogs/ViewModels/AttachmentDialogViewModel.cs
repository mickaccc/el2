using El2Core.Models;
using El2Core.Utils;
using GongSolutions.Wpf.DragDrop;
using Microsoft.Extensions.Logging;
using Prism.Dialogs;
using Prism.Ioc;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace ModuleDeliverList.Dialogs.ViewModels
{
    public class AttachmentDialogViewModel : IDropTarget, IDialogAware
    {
        public AttachmentDialogViewModel(IContainerProvider container)
        {
            Container = container;
            var loggerFactory = container.Resolve<ILoggerFactory>();
            Logger = loggerFactory.CreateLogger<AttachmentDialogViewModel>();
            OpenFileCommand = new ActionCommand(OnFileOpenExecuted, OnOpenFileCanExecute);
        }

        public string Title { get; } = "Anhang Vorgang";
        private Vorgang Vorgang { get; set; }
        public DialogCloseListener RequestClose { get; }

        IContainerProvider Container;
        ILogger Logger;
        private RelayCommand? _closeCommand;
        public RelayCommand CloseCommand => _closeCommand ??= new RelayCommand(OnDialogClosed);
        public ICommand OpenFileCommand { get; private set; }
        public ICollectionView AttachView { get; private set; }
        private ObservableCollection<IDisplayAttachment> _attachments = [];
        private bool OnOpenFileCanExecute(object arg)
        {
            return true;
        }

        private void OnFileOpenExecuted(object obj)
        {
            throw new NotImplementedException();
        }
        public void DragOver(IDropInfo dropInfo)
        {
            throw new NotImplementedException();
        }

        public void Drop(IDropInfo dropInfo)
        {
            if (dropInfo.Data is IDataObject f)
            {
                var o = (string[])f.GetData(DataFormats.FileDrop);
                if (o.Length > 0)
                {
                    var dbfact = new VorgangAttachmentCreator();
                    var att = dbfact.CreateDbAttachment(o[0], true);

                    var vatt = new VorgangAttachment();
                    vatt.Timestamp = att.TimeStamp;
                    vatt.Data = att.BinaryData;
                    using var db = Container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
                    var vrg = db.Vorgangs.Single(x => x.VorgangId == this.Vorgang.VorgangId);
                    vrg.VorgangAttachments.Add(vatt);
                }
            }
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            IDialogParameters param = new DialogParameters();
            RequestClose.Invoke(param);
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            using var db = Container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
            var vrg = parameters.GetValue<Vorgang>("vrg");
            this.Vorgang = vrg;
            var vaFactory = new VorgangAttachmentCreator();

            var attach = db.VorgangAttachments.Where(x => x.VorgangId == vrg.VorgangId).ToList();
            foreach (var att in attach)
            {
                var va = vaFactory.CreateDisplayAttachment(att.Link, true);
                va.Description = att.VorgangId;

                _attachments.Add(va);
            }
            AttachView = CollectionViewSource.GetDefaultView(_attachments);
        }
    }
    internal class VrgDisplayAttachment : IDisplayAttachment
    {

        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public object? Content { get; set; }
    }
    internal class VrgDbAttachment : IDbAttachment
    {
        public string? Link { get; set; }
        public bool IsLink { get; set; }
        public DateTime TimeStamp { get; set; }
        public byte[]? BinaryData { get; set; }
    }
    internal class VorgangAttachmentCreator : AttachmentFactory
    {
        public override IDbAttachment CreateDbAttachment(string? Link, bool IsLink)
        {
            var attachment = new VrgDbAttachment();
            attachment.Link = Link;
            attachment.IsLink = IsLink;
            var att = FloatAttachment(attachment, Link);
            return att;
        }

        public override IDisplayAttachment CreateDisplayAttachment(string link, bool isLink)
        {
            var attachment = new VrgDisplayAttachment();
            var att = FloatAttachment(attachment, link, isLink);
            return att;
        }
    }
}
