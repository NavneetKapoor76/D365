using Microsoft.Xrm.Sdk;
using System;

namespace BDP.DPAM.Plugins.PhoneCall
{
    public class PreUpdatePhoneCall : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            try
            {
                PhoneCallController ctrl = new PhoneCallController(serviceProvider);
                ctrl.ValidatePipeline("phonecall", "update", Shared.Manager_Base.PluginStage.PreOperation);
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
