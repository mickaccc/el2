using El2Core.ViewModelBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuleProducts.Dialogs.ViewModels
{
    internal class ArchivProcessDialogVM : ViewModelBase, IDialogAware
    {
        private int _ArchivProcessingCount;

        public int ArchivProcessingCount
        {
            get { return _ArchivProcessingCount; }
            set { _ArchivProcessingCount = value; }
        }
        private int _Archivated;

        public int Archivated
        {
            get { return _Archivated; }
            set { _Archivated = value; }
        }
        private int _ArchivState2Count;

        public int ArchivState2Count
        {
            get { return _ArchivState2Count; }
            set { _ArchivState2Count = value; }
        }
        private int  _ArchivState3Count
;

        public int ArchivState3Count

        {
            get { return  _ArchivState3Count; }
            set {  _ArchivState3Count = value; }
        }
        private int _ArchivState4Count;

        public int ArchivState4Count
        {
            get { return _ArchivState4Count; }
            set { _ArchivState4Count = value; }
        }
        private bool _ArchivComplete;

        public bool ArchivComplete
        {
            get { return _ArchivComplete; }
            set { _ArchivComplete = value; }
        }

        public DialogCloseListener RequestClose => throw new NotImplementedException();

        public bool CanCloseDialog()
        {
            throw new NotImplementedException();
        }

        public void OnDialogClosed()
        {
            throw new NotImplementedException();
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            throw new NotImplementedException();
        }
    }
}
