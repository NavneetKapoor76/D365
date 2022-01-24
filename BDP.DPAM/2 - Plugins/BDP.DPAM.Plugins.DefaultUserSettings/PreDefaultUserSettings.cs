using BDP.DPAM.Shared.Manager_Base;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDP.DPAM.Plugins.DefaultUserSettings
{
    public class PreDefaultUserSettings : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            try
            {
                DefaultUserSettingsController ctrl = new DefaultUserSettingsController(serviceProvider);
               // ctrl.ValidatePipeline("dpam_departments", "create", PluginStage.PreOperation);
                //SHER-695
                ctrl.Update();

            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }
    }
}
