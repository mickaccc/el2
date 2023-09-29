using Lieferliste_WPF.ViewModels.Base;
using Lieferliste_WPF.ViewModels.Support;
using Lieferliste_WPF.Utilities;
using Lieferliste_WPF.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;

namespace Lieferliste_WPF.ViewModels
{
    class ToolCase : Base.ViewModelBase
    {
        static ToolCase _this = new ToolCase();
        private Perspective _activePerspective;
        private List<Perspective> _perspectives = new List<Perspective>();
        public ICommand toArchive { get; private set; }
        public List<Perspective> Perspectives { get { return _perspectives; } }
        protected ToolCase()
        {
            _activePerspective = _perspectives.ElementAt(0);
            toArchive = new ActionCommand(OnArchiveExecuted, OnArchiveCanExecute);
        }
        public static ToolCase This { get { return _this; } }
        public Perspective ActivePerspective
        {
            get { return _activePerspective; }
            set { _activePerspective = value; }
        }
        public void ChangeActivePerpective(string Name)
        {
            if (_perspectives.Exists(x => x.Name == Name))
            {
                _activePerspective = _perspectives.Find(x => x.Name == Name);
                RaisePropertyChanged("ActivePerspective");
                RaisePropertyChanged("LeaderPanes");
                RaisePropertyChanged("AttachedPanes");
            }
        }
        public void PropertyModifieded()
        {
            RaisePropertyChanged("AttachedPanes");
        }
        public ObservableList<ViewModelBase> LeaderPanes
        {
            get { return _activePerspective.LeaderPanes; }
        }
        public ObservableList<ViewModelBase> AttachedPanes
        {
            get { return _activePerspective.AttatchedPanes; }
        }



        public void InitCommandBindings(Window win)
        {

            win.CommandBindings.Add(new CommandBinding((_perspectives[0].LeaderPanes[0] as DeliveryListViewModel).SortAscCommand));
            win.CommandBindings.Add(new CommandBinding((_perspectives[0].LeaderPanes[0] as DeliveryListViewModel).SortDescCommand));

        }

        void OnArchiveExecuted(object parameter)
        {
            if (parameter != null)
            {
            }
        }

        bool OnArchiveCanExecute(object parameter) { return true; }
    }
}
