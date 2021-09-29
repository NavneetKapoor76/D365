using System;
using BDP.DPAM.Plugins.Account.Controller;
using BDP.DPAM.Shared.Manager_Base;
using Microsoft.Xrm.Sdk;

namespace BDP.DPAM.Plugins.Account
{
    public class PreUpdateAccount : IPlugin
    {

        public void Execute(IServiceProvider serviceProvider)
        {
            try
            {
                AccountController ctrl = new AccountController(serviceProvider);
                ctrl.ValidatePipeline("account", "update", PluginStage.PreOperation);
                //SHER-292
                ctrl.CompleteSegmentation();
                //SHER-337
                ctrl.CheckLocalAndBusinessSegmentationCountry();
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }

    }
}

