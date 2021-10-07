using BDP.DPAM.Shared.Manager_Base;
using Microsoft.Xrm.Sdk;
using System;

namespace BDP.DPAM.Plugins.Opportunity
{
    public class PreCreateOpportunity : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            try
            {
                OpportunityController ctrl = new OpportunityController(serviceProvider);
                ctrl.ValidatePipeline("opportunity", "create", PluginStage.PreOperation);
                //SHER-368
                ctrl.SetDefaultPriceList();
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }
    }
}