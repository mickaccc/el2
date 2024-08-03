using El2Core.Constants;
using ModuleReport.Views;
using Prism.Ioc;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuleReport.ViewModels
{
    internal class ReportMainViewModel
    {
        public ReportMainViewModel(IContainerProvider container, IRegionManager regionManager)
        {
            _containerProvider = container;
            _regionManager = regionManager.CreateRegionManager();

            _regionManager.RegisterViewWithRegion<MaterialResultList>(RegionNames.ReportViewRegion);
            _regionManager.RegisterViewWithRegion<SelectionWorkArea>(RegionNames.ReportFilterRegion);
            _regionManager.RegisterViewWithRegion<SelectionDate>(RegionNames.ReportToolRegion);
            
        }
        public string Title { get; } = "Bericht und Auswertungen";
        private IContainerProvider _containerProvider;
        private IRegionManager _regionManager;
    }
}
