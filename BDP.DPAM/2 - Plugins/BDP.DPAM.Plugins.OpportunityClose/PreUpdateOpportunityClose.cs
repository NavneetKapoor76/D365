using BDP.DPAM.Shared.Manager_Base;
using Microsoft.Xrm.Sdk;
using System;

namespace BDP.DPAM.Plugins.OpportunityClose
{
    public class PreUpdateOpportunityClose : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            try
            {
                OpportunityCloseController ctrl = new OpportunityCloseController(serviceProvider);
                ctrl.ValidatePipeline("opportunityclose", "update", PluginStage.PreOperation);
                //SHER-982
                ctrl.ManageSortDateColumn();
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }
    }
}
