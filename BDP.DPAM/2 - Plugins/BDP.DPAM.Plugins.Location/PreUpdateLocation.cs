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
                //SHER-145
                ctrl.SetIsMainLocationWhenLocationBecomesInactive();

                //SHER-148
                ctrl.ConcatenateName();

                //SHER-141
                ctrl.RemoveInactiveLocationLinkedToContact();
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }
    }
}
