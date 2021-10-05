using BDP.DPAM.Shared.Manager_Base;
using Microsoft.Xrm.Sdk;
using System;

namespace BDP.DPAM.Plugins.Account
{
    public class PreCreateAccount : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            try
            {
                AccountController ctrl = new AccountController(serviceProvider);
                ctrl.ValidatePipeline("account", "create", PluginStage.PreOperation);
                //SHER-334
                ctrl.AddFieldsInTargetWhenOriginatingLeadExists();
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }
    }
}
