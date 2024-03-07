using El2Core.Services;
using El2Core.Utils;
using GongSolutions.Wpf.DragDrop;
using Microsoft.Identity.Client.NativeInterop;
using Prism.Services.Dialogs;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
        private RelayCommand? vmpbCommand;
        private RelayCommand? gutCommand;
        private RelayCommand? musterCommand;
        public RelayCommand VmpbCommand => vmpbCommand ??= new RelayCommand(OnVmpbExecute);
        public RelayCommand GutCommand => gutCommand ??= new RelayCommand(OnGutExecute);
        public RelayCommand MusterCommand => musterCommand ??= new RelayCommand(OnMusterExecute);
        public string? Path { get; set; }

        private void OnMusterExecute(object obj)
        {
            if(string.IsNullOrEmpty(Path))
            {
                OnError();
            }
            else 
            {
                FileInfo file = new FileInfo(Path);
                if(file.Exists)
                {
                    //file.CopyTo(RuleInfo.Rules[0]);
                }
            }
        }

        private void OnVmpbExecute(object obj)
        {
            if (string.IsNullOrEmpty(Path))
            {
                OnError();
            }
            else { }
        }
        private void OnGutExecute(object obj)
        {
            if (string.IsNullOrEmpty(Path))
            {
                OnError();
            }
            else { }
        }
        
        private void OnError()
        {
            MessageBox.Show("Pfadangabe ist leer!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
        }
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
