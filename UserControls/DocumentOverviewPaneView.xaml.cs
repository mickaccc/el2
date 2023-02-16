using System;
using System.Windows.Controls;
using System.Windows;
using Lieferliste_WPF.ViewModels;

namespace Lieferliste_WPF.UserControls
{
    public partial class DocumentOverviewPaneView : UserControl
    {
        public DocumentOverviewPaneView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Convenient accessor for the view-model.
        /// </summary>
        private DocumentOverviewPaneViewModel ViewModel
        {
            get
            {
                return (DocumentOverviewPaneViewModel)this.DataContext;
            }
        }
    }
}
