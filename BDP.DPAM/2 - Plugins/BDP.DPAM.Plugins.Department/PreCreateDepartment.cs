using BDP.DPAM.Shared.Manager_Base;
using Microsoft.Xrm.Sdk;
using System;

namespace BDP.DPAM.Plugins.Department
{
    public class PreCreateDepartment : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            try
            {
                DepartmentController ctrl = new DepartmentController(serviceProvider);
                ctrl.ValidatePipeline("dpam_departments", "create", PluginStage.PreOperation);
                //SHER-324
                ctrl.FillInBusinessSegmentation();
                
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }
    }
}
