using Lieferliste_WPF.ViewModels.Base;
using Lieferliste_WPF.Utilities;

namespace Lieferliste_WPF.ViewModels
{
    class Perspective
    {
        private ObservableList<ViewModelBase> _leaderPanes = new ObservableList<ViewModelBase>();
        private ObservableList<ViewModelBase> _attachedPanes = new ObservableList<ViewModelBase>();
        private bool _isVisible = false;
        public ObservableList<ViewModelBase> LeaderPanes { get { return _leaderPanes; } }
        public ObservableList<ViewModelBase> AttatchedPanes { get { return _attachedPanes; } }
        public string Name { get; set; }
        public int Type { get; set; }
        public int SubType { get; set; }
        public bool isVisible { get { return _isVisible; } set { _isVisible = value; } }

    }
}
