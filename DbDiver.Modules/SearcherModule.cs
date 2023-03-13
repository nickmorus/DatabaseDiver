using DbDiver.Core;
using DbDiver.Modules.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace DbDiver.Modules.Searcher
{
    public class SearcherModule : IModule
    {
        IRegionManager _regionManager;

        public SearcherModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            _regionManager.RegisterViewWithRegion(RegionNames.ContentRegion, typeof(DiverParameters));
            _regionManager.RegisterViewWithRegion(RegionNames.MenuRegion, typeof(Menu));
            _regionManager.RegisterViewWithRegion(RegionNames.ProgressRegion, typeof(Progress));
        }
        
        public void RegisterTypes(IContainerRegistry containerRegistry){}
    }
}