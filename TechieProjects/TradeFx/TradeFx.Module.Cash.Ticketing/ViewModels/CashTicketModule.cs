using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;

using TradeFx.Module.Cash.Ticket.Views;

namespace TradeFx.Module.Cash.Ticket.ViewModels
{
    [Module(ModuleName = "CashTicket")]
    public class CashTicketModule : IModule
    {
        private readonly IRegionViewRegistry _regionViewRegistry;

        public CashTicketModule(IRegionViewRegistry regionViewRegistry)
        {
            _regionViewRegistry = regionViewRegistry;
        }

        public void Initialize()
        {
            _regionViewRegistry.RegisterViewWithRegion("CashTicketRegion", typeof(CashTicketView));
        }
    }
}
