using System;
using BDP.DPAM.Shared.Extension_Methods;
using BDP.DPAM.Shared.Helper;
using BDP.DPAM.Shared.Manager_Base;
using Microsoft.Xrm.Sdk;

namespace BDP.DPAM.Plugins.Opportunity
{
    internal class OpportunityController : PluginManagerBase
    {
        internal OpportunityController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        /// <summary>
        /// Sets the default price list on creation of an opportunity
        /// </summary>
        internal void SetDefaultPriceList()
        {
            _tracing.Trace("SetDefaultPriceList - Start");

            _target["pricelevelid"] = new EntityReference("pricelevel", new Guid("3d11a17d-4020-ec11-b6e6-000d3aaf3bd5")); // Default "DPAM" Price List

            _tracing.Trace("SetDefaultPriceList - End");
        }
    }
}
