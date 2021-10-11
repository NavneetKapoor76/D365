using BDP.DPAM.Shared.Manager_Base;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDP.DPAM.Plugins.Lead
{
    internal class LeadController : PluginManagerBase
    {
        internal LeadController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        /// <summary>
        /// Fill in the field dpam_lk_businesssegmentation
        /// </summary>
        internal void FillInBusinessSegmentation()
        {
            if (!_target.Contains("dpam_lk_localbusinesssegmentation")) return;

            _tracing.Trace("FillInBusinessSegmentation - Start");

            var localBusinessSegmentation = _target.GetAttributeValue<EntityReference>("dpam_lk_localbusinesssegmentation");

            if (localBusinessSegmentation != null)
            {
                var localBusinessSegmentationEntity = _service.Retrieve(localBusinessSegmentation.LogicalName, localBusinessSegmentation.Id, new ColumnSet("dpam_lk_businesssegmentation"));
                _target["dpam_lk_businesssegmentation"] = localBusinessSegmentationEntity.GetAttributeValue<EntityReference>("dpam_lk_businesssegmentation");
            }
            else if(_context.MessageName.ToLower() == MessageName.Update.ToLower())
            {
                _target["dpam_lk_businesssegmentation"] = null;
            }

            _tracing.Trace("FillInBusinessSegmentation - End");
        }
    }
}
