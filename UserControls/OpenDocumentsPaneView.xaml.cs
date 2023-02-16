using System;
using System.Windows.Controls;
using System.Windows;
using Lieferliste_WPF.ViewModels;

namespace Lieferliste_WPF.UserControls
{
    /// <summary>
    /// View class for the view of open documents.
    /// </summary>
    public partial class OpenDocumentsPaneView : UserControl
    {
        public OpenDocumentsPaneView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Convenient accessor for the view-model.
        /// </summary>
        private OpenDocumentsPaneViewModel ViewModel
        {
            get
            {
                return (OpenDocumentsPaneViewModel)this.DataContext;
            }
        }

        /// <summary>
        /// Event raised the user clicks the 'Close Selected' button.
        /// </summary>
        private void CloseSelected_Click(object sender, RoutedEventArgs e)
        {
            this.ViewModel.CloseSelected();
        }
    }
}
