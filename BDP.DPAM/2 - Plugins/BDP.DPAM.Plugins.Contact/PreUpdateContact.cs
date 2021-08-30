using BDP.DPAM.Shared.Manager_Base;
using Microsoft.Xrm.Sdk;
using System;

namespace BDP.DPAM.Plugins.Contact
{
    public class PreUpdateContact : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            try
            {
                ContactController ctrl = new ContactController(serviceProvider);
                ctrl.ValidatePipeline("contact", "update", PluginStage.PreOperation);
                //SHER-141
                ctrl.AddAddressFieldInTargetBasedOnMainLocation();

                // SHER-201
                ctrl.SetContactGreetingBasedOnLanguageAndGender();
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }
    }
}
