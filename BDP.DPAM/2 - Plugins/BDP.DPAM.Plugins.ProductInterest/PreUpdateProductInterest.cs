using BDP.DPAM.Shared.Manager_Base;
using Microsoft.Xrm.Sdk;
using System;

namespace BDP.DPAM.Plugins.ProductInterest
{
    public class PreUpdateProductInterest : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            try
            {
                ProductInterestController ctrl = new ProductInterestController(serviceProvider);
                ctrl.ValidatePipeline("dpam_productinterest", "update", PluginStage.PreOperation);
                //SHER-477
                ctrl.SetDefaultName();
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }
    }
}
