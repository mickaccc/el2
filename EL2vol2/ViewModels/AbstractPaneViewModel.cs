namespace Lieferliste_WPF.ViewModels
{
    /// <summary>
    /// Abstract base class for an AvalonDock pane view-model.
    /// </summary>
    public abstract class AbstractPaneViewModel : AbstractViewModel
    {
        /// <summary>
        /// Set to 'true' when the pane is visible.
        /// </summary>
        private bool isVisible = true;

        /// <summary>
        /// Set to 'true' when the pane is visible.
        /// </summary>
        public bool IsVisible
        {
            get
            {
                return isVisible;
            }
            set
            {
                if (isVisible == value)
                {
                    return;
                }

                isVisible = value;

                OnPropertyChanged("IsVisible");
            }
        }
    }
}
