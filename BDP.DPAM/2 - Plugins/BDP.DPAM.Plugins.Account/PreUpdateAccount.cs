using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }

    }
}

