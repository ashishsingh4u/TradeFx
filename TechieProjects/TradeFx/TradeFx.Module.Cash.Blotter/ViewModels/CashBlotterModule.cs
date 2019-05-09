using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;

using TradeFx.Module.Cash.Blotter.Views;

namespace TradeFx.Module.Cash.Blotter.ViewModels
{
    [Module(ModuleName = "CashBlotter")]
    public class CashBlotterModule : IModule
    {
        private readonly IRegionViewRegistry _regionViewRegistry;

        public CashBlotterModule(IRegionViewRegistry regionViewRegistry)
        {
            _regionViewRegistry = regionViewRegistry;
        }

        public void Initialize()
        {
            _regionViewRegistry.RegisterViewWithRegion("CashBlotterRegion", typeof(CashBlotterView));
        }
    }
}
