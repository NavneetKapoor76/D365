using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BDP.DPAM.Shared.Extension_Methods;
using BDP.DPAM.Shared.Manager_Base;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace BDP.DPAM.Plugins.Account.Controller
{
    class AccountController : PluginManagerBase
    {

        internal AccountController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }


        internal void CompleteSegmentation()
        {
            if (!this._target.Contains("dpam_lk_localbusinesssegmentation"))
                return;

            this._tracing.Trace("CompleteSegmentation on Account - Start");

            Entity mergedAccount = this._target.MergeEntity(this._preImage);

            var localSegmentationEntityReference = _target.GetAttributeValue<EntityReference>("dpam_lk_localbusinesssegmentation");

            if (localSegmentationEntityReference == null && _context.MessageName == MessageName.Update)
            {
                _tracing.Trace("CompleteSegmentation on Account - End");
                return;
            }

            var localSegmentationEntity = new Entity("dpam_localbusinesssegmentation");
            if (localSegmentationEntityReference != null)
            {
                var columnSet = new ColumnSet("dpam_lk_businesssegmentation");
                localSegmentationEntity = _service.Retrieve(localSegmentationEntityReference.LogicalName, localSegmentationEntityReference.Id, columnSet);
                var segmentation = _service.Retrieve("dpam_counterpartybusinesssegmentation", localSegmentationEntity.GetAttributeValue<EntityReference>("dpam_lk_businesssegmentation").Id, new ColumnSet("dpam_counterpartybusinesssegmentationid"));
                this._target["dpam_lk_businesssegmentation"] = segmentation.ToEntityReference();
            }
            this._tracing.Trace("CompleteSegmentation on Account - End");
        }
    }
}
