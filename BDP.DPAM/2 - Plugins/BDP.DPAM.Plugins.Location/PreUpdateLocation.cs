using BDP.DPAM.Shared.Manager_Base;
using Microsoft.Xrm.Sdk;
using System;

namespace BDP.DPAM.Plugins.Location
{
    public class PreUpdateLocation : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            try
            {
                LocationController ctrl = new LocationController(serviceProvider);
                ctrl.ValidatePipeline("dpam_location", "update", PluginStage.PreOperation);
                ctrl.SetIsMainLocationWhenLocationBecomesInactive();
                ctrl.ConcatenateName();
                ctrl.RemoveInactiveLocationLinkedToContact();
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }
    }
}
