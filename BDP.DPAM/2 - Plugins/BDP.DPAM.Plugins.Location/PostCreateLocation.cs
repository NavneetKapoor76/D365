using BDP.DPAM.Shared.Manager_Base;
using Microsoft.Xrm.Sdk;
using System;

namespace BDP.DPAM.Plugins.Location
{
    public class PostCreateLocation : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            try
            {
                LocationController ctrl = new LocationController(serviceProvider);
                ctrl.ValidatePipeline("dpam_location", "create", PluginStage.PostOperation);
                ctrl.SyncAddressAccountWhenLocationIsCreated();
            }
            catch(Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }
    }
}
