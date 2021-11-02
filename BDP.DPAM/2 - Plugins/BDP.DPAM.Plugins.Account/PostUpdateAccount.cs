using BDP.DPAM.Shared.Manager_Base;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDP.DPAM.Plugins.Account
{
    public class PostUpdateAccount: IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            try
            {
                AccountController ctrl = new AccountController(serviceProvider);
                ctrl.ValidatePipeline("account", "update", PluginStage.PostOperation);
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }
    }
}
