using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lieferliste_WPF.ViewModels
{
    class MainViewModel
    {
        public ObservableCollection<Base.ViewModelBase> ViewModels { get; set; }

        public MainViewModel()
        {
            ViewModels= new ObservableCollection<Base.ViewModelBase>();
            ViewModels.Add(new(Lieferliste_WPF.ViewModels.ToolCase));
        }
        public void add_ViewModel(Base.ViewModelBase viewModel)
        {
            ViewModels.Add(viewModel);

        }
        public void remove_ViewModel(Base.ViewModelBase viewModel) { ViewModels.Remove(viewModel); }
    }
}
