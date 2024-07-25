using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuleReport.ViewModels
{
    internal class ReportMainViewModel
    {
        public ReportMainViewModel(IContainerProvider container)
        {
            _containerProvider = container;
        }
        private IContainerProvider _containerProvider;
    }
}
