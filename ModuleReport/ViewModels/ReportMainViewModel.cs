using El2Core.Constants;
using El2Core.Models;
using El2Core.ViewModelBase;
using ModuleReport.Views;
using Prism.Events;
using Prism.Ioc;
using Prism.Regions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using El2Core.Utils;

namespace ModuleReport.ViewModels
{
    internal class ReportMainViewModel : ViewModelBase
    {
        public ReportMainViewModel(IContainerProvider containerProvider, IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            container = containerProvider;
            _regionManager = regionManager.CreateRegionManager();
            ea = eventAggregator;
  
 
            _regionManager.RegisterViewWithRegion<MaterialResultList>(RegionNames.ReportViewRegion);
            _regionManager.RegisterViewWithRegion<SelectionWorkArea>(RegionNames.ReportFilterRegion);
            _regionManager.RegisterViewWithRegion<SelectionDate>(RegionNames.ReportToolRegion);
            _regionManager.RegisterViewWithRegion<MaterialResultChart>(RegionNames.ReportViewRegion1);
            
        }
        public string Title { get; } = "Bericht und Auswertungen";

        private IRegionManager _regionManager;
        IContainerProvider container;
        IEventAggregator ea;
     
    }
}
