using BDP.DPAM.Shared.Helper;
using BDP.DPAM.Shared.Manager_Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDP.DPAM.Plugins.OpportunityClose
{
    internal class OpportunityCloseController : PluginManagerBase
    {
        internal OpportunityCloseController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        /// <summary>
        /// Manage the Sort Date Column
        /// </summary>
        internal void ManageSortDateColumn()
        {
            _tracing.Trace("ManageSortDateColumn - Start");

            CommonLibrary.ManageSortDateColumnOnActivity(_target, "modifiedon");

            _tracing.Trace("ManageSortDateColumn - End");
        }
    }
}
