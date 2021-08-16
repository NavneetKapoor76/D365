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
                ctrl.AddAddressFieldInTargetBasedOnMainLocation("create");

                // SHER-201
                ctrl.SetContactGreetingBasedOnLanguageAndGender();
            }
            catch(Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }

		}
	}
}
