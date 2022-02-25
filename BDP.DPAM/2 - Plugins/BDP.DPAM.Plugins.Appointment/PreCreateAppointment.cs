using BDP.DPAM.Shared.Manager_Base;
using Microsoft.Xrm.Sdk;
using System;

namespace BDP.DPAM.Plugins.Appointment
{
    public class PreCreateAppointment : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            try
            {
                AppointmentController ctrl = new AppointmentController(serviceProvider);
                ctrl.ValidatePipeline("appointment", "create", PluginStage.PreOperation);
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
