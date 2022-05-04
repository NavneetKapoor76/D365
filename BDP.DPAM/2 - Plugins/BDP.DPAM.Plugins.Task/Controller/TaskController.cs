using BDP.DPAM.Shared.Helper;
using BDP.DPAM.Shared.Manager_Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDP.DPAM.Plugins.Task
{
    internal class TaskController : PluginManagerBase
    {
        internal TaskController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        /// <summary>
        /// Manage the Sort Date Column
        /// </summary>
        internal void ManageSortDateColumn()
        {
            _tracing.Trace("ManageSortDateColumn - Start");

            CommonLibrary.ManageSortDateColumnOnActivity(_target, "scheduledstart");

            _tracing.Trace("ManageSortDateColumn - End");
        }
    }
}
