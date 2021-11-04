using BDP.DPAM.Shared.Manager_Base;
using Microsoft.Xrm.Sdk;
using System;

namespace BDP.DPAM.Plugins.ContactFrequency
{
    public class PreCreateContactFrequency : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            try
            {
                ContactFrequencyController ctrl = new ContactFrequencyController(serviceProvider);
                ctrl.ValidatePipeline("dpam_contactfrequency", "create", PluginStage.PreOperation);
                //SHER-478
                ctrl.SetDefaultName();
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }
    }
}
