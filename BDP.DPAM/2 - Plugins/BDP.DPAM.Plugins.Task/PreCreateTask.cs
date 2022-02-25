using BDP.DPAM.Shared.Manager_Base;
using Microsoft.Xrm.Sdk;
using System;

namespace BDP.DPAM.Plugins.Task
{
    public class PreCreateTask : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            try
            {
                TaskController ctrl = new TaskController(serviceProvider);
                ctrl.ValidatePipeline("task", "create", PluginStage.PreOperation);
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
