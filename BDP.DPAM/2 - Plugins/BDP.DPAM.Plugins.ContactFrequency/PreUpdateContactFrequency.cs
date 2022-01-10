using BDP.DPAM.Shared.Manager_Base;
using Microsoft.Xrm.Sdk;
using System;

namespace BDP.DPAM.Plugins.ContactFrequency
{
    public class PreUpdateContactFrequency : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            try
            {
                ContactFrequencyController ctrl = new ContactFrequencyController(serviceProvider);
                ctrl.ValidatePipeline("dpam_contactfrequency", "update", PluginStage.PreOperation);
                //SHER-550
                ctrl.PotentialDuplicationManagement();
                //SHER-421
                ctrl.UpdateNumberOfRemainingActivitiesValue();
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
