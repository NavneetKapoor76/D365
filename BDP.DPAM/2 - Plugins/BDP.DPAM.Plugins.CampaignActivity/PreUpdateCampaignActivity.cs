using BDP.DPAM.Shared.Manager_Base;
using Microsoft.Xrm.Sdk;
using System;

namespace BDP.DPAM.Plugins.CampaignActivity
{
    public class PreUpdateCampaignActivity : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            try
            {
                CampaignActivityController ctrl = new CampaignActivityController(serviceProvider);
                ctrl.ValidatePipeline("campaignactivity", "update", PluginStage.PreOperation);
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
