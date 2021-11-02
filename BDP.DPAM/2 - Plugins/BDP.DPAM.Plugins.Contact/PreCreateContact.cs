using System;
using BDP.DPAM.Shared.Manager_Base;
using Microsoft.Xrm.Sdk;

namespace BDP.DPAM.Plugins.Contact
{
	public class PreCreateContact : IPlugin
	{
		public void Execute(IServiceProvider serviceProvider)
		{
            try
            {
                ContactController ctrl = new ContactController(serviceProvider);
                ctrl.ValidatePipeline("contact", "create", PluginStage.PreOperation);
                //SHER-141
                ctrl.AddAddressFieldInTargetBasedOnMainLocation();

                // SHER-201
                ctrl.SetContactGreetingBasedOnLanguageAndGender();

                // SHER-379
                ctrl.SetContactDirectLineBasedOnCounterpartyMainPhone();
            }
            catch(Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }

		}
	}
}
