using BDP.DPAM.Shared.Manager_Base;
using Microsoft.Xrm.Sdk;
using System;

namespace BDP.DPAM.Plugins.Lead
{
    public class PreCreateLead : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            try
            {
                LeadController ctrl = new LeadController(serviceProvider);
                ctrl.ValidatePipeline("lead", "create", PluginStage.PreOperation);
                //SHER-331
                ctrl.FillInBusinessSegmentation();

            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }
    }
}
