using ModuleReport.ReportSources;

namespace ModuleReport
{
    public class ReportModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IMaterialSource, MaterialSource>();
        }
 
    }
}
