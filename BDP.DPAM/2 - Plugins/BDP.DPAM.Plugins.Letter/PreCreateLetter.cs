using BDP.DPAM.Shared.Manager_Base;
using Microsoft.Xrm.Sdk;
using System;

namespace BDP.DPAM.Plugins.Letter
{
    public class PreCreateLetter : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            try
            {
                LetterController ctrl = new LetterController(serviceProvider);
                ctrl.ValidatePipeline("letter", "create", PluginStage.PreOperation);
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
