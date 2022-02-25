using BDP.DPAM.Shared.Manager_Base;
using Microsoft.Xrm.Sdk;
using System;

namespace BDP.DPAM.Plugins.PhoneCall
{
    public class PreCreatePhoneCall : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            try
            {
                PhoneCallController ctrl = new PhoneCallController(serviceProvider);
                ctrl.ValidatePipeline("phonecall", "create", PluginStage.PreOperation);
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
