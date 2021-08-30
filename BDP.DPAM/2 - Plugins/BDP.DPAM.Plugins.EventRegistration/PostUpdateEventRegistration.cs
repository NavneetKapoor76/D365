using BDP.DPAM.Shared.Manager_Base;
using Microsoft.Xrm.Sdk;
using System;

namespace BDP.DPAM.Plugins.EventRegistration
{
    public class PostUpdateEventRegistration : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            try
            {
                EventRegistrationController ctrl = new EventRegistrationController(serviceProvider);
                ctrl.ValidatePipeline("msevtmgt_eventregistration", "update", PluginStage.PostOperation);

                //SHER-258
                ctrl.DeactivateRegistrationResponses();

                //SHER-260
                ctrl.CancelRelatedSessionRegistrations();
            }
            catch (Exception ex)
            {

                throw new InvalidPluginExecutionException(ex.Message);
            }
        }
    }
}
