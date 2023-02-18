using Lieferliste_WPF.ViewModels.Base;
using System;
using System.Collections.ObjectModel;

namespace Lieferliste_WPF.ViewModels
{
    /// <summary>
    /// View-model for a pane that shows a list of open documents.
    /// </summary>
    public class OpenDocumentsPaneViewModel : AbstractPaneViewModel
    {
        public OpenDocumentsPaneViewModel(MainWindowViewModel mainWindowViewModel)
        {
            if (mainWindowViewModel == null)
            {
                throw new ArgumentNullException("mainWindowViewModel");
            }

            this.MainWindowViewModel = mainWindowViewModel;
            this.MainWindowViewModel.ActiveDocumentChanged += new EventHandler<EventArgs>(MainWindowViewModel_ActiveDocumentChanged);
        }

        /// <summary>
        /// View-models for documents.
        /// </summary>
        public ObservableCollection<ViewModelBase> Documents
        {
            get
            {
                return this.MainWindowViewModel.Documents;
            }
        }

        /// <summary>
        /// View-model for the active document.
        /// </summary>
        public ViewModelBase ActiveDocument
        {
            get
            {
                return this.MainWindowViewModel.ActiveDocument;
            }
            set
            {
                this.MainWindowViewModel.ActiveDocument = value;
            }
        }

        /// <summary>
        /// Close the currently selected document.
        /// </summary>
        public void CloseSelected()
        {
            var activeDocument = this.MainWindowViewModel.ActiveDocument;
            if (activeDocument != null)
            {
                this.MainWindowViewModel.Documents.Remove(activeDocument);
            }
        }

        /// <summary>
        /// Reference to the main window's view model.
        /// </summary>
        private MainWindowViewModel MainWindowViewModel
        {
            get;
            set;
        }

        /// <summary>
        /// Event raised when the active document in the main window has changed.
        /// </summary>
        private void MainWindowViewModel_ActiveDocumentChanged(object sender, EventArgs e)
        {
            OnPropertyChanged("ActiveDocument");
        }
    }
}
