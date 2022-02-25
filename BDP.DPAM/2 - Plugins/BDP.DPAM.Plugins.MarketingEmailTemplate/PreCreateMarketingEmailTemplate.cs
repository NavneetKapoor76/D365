using BDP.DPAM.Shared.Manager_Base;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDP.DPAM.Plugins.MarketingEmailTemplate
{
      public class PreCreateMarketingEmailTemplate : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            try
            {
                MarketingEmailTemplateController ctrl = new MarketingEmailTemplateController(serviceProvider);
                ctrl.ValidatePipeline("msdyncrm_marketingemailtemplate", "create", PluginStage.PreOperation);
            

                //SHER-812
                ctrl.DefaultFromFields();
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }
    }
}