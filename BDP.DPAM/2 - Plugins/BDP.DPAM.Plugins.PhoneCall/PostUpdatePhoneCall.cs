using BDP.DPAM.Shared.Manager_Base;
using Microsoft.Xrm.Sdk;
using System;

namespace BDP.DPAM.Plugins.PhoneCall
{
    public class PostUpdatePhoneCall : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            try
            {
                PhoneCallController ctrl = new PhoneCallController(serviceProvider);
                ctrl.ValidatePipeline("phonecall", "update", PluginStage.PostOperation);
                //SHER-421
                ctrl.UpdateNumberOfCompletedActivitiesOnContactFrequency();
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }
    }
}
