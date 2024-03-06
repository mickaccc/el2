using El2Core.Services;
using GongSolutions.Wpf.DragDrop;
using Prism.Services.Dialogs;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Lieferliste_WPF.Dialogs.ViewModels
{
    internal class DocumentDialogVM : IDialogAware, IDropTarget
    { 
        public DocumentDialogVM(IUserSettingsService userSettingsService)
        {
            settingservice = userSettingsService;
        }
        public string Title => "Dokument kopieren";
        private IUserSettingsService settingservice;
        public event Action<IDialogResult> RequestClose;

        public string? Path { get; set; }

        public bool CanCloseDialog()
        {
            return true;        }

        public void OnDialogClosed()
        {
            
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            
        }

        public void DragOver(IDropInfo dropInfo)
        {
            if (dropInfo.Data is IDataObject)
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                dropInfo.Effects = DragDropEffects.Copy;
            }
        }

        public void Drop(IDropInfo dropInfo)
        {
            if (dropInfo.Data is IDataObject f)
            {
                var o = (string[])f.GetData(DataFormats.FileDrop);
                if (o.Length > 0)
                {
                    TextBox? tx = dropInfo.VisualTarget as TextBox;
                    if (tx != null) tx.Text = o[0];
                }
            }
        }
    }
}
