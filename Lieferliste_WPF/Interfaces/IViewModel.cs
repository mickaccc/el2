using Lieferliste_WPF.Utilities;
using Lieferliste_WPF.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Lieferliste_WPF.Interfaces
{
    internal interface IViewModel
    {
        public NotifyTaskCompletion<ObservableCollection<ViewModelBase>> TaskCompletion { get; }
    }
}
