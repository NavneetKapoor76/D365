using BDP.DPAM.Shared.Manager_Base;
using Microsoft.Xrm.Sdk;
using System;

namespace BDP.DPAM.Plugins.Appointment
{
    public class PreUpdateAppointment : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            try
            {
                AppointmentController ctrl = new AppointmentController(serviceProvider);
                ctrl.ValidatePipeline("appointment", "update", PluginStage.PreOperation);
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
