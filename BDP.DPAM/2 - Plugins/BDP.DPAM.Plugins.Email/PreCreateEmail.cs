using BDP.DPAM.Shared.Manager_Base;
using Microsoft.Xrm.Sdk;
using System;

namespace BDP.DPAM.Plugins.Email
{
    public class PreCreateEmail : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            try
            {
                EmailController ctrl = new EmailController(serviceProvider);
                ctrl.ValidatePipeline("email", "create", PluginStage.PreOperation);
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
